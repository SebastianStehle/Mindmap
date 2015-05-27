// ==========================================================================
// LimitedThreadsScheduler.cs
// Mindmap Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GreenParrot.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;

namespace Mindmap.Model.Utils
{
    public sealed class LimitedThreadsScheduler : TaskScheduler
    {
        [ThreadStatic]
        private static bool currentThreadIsProcessingItems;
        private readonly LinkedList<Task> tasks = new LinkedList<Task>();
        private readonly int maxDegreeOfParallelism;
        private int delegatesQueuedOrRunning;

        public sealed override int MaximumConcurrencyLevel
        {
            get
            {
                return maxDegreeOfParallelism;
            }
        }

        public LimitedThreadsScheduler()
            : this(1)
        {
        }

        public LimitedThreadsScheduler(int maxDegreeOfParallelism)
        {
            Guard.GreaterEquals(maxDegreeOfParallelism, 1, "maxDegreeOfParallelism");

            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

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

        protected sealed override bool TryDequeue(Task task)
        {
            lock (tasks)
            {
                return tasks.Remove(task);
            }
        }

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
    }
}
