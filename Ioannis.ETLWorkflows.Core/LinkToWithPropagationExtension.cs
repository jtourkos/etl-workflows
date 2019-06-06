using System;
using System.Threading.Tasks.Dataflow;

namespace ETLWorkflows.Core
{
    public static class LinkToWithPropagateCompletionExtension
    {
        /// <summary>
        /// Links a source to a target with PropagateCompletion set to true.
        /// </summary>
        /// <typeparam name="T">The type of messages the source sends to the target.</typeparam>
        /// <param name="source">The source block.</param>
        /// <param name="target">The target block.</param>
        /// <returns></returns>
        public static IDisposable LinkToWithPropagateCompletion<T>(this ISourceBlock<T> source, ITargetBlock<T> target)
        {
            return source.LinkTo(target, new DataflowLinkOptions() { PropagateCompletion = true });
        }
    }
}
