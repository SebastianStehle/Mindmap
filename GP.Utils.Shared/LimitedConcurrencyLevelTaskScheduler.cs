// ==========================================================================
// LimitedConcurrencyLevelTaskScheduler.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GP.Utils
{
    /// <summary>
    /// Provides a task scheduler that ensures a maximum concurrency level while running on top of the thread pool. 
    /// </summary>
    public sealed class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
    {
        [ThreadStatic] private static bool currentThreadIsProcessingItems;
        private readonly LinkedList<Task> tasks = new LinkedList<Task>();
        private readonly Action<Action> scheduler;
        private readonly int maxDegreeOfParallelism;
        private int delegatesQueuedOrRunning;

        /// <summary>
        /// Indicates the maximum concurrency level this <see cref="T:System.Threading.Tasks.TaskScheduler" /> is able to support.
        /// </summary>
        public override int MaximumConcurrencyLevel
        {
            get { return maxDegreeOfParallelism; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LimitedConcurrencyLevelTaskScheduler"/> class with the specified degree of parallelism.  
        /// </summary>
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism.</param>
        /// <param name="scheduler">The scheduler. Cannot be null.</param>
        public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism, Action<Action> scheduler)
        {
            Guard.GreaterEquals(maxDegreeOfParallelism, 1, nameof(maxDegreeOfParallelism));
            Guard.NotNull(scheduler, nameof(scheduler));

            this.maxDegreeOfParallelism = maxDegreeOfParallelism;

            this.scheduler = scheduler;
        }

        /// <summary>
        /// Queues a <see cref="T:System.Threading.Tasks.Task" /> to the scheduler.
        /// </summary>
        /// <param name="task">The <see cref="T:System.Threading.Tasks.Task" /> to be queued.</param>
        protected override void QueueTask(Task task)
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
            scheduler(() =>
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

                        TryExecuteTask(item);
                    }
                }
                finally
                {
                    currentThreadIsProcessingItems = false;
                }
            });
        }

        /// <summary>
        /// Determines whether the provided <see cref="T:System.Threading.Tasks.Task" /> can be executed synchronously in this call, and if it can, executes it.
        /// </summary>
        /// <param name="task">The <see cref="T:System.Threading.Tasks.Task" /> to be executed.</param>
        /// <param name="taskWasPreviouslyQueued">A Boolean denoting whether or not task has previously been queued. If this parameter is True, then the task may have been previously queued (scheduled); if False, then the task is known not to have been queued, and this call is being made in order to execute the task inline without queuing it.</param>
        /// <returns>
        /// A Boolean value indicating whether the task was executed inline.
        /// </returns>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (!currentThreadIsProcessingItems)
            {
                return false;
            }

            if (taskWasPreviouslyQueued)
            {
                return TryDequeue(task) && TryExecuteTask(task);
            }

            return TryExecuteTask(task);
        }

        /// <summary>
        /// Attempts to dequeue a <see cref="T:System.Threading.Tasks.Task" /> that was previously queued to this scheduler.
        /// </summary>
        /// <param name="task">The <see cref="T:System.Threading.Tasks.Task" /> to be dequeued.</param>
        /// <returns>
        /// A Boolean denoting whether the <paramref name="task" /> argument was successfully dequeued.
        /// </returns>
        protected override bool TryDequeue(Task task)
        {
            lock (tasks)
            {
                return tasks.Remove(task);
            }
        }

        /// <summary>
        /// For debugger support only, generates an enumerable of <see cref="T:System.Threading.Tasks.Task" /> instances currently queued to the scheduler waiting to be executed.
        /// </summary>
        /// <returns>
        /// An enumerable that allows a debugger to traverse the tasks currently queued to this scheduler.
        /// </returns>
        /// <exception cref="System.NotSupportedException"></exception>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(tasks, ref lockTaken);
                if (lockTaken)
                {
                    return tasks;
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
    }
}
