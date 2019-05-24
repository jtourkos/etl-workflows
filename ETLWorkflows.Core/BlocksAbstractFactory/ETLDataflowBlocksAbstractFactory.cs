using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ETLWorkflows.Core.BlocksAbstractFactory
{
    public class ETLDataflowBlocksAbstractFactory : IETLDataflowBlocksAbstractFactory
    {
        private readonly bool _optionsAreSet = false;
        private EtlExecutionDataflowBlockOptions _etlExecutionDataflowBlockOptions;

        public EtlExecutionDataflowBlockOptions EtlExecutionDataflowBlockOptions
        {
            // Not letting the client set more than once the options.
            // Options are meant to be read once at the startup of the workflow.
            // Since this is done at the startup of the process, where no multi threading scenario is involved, there is no need for locks.
            get => _etlExecutionDataflowBlockOptions;
            set
            {
                if (!_optionsAreSet) _etlExecutionDataflowBlockOptions = value;
                else
                {
                    throw new InvalidOperationException($"Attempt to set again {nameof(EtlExecutionDataflowBlockOptions)}. These options are set INTERNALLY only once and are handled by the framework. You must override the GetWorkflowBlockOptions in your ETLWorkflowBase subclass.");
                }
            }
        }

        public BufferBlock<TriggerRequest<TPayload>> CreateProducerBlock<TPayload>()
        {
            return new BufferBlock<TriggerRequest<TPayload>>(EtlExecutionDataflowBlockOptions.ProducerDataflowBlockOptions);
        }

        public TransformBlock<TriggerRequest<TPayload>, TExtractorResult> CreateExtractBlock<TPayload, TExtractorResult>(Func<TriggerRequest<TPayload>, Task<TExtractorResult>> func)
        {
            return new TransformBlock<TriggerRequest<TPayload>, TExtractorResult>(func, EtlExecutionDataflowBlockOptions.ExtractDataflowBlockOptions);
        }

        public TransformBlock<TExtractorResult, TExtractorResult> CreateExtractCompletedBlock<TExtractorResult>(Func<TExtractorResult, Task<TExtractorResult>> func)
        {
            return new TransformBlock<TExtractorResult, TExtractorResult>(func, EtlExecutionDataflowBlockOptions.OnExtractCompletedDataflowBlockOptions);
        }

        public TransformBlock<TExtractorResult, TTransformResult> CreateTransformBlock<TExtractorResult, TTransformResult>(Func<TExtractorResult, Task<TTransformResult>> func)
        {
            return new TransformBlock<TExtractorResult, TTransformResult>(func, EtlExecutionDataflowBlockOptions.TransformDataflowBlockOptions);
        }

        public TransformBlock<TTransformResult, TTransformResult> CreateTransformCompletedBlock<TTransformResult>(Func<TTransformResult, Task<TTransformResult>> func)
        {
            return new TransformBlock<TTransformResult, TTransformResult>(func, EtlExecutionDataflowBlockOptions.OnTransformCompletedDataflowBlockOptions);
        }

        public TransformBlock<TTransformResult, TLoadResult> CreateLoadBlock<TTransformResult, TLoadResult>(Func<TTransformResult, Task<TLoadResult>> func)
        {
            return new TransformBlock<TTransformResult, TLoadResult>(func, EtlExecutionDataflowBlockOptions.LoadDataflowBlockOptions);
        }

        public ActionBlock<TLoadResult> CreateLoadCompletedBlock<TLoadResult>(Func<TLoadResult, Task> func)
        {
            return new ActionBlock<TLoadResult>(func, EtlExecutionDataflowBlockOptions.OnLoadCompletedDataflowBlockOptions);
        }
    }
}
