// ==========================================================================
// LimitedThreadsScheduler.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace RavenMind.Model.Utils
{
    /// <summary>
    /// Provides a custom task scheduler where only one thread is used to execute the tasks.
    /// </summary>
    public sealed class LimitedThreadsScheduler : TaskScheduler
    {
        #region Fields

        [ThreadStatic]
        private static bool currentThreadIsProcessingItems;
        private readonly LinkedList<Task> tasks = new LinkedList<Task>();
        private readonly int maxDegreeOfParallelism;
        private int delegatesQueuedOrRunning;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the maximum concurrency level supported by this scheduler.
        /// </summary>
        /// <value>
        /// The maximum concurrency level.
        /// </value>
        public sealed override int MaximumConcurrencyLevel
        {
            get
            {
                return maxDegreeOfParallelism;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LimitedThreadsScheduler" /> class.
        /// </summary>
        public LimitedThreadsScheduler()
            : this(1)
        {
        }

        /// <summary>
        /// Initializes an instance of the LimitedConcurrencyLevelTaskScheduler class with the
        /// specified degree of parallelism.
        /// </summary>
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism provided by this scheduler.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="maxDegreeOfParallelism"/> is smaller than one.</exception>
        public LimitedThreadsScheduler(int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism < 1)
            {
                throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");
            }

            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Queues the task.
        /// </summary>
        /// <param name="task">The task to be queued.</param>
        protected sealed override void QueueTask(Task task)
        {
            lock (tasks)
            {
                tasks.AddLast(task);

                if (delegatesQueuedOrRunning < maxDegreeOfParallelism)
                {
                    ++delegatesQueuedOrRunning;
                    NotifyThreadPoolOfPendingWork();
                }
            }
        }

        private void NotifyThreadPoolOfPendingWork()
        {
#pragma warning disable 4014
            ThreadPool.RunAsync(x =>
            {
                currentThreadIsProcessingItems = true;
                try
                { 
                    while (true)
                    {
                        Task item;

                        lock (tasks)
                        { 
                            if (tasks.Count == 0)
                            {
                                --delegatesQueuedOrRunning;
                                break;
                            }
                            item = tasks.First.Value;

                            tasks.RemoveFirst();
                        }

                        base.TryExecuteTask(item);
                    }
                }
                finally 
                { 
                    currentThreadIsProcessingItems = false; 
                }
            });
#pragma warning restore 4014
        }

        /// <summary>
        /// Attempts to execute the specified task on the current thread.
        /// </summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="taskWasPreviouslyQueued">
        /// A Boolean denoting whether or not task has previously been queued. If this
        /// parameter is True, then the task may have been previously queued (scheduled);
        /// if False, then the task is known not to have been queued, and this call is
        /// being made in order to execute the task inline without queuing it.
        /// </param>
        /// <returns>
        /// Whether the task could be executed on the current thread.
        /// </returns>
        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (!currentThreadIsProcessingItems)
            {
                return false;
            }

            if (taskWasPreviouslyQueued)
            {
                TryDequeue(task);
            }

            return TryExecuteTask(task);
        }

        /// <summary>
        /// Attempts to remove a previously scheduled task from the scheduler.
        /// </summary>
        /// <param name="task">The task to be removed.</param>
        /// <returns>
        /// Whether the task could be found and removed.
        /// </returns>
        protected sealed override bool TryDequeue(Task task)
        {
            lock (tasks)
            {
                return tasks.Remove(task);
            }
        }

        /// <summary>
        /// Gets an enumerable of the tasks currently scheduled on this scheduler.
        /// </summary>
        /// <returns>
        /// An enumerable of the tasks currently scheduled.
        /// </returns>
        /// <exception cref="System.NotSupportedException"></exception>
        protected sealed override IEnumerable<Task> GetScheduledTasks()
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(tasks, ref lockTaken);

                if (lockTaken)
                {
                    return tasks.ToArray();
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(tasks);
                }
            }
        }

        #endregion
    }
}
