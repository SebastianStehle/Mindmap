// ==========================================================================
// LimitedThreadsSchedulerTest.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GP.Utils;
using Xunit;
// ReSharper disable ImplicitlyCapturedClosure

namespace Tests.Facts
{
    public class LimitedConcurrencyLevelTaskSchedulerTest
    {
        private readonly LimitedConcurrencyLevelTaskScheduler scheduler;
        private readonly TaskFactory taskFactory;

        public LimitedConcurrencyLevelTaskSchedulerTest()
        {
            scheduler = new LimitedConcurrencyLevelTaskScheduler(1, x => ThreadPool.QueueUserWorkItem(y => x(), null));

            taskFactory = new TaskFactory(scheduler);
        }

        [Fact]
        public void SingleTask()
        {
            Task task = taskFactory.StartNew(() => { });

            task.Wait();

            Assert.Equal(1, scheduler.MaximumConcurrencyLevel);
            Assert.True(task.IsCompleted);
        }

        [Fact]
        public void NestedTasks()
        {
            List<int> results = new List<int>();

            Task task = taskFactory.StartNew(() =>
            {
                results.Add(1);

                taskFactory.StartNew(() => results.Add(2)).Wait();
            });

            task.Wait();

            Assert.Equal(1, results[0]);
            Assert.Equal(2, results[1]);
        }

        [Fact]
        public void MultipleTasks()
        {
            List<int> results = new List<int>();

            Task task1 = taskFactory.StartNew(() => results.Add(1));
            Task task2 = taskFactory.StartNew(() => results.Add(2));
            Task task3 = taskFactory.StartNew(() => results.Add(3));
            Task task4 = taskFactory.StartNew(() => results.Add(4));
            Task task5 = taskFactory.StartNew(() => results.Add(5));

            Task.WaitAll(task1, task2, task3, task4, task5);

            Assert.True(task1.IsCompleted);
            Assert.True(task2.IsCompleted);
            Assert.True(task3.IsCompleted);
            Assert.True(task4.IsCompleted);
            Assert.True(task5.IsCompleted);

            Assert.Equal(5, results.Count);
            Assert.Equal(1, results[0]);
            Assert.Equal(2, results[1]);
            Assert.Equal(3, results[2]);
            Assert.Equal(4, results[3]);
            Assert.Equal(5, results[4]);
        }
    }
}
