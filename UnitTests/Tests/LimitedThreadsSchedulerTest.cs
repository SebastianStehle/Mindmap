// ==========================================================================
// LimitedThreadsSchedulerTest.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using Hercules.Model.Utils;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace UnitTests.Tests
{
    [TestClass]
    public class LimitedThreadsSchedulerTest
    {
        private readonly TaskFactory taskFactory = new TaskFactory(new LimitedThreadsScheduler());

        [TestMethod]
        public void SingleTask()
        {
            Task task = taskFactory.StartNew(() => { });

            task.Wait();

            Assert.IsTrue(task.IsCompleted);
        }

        [TestMethod]
        public void MultipleTasks()
        {
            List<int> results = new List<int>();

            Task task1 = taskFactory.StartNew(() => results.Add(1));
            Task task2 = taskFactory.StartNew(() => results.Add(2));
            Task task3 = taskFactory.StartNew(() => results.Add(3));
            Task task4 = taskFactory.StartNew(() => results.Add(4));
            Task task5 = taskFactory.StartNew(() => results.Add(5));

            Task.WaitAll(task1, task2, task3, task4, task5);

            Assert.IsTrue(task1.IsCompleted);
            Assert.IsTrue(task2.IsCompleted);
            Assert.IsTrue(task3.IsCompleted);
            Assert.IsTrue(task4.IsCompleted);
            Assert.IsTrue(task5.IsCompleted);

            Assert.AreEqual(5, results.Count);
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(2, results[1]);
            Assert.AreEqual(3, results[2]);
            Assert.AreEqual(4, results[3]);
            Assert.AreEqual(5, results[4]);
        }
    }
}
