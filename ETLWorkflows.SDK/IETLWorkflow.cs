using System.Threading;
using System.Threading.Tasks;

namespace ETLWorkflows.SDK
{
    /// <summary>
    /// Convenient interface for Dependency injection. The functionality you'll need leaves in <see cref="ETLWorkflowBase{TRunRequest,TExtractorResult,TTransformerResult,TLoaderResult}"/>
    /// Usually what you should do, is to subclass your workflow class from <see cref="ETLWorkflowBase{TTriggerRequest,TExtractorResult,TTransformerResult,TLoaderResult}"/> and register this class in your favorite IoC Container with this interface.
    /// Clients make use only the <see cref="StartWorkflowAsync"/> method.
    /// </summary>
    /// <typeparam name="TRunRequest"></typeparam>
    /// <typeparam name="TExtractorResult"></typeparam>
    /// <typeparam name="TTransformerResult"></typeparam>
    /// <typeparam name="TLoaderResult"></typeparam>
    public interface IETLWorkflow<out TRunRequest, TExtractorResult, TTransformerResult, TLoaderResult>
    {
        /// <summary>
        /// Starts the workflow.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for cancelling the workflow.</param>
        /// <returns>A task to monitor the workflow.</returns>
        Task StartWorkflowAsync(CancellationToken cancellationToken);
    }
}