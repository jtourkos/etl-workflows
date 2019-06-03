using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ETLWorkflows.Core;
using ETLWorkflows.Core.BlocksAbstractFactory;

[assembly: InternalsVisibleTo("ETLWorkflows.SDK.Tests")]

namespace ETLWorkflows.SDK
{
    /// <summary>
    /// Base class for creating ETL workflows.
    /// To implement your ETL flow, subclass your workflow class and implement the ETL methods. Hook methods are provided as well.
    /// </summary>
    /// <typeparam name="TPayload">The type of triggering request's payload.</typeparam>
    /// <typeparam name="TExtractorResult">The type of the result of the extraction step.</typeparam>
    /// <typeparam name="TTransformerResult">The type of the result of the transformation step.</typeparam>
    /// <typeparam name="TLoaderResult">The type of the result of the loading step.</typeparam>
    public abstract class ETLWorkflowBase<TPayload, TExtractorResult, TTransformerResult, TLoaderResult> : IETLWorkflow<TPayload, TExtractorResult, TTransformerResult, TLoaderResult>
    {
        protected ILogger _logger;
        private readonly IETLDataflowBlocksAbstractFactory _etlBlocksAbstractFactory;

        /// <param name="logger">A logger implementation.</param>
        protected ETLWorkflowBase(ILogger logger = null)
            : this(logger, new ETLDataflowBlocksAbstractFactory())
        {

        }

        /// Internal constructor for testing.
        internal ETLWorkflowBase(
            ILogger logger,
            IETLDataflowBlocksAbstractFactory etlBlocksAbstractFactory)
        {
            _logger = logger;
            _etlBlocksAbstractFactory = etlBlocksAbstractFactory ?? throw new ArgumentNullException(nameof(etlBlocksAbstractFactory));
        }

        /// <summary>
        /// Starts the workflow. Call this method from your client.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for cancelling the workflow.</param>
        /// <returns>A task to monitor the workflow.</returns>
        public async Task StartWorkflowAsync(CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Configure the pipeline.
                // Options are set ONLY ONCE, when the workflow bootstraps, here.
                // The setter is implemented in a way that will not allow a set command being called twice.
                // So, there is no way the client can access and/or set this property from "the outside".
                _etlBlocksAbstractFactory.EtlExecutionDataflowBlockOptions = GetWorkflowBlockOptions();

                // Step 2: Create the blocks.
                var producer = _etlBlocksAbstractFactory.CreateProducerBlock<TPayload>();

                var extractBlock = _etlBlocksAbstractFactory.CreateExtractBlock<TPayload, TExtractorResult>(ExtractAsync);
                var extractCompletedBlock = _etlBlocksAbstractFactory.CreateExtractCompletedBlock<TExtractorResult>(OnExtractCompletedAsync);

                var transformBlock = _etlBlocksAbstractFactory.CreateTransformBlock<TExtractorResult, TTransformerResult>(TransformAsync);
                var transformCompletedBlock = _etlBlocksAbstractFactory.CreateTransformCompletedBlock<TTransformerResult>(OnTransformCompletedAsync);

                var loadBlock = _etlBlocksAbstractFactory.CreateLoadBlock<TTransformerResult, TLoaderResult>(LoadAsync);
                var loadCompletedBlock = _etlBlocksAbstractFactory.CreateLoadCompletedBlock<TLoaderResult>(OnLoadCompletedAsync);

                // Step 3: Link blocks.
                producer.LinkToWithPropagateCompletion(extractBlock);
                extractBlock.LinkToWithPropagateCompletion(extractCompletedBlock);
                extractCompletedBlock.LinkToWithPropagateCompletion(transformBlock);
                transformBlock.LinkToWithPropagateCompletion(transformCompletedBlock);
                transformCompletedBlock.LinkToWithPropagateCompletion(loadBlock);
                loadBlock.LinkToWithPropagateCompletion(loadCompletedBlock);

                // Step 4: Start the producer in a new Task.
                var receiveMessagesTask = Task.Factory.StartNew(() =>
                {
                    while (!cancellationToken.IsCancellationRequested && !extractBlock.Completion.IsCompleted)
                    {
                        FeedProducerAsync(producer, cancellationToken, _logger);
                    }
                }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                // Step 5: Keep going until the CancellationToken is cancelled, or the leaf block is completed, either due to a fault or the completion of the workflow.
                while (!cancellationToken.IsCancellationRequested && !loadCompletedBlock.Completion.IsCompleted)
                {
                    await Task.Delay(500);
                }

                // Step 6: the CancellationToken has been cancelled and the producer has stopped producing.
                // Call Complete on the first block, this will propagate down the workflow.
                extractBlock.Complete();

                // Step 7: Wait for the leaf block to finish processing its data.
                await Task.WhenAll(loadCompletedBlock.Completion, receiveMessagesTask);

                // Step 8: Clean up any other resources like RabbitMQ for example.
                await CleanupAsync();
            }
            catch (Exception e)
            {
                _logger.Error($"Error occurred at the workflow: {e.GetBaseException().Message}" +
                                  Environment.NewLine + $"StackTrace: {e.StackTrace}");
            }
        }

        #region Workflow Producer

        /// <summary>
        /// Start sending messages (triggering requests) to workflow's producer.
        /// </summary>
        /// <param name="targetBlock">The producer block.</param>
        /// <param name="cancellationToken">Cancellation token for cancelling the producer's feeding.</param>
        /// <param name="logger">An optional logger.</param>
        /// <returns>A task to monitor consumer's message feeding.</returns>
        public abstract Task FeedProducerAsync(ITargetBlock<TriggerRequest<TPayload>> targetBlock,
            CancellationToken cancellationToken, ILogger logger = null);

        #endregion

        #region ETL Steps

        /// <summary>
        /// When a triggering request is sent to the producer from <see cref="FeedProducerAsync"/>, the producer pushes this request to the Extraction step to initiate the ETL process.
        /// You can pass any custom data in your request's payload, if necessary.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A task to monitor the extraction step.</returns>
        protected abstract Task<TExtractorResult> ExtractAsync(TriggerRequest<TPayload> request);

        /// <summary>
        /// Receives extraction's completion result, performs some transformation returning and returns new result.
        /// </summary>
        /// <param name="extractorResult"></param>
        /// <returns>A task to monitor the transformation step.</returns>
        protected abstract Task<TTransformerResult> TransformAsync(TExtractorResult extractorResult);

        /// <summary>
        /// Receives transformer's completion result and performs a custom loading operation with it.
        /// </summary>
        /// <param name="transformerResult"></param>
        /// <returns>A task to monitor the loading step.</returns>
        protected abstract Task<TLoaderResult> LoadAsync(TTransformerResult transformerResult);

        #endregion

        #region Hooks

        /// <summary>
        /// Implement this method in case you need custom logic after the extraction step is completed.
        /// Avoid modifying the <see cref="TExtractorResult"/> but instead perform side effects with the result if needed.
        /// </summary>
        /// <param name="extractorResult"></param>
        /// <returns>A task to monitor the this step.</returns>
        protected virtual Task<TExtractorResult> OnExtractCompletedAsync(TExtractorResult extractorResult)
        {
            _logger.Info($"Extract step completed for {extractorResult}");

            return Task.FromResult(extractorResult);
        }

        /// <summary>
        /// Implement this method in case you need custom logic after the transformation step is completed.
        /// Avoid modifying the <see cref="TTransformerResult"/> but instead perform side effects with the result if needed.
        /// </summary>
        /// <param name="transformerResult"></param>
        /// <returns>A task to monitor the this step.</returns>
        protected virtual Task<TTransformerResult> OnTransformCompletedAsync(TTransformerResult transformerResult)
        {
            _logger.Info("Transform step completed");

            return Task.FromResult(transformerResult);
        }

        /// <summary>
        /// Implement this method in case you need custom logic after the loading step is completed.
        /// Avoid modifying the <see cref="TLoaderResult"/> but instead perform side effects with the result if needed.
        /// </summary>
        /// <param name="loadResult"></param>
        /// <returns>A task to monitor the this step.</returns>
        protected virtual Task<TLoaderResult> OnLoadCompletedAsync(TLoaderResult loadResult)
        {
            _logger.Info("Load step completed");

            return Task.FromResult(loadResult);
        }


        /// <summary>
        /// Override this method to provide any cleanup logic you may need. This method is called after the workflow is shut down.
        /// </summary>
        /// <returns>A task to monitor the cleanup operation.</returns>
        protected virtual Task CleanupAsync()
        {
            _logger.Info("Cleaning up any resources");

            return Task.CompletedTask;
        }

        #endregion

        #region Other Workflow Methods

        /// <summary>
        /// Gets the configuration options for the workflow. If the client does not override this method default <see cref="EtlExecutionDataflowBlockOptions"/> are returned. 
        /// </summary>
        /// <returns>The ETL workflow's options.</returns>
        protected virtual EtlExecutionDataflowBlockOptions GetWorkflowBlockOptions()
        {
            return EtlExecutionDataflowBlockOptions.DefaultOptions;
        }

        #endregion
    }
}
