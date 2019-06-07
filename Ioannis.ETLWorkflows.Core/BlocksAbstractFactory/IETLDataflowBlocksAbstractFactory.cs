using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Ioannis.ETLWorkflows.Core.Models;

namespace Ioannis.ETLWorkflows.Core.BlocksAbstractFactory
{
    /// <summary>
    /// Provides all the necessary factory methods of creating the workflow's blocks.
    /// </summary>
    public interface IETLDataflowBlocksAbstractFactory
    {
        EtlExecutionDataflowBlockOptions EtlExecutionDataflowBlockOptions { get; set; }

        BufferBlock<TriggerRequest> CreateProducerBlock<TPayload>();
        TransformBlock<TriggerRequest, TExtractorResult> CreateExtractBlock<TPayload, TExtractorResult>(Func<TriggerRequest, Task<TExtractorResult>> func);
        TransformBlock<TExtractorResult, TExtractorResult> CreateExtractCompletedBlock<TExtractorResult>(Func<TExtractorResult, Task<TExtractorResult>> func);
        TransformBlock<TExtractorResult, TTransformResult> CreateTransformBlock<TExtractorResult, TTransformResult>(Func<TExtractorResult, Task<TTransformResult>> func);
        TransformBlock<TTransformResult, TTransformResult> CreateTransformCompletedBlock<TTransformResult>(Func<TTransformResult, Task<TTransformResult>> func);
        TransformBlock<TTransformResult, TLoadResult> CreateLoadBlock<TTransformResult, TLoadResult>(Func<TTransformResult, Task<TLoadResult>> func);
        ActionBlock<TLoadResult> CreateLoadCompletedBlock<TLoadResult>(Func<TLoadResult, Task> action);
    }
}