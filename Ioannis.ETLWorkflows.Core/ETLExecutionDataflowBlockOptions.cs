using System;
using System.Threading.Tasks.Dataflow;

namespace ETLWorkflows.Core
{
    /// <summary>
    /// Contains the configuration options for all workflow components.
    /// </summary>
    public class EtlExecutionDataflowBlockOptions
    {
        public DataflowBlockOptions ProducerDataflowBlockOptions { get; set; }
        public ExecutionDataflowBlockOptions LoadDataflowBlockOptions { get; set; }
        public ExecutionDataflowBlockOptions ExtractDataflowBlockOptions { get; set; }
        public ExecutionDataflowBlockOptions TransformDataflowBlockOptions { get; set; }
        public ExecutionDataflowBlockOptions OnLoadCompletedDataflowBlockOptions { get; set; }
        public ExecutionDataflowBlockOptions OnExtractCompletedDataflowBlockOptions { get; set; }
        public ExecutionDataflowBlockOptions OnTransformCompletedDataflowBlockOptions { get; set; }

        public EtlExecutionDataflowBlockOptions(
                DataflowBlockOptions producerDataflowBlockOptions,
                ExecutionDataflowBlockOptions extractDataflowBlockOptions,
                ExecutionDataflowBlockOptions onExtractCompletedDataflowBlockOptions,
                ExecutionDataflowBlockOptions transformDataflowBlockOptions,
                ExecutionDataflowBlockOptions onTransformCompletedDataflowBlockOptions,
                ExecutionDataflowBlockOptions loadDataflowBlockOptions,
                ExecutionDataflowBlockOptions onLoadCompletedDataflowBlockOptions
        )
        {
            LoadDataflowBlockOptions = loadDataflowBlockOptions ?? throw new ArgumentNullException(nameof(loadDataflowBlockOptions));
            ExtractDataflowBlockOptions = extractDataflowBlockOptions ?? throw new ArgumentNullException(nameof(extractDataflowBlockOptions));
            TransformDataflowBlockOptions = transformDataflowBlockOptions ?? throw new ArgumentNullException(nameof(transformDataflowBlockOptions));
            ProducerDataflowBlockOptions = producerDataflowBlockOptions ?? throw new ArgumentNullException(nameof(producerDataflowBlockOptions));
            OnLoadCompletedDataflowBlockOptions = onLoadCompletedDataflowBlockOptions ?? throw new ArgumentNullException(nameof(onLoadCompletedDataflowBlockOptions));
            OnExtractCompletedDataflowBlockOptions = onExtractCompletedDataflowBlockOptions ?? throw new ArgumentNullException(nameof(onExtractCompletedDataflowBlockOptions));
            OnTransformCompletedDataflowBlockOptions = onTransformCompletedDataflowBlockOptions ?? throw new ArgumentNullException(nameof(onTransformCompletedDataflowBlockOptions));
        }

        public static EtlExecutionDataflowBlockOptions DefaultOptions = new EtlExecutionDataflowBlockOptions(
            new DataflowBlockOptions() {BoundedCapacity = 1000},
            new ExecutionDataflowBlockOptions() {MaxDegreeOfParallelism = 4},
            new ExecutionDataflowBlockOptions() {MaxDegreeOfParallelism = 4},
            new ExecutionDataflowBlockOptions() {MaxDegreeOfParallelism = 4},
            new ExecutionDataflowBlockOptions() {MaxDegreeOfParallelism = 4},
            new ExecutionDataflowBlockOptions() {MaxDegreeOfParallelism = 4},
            new ExecutionDataflowBlockOptions() {MaxDegreeOfParallelism = 4}
        );
    }
}
