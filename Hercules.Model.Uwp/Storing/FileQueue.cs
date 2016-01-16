// ==========================================================================
// FileQueue.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Windows.System.Threading;
using GP.Utils;

namespace Hercules.Model.Storing
{
    public static class FileQueue
    {
        private static readonly LimitedConcurrencyLevelTaskScheduler Scheduler = new LimitedConcurrencyLevelTaskScheduler(1, z => ThreadPool.RunAsync(x => z(), WorkItemPriority.Normal).Forget());
        private static readonly TaskFactory TaskFactory;

        static FileQueue()
        {
            TaskFactory = new TaskFactory(Scheduler);
        }

        public static Task EnqueueAsync(Func<Task> action)
        {
            return TaskFactory.StartNew(action).Unwrap();
        }

        public static Task<T> EnqueueAsync<T>(Func<Task<T>> action)
        {
            return TaskFactory.StartNew(action).Unwrap();
        }
    }
}
