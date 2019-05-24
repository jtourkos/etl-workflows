using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ETLWorkflows.Core.BlocksAbstractFactory
{
    public interface IETLDataflowBlocksAbstractFactory
    {
        EtlExecutionDataflowBlockOptions EtlExecutionDataflowBlockOptions { get; set; }

        BufferBlock<TriggerRequest<TPayload>> CreateProducerBlock<TPayload>();
        TransformBlock<TriggerRequest<TPayload>, TExtractorResult> CreateExtractBlock<TPayload, TExtractorResult>(Func<TriggerRequest<TPayload>, Task<TExtractorResult>> func);
        TransformBlock<TExtractorResult, TExtractorResult> CreateExtractCompletedBlock<TExtractorResult>(Func<TExtractorResult, Task<TExtractorResult>> func);
        TransformBlock<TExtractorResult, TTransformResult> CreateTransformBlock<TExtractorResult, TTransformResult>(Func<TExtractorResult, Task<TTransformResult>> func);
        TransformBlock<TTransformResult, TTransformResult> CreateTransformCompletedBlock<TTransformResult>(Func<TTransformResult, Task<TTransformResult>> func);
        TransformBlock<TTransformResult, TLoadResult> CreateLoadBlock<TTransformResult, TLoadResult>(Func<TTransformResult, Task<TLoadResult>> func);
        ActionBlock<TLoadResult> CreateLoadCompletedBlock<TLoadResult>(Func<TLoadResult, Task> action);
    }
}