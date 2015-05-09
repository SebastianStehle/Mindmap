// ==========================================================================
// OrderedCollectionTest.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RavenMind.Mockups;
using RavenMind.Model;

namespace RavenMind.Tests
{
    [TestClass]
    public class OrderedCollectionTest
    {
        [TestMethod]
        public void TestOrderIndexChangedAfter()
        {
            MockupSelectableOrderedItem item1 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item2 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item3 = new MockupSelectableOrderedItem();

            OrderedCollection<MockupSelectableOrderedItem> collection = new OrderedCollection<MockupSelectableOrderedItem>();

            collection.Add(item1);
            collection.Add(item2);
            collection.Add(item3);

            item1.OrderIndex = 1000;

            Assert.AreEqual(3, item1.OrderIndex);
            Assert.AreEqual(1, item2.OrderIndex);
            Assert.AreEqual(2, item3.OrderIndex);

            Assert.AreEqual(2, collection.IndexOf(item1));
            Assert.AreEqual(0, collection.IndexOf(item2));
            Assert.AreEqual(1, collection.IndexOf(item3));
        }

        [TestMethod]
        public void TestOrderIndexChangedBefore()
        {
            MockupSelectableOrderedItem item1 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item2 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item3 = new MockupSelectableOrderedItem();

            OrderedCollection<MockupSelectableOrderedItem> collection = new OrderedCollection<MockupSelectableOrderedItem>();

            collection.Add(item1);
            collection.Add(item2);
            collection.Add(item3);

            item3.OrderIndex = 0;

            Assert.AreEqual(2, item1.OrderIndex);
            Assert.AreEqual(3, item2.OrderIndex);
            Assert.AreEqual(1, item3.OrderIndex);

            Assert.AreEqual(1, collection.IndexOf(item1));
            Assert.AreEqual(2, collection.IndexOf(item2));
            Assert.AreEqual(0, collection.IndexOf(item3));
        }

        [TestMethod]
        public void TestOrderIndexChanged()
        {
            MockupSelectableOrderedItem item1 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item2 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item3 = new MockupSelectableOrderedItem();

            OrderedCollection<MockupSelectableOrderedItem> collection = new OrderedCollection<MockupSelectableOrderedItem>();

            collection.Add(item1);
            collection.Add(item2);
            collection.Add(item3);

            item3.OrderIndex = 2;

            Assert.AreEqual(1, item1.OrderIndex);
            Assert.AreEqual(3, item2.OrderIndex);
            Assert.AreEqual(2, item3.OrderIndex);

            Assert.AreEqual(0, collection.IndexOf(item1));
            Assert.AreEqual(2, collection.IndexOf(item2));
            Assert.AreEqual(1, collection.IndexOf(item3));
        }

        [TestMethod]
        public void TestMove()
        {
            MockupSelectableOrderedItem item1 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item2 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item3 = new MockupSelectableOrderedItem();

            OrderedCollection<MockupSelectableOrderedItem> collection = new OrderedCollection<MockupSelectableOrderedItem>();

            collection.Add(item1);
            collection.Add(item2);
            collection.Add(item3);
            collection.Move(0, 2);

            Assert.AreEqual(3, item1.OrderIndex);
            Assert.AreEqual(1, item2.OrderIndex);
            Assert.AreEqual(2, item3.OrderIndex);
        }

        [TestMethod]
        public void TestReplace()
        {
            MockupSelectableOrderedItem item1 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item2 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item3 = new MockupSelectableOrderedItem();

            OrderedCollection<MockupSelectableOrderedItem> collection = new OrderedCollection<MockupSelectableOrderedItem>();

            collection.Add(item1);
            collection.Add(item2);
            collection[0] = item3;

            Assert.AreEqual(1, item1.OrderIndex);
            Assert.AreEqual(2, item2.OrderIndex);
            Assert.AreEqual(1, item3.OrderIndex);
        }

        [TestMethod]
        public void TestInsertBegin()
        {
            MockupSelectableOrderedItem item1 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item2 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item3 = new MockupSelectableOrderedItem();

            OrderedCollection<MockupSelectableOrderedItem> collection = new OrderedCollection<MockupSelectableOrderedItem>();

            collection.Add(item1);
            collection.Add(item2);
            collection.Insert(0, item3);

            Assert.AreEqual(2, item1.OrderIndex);
            Assert.AreEqual(3, item2.OrderIndex);
            Assert.AreEqual(1, item3.OrderIndex);
        }

        [TestMethod]
        public void TestInsertMiddle()
        {
            MockupSelectableOrderedItem item1 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item2 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item3 = new MockupSelectableOrderedItem();

            OrderedCollection<MockupSelectableOrderedItem> collection = new OrderedCollection<MockupSelectableOrderedItem>();

            collection.Add(item1);
            collection.Add(item2);
            collection.Insert(1, item3);

            Assert.AreEqual(1, item1.OrderIndex);
            Assert.AreEqual(3, item2.OrderIndex);
            Assert.AreEqual(2, item3.OrderIndex);
        }

        [TestMethod]
        public void TestAdd()
        {
            MockupSelectableOrderedItem item1 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item2 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item3 = new MockupSelectableOrderedItem();

            OrderedCollection<MockupSelectableOrderedItem> collection = new OrderedCollection<MockupSelectableOrderedItem>();

            collection.Add(item1);

            Assert.AreEqual(1, item1.OrderIndex);

            collection.Add(item2);

            Assert.AreEqual(1, item1.OrderIndex);
            Assert.AreEqual(2, item2.OrderIndex);

            collection.Add(item3);

            Assert.AreEqual(1, item1.OrderIndex);
            Assert.AreEqual(2, item2.OrderIndex);
            Assert.AreEqual(3, item3.OrderIndex);
        }

        [TestMethod]
        public void TestRemoveBegin()
        {
            MockupSelectableOrderedItem item1 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item2 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item3 = new MockupSelectableOrderedItem();

            OrderedCollection<MockupSelectableOrderedItem> collection = new OrderedCollection<MockupSelectableOrderedItem>();

            collection.Add(item1);
            collection.Add(item2);
            collection.Add(item3);
            collection.Remove(item1);

            Assert.AreEqual(1, item2.OrderIndex);
            Assert.AreEqual(2, item3.OrderIndex);
        }

        [TestMethod]
        public void TestRemoveMiddle()
        {
            MockupSelectableOrderedItem item1 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item2 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item3 = new MockupSelectableOrderedItem();

            OrderedCollection<MockupSelectableOrderedItem> collection = new OrderedCollection<MockupSelectableOrderedItem>();

            collection.Add(item1);
            collection.Add(item2);
            collection.Add(item3);
            collection.Remove(item2);

            Assert.AreEqual(1, item1.OrderIndex);
            Assert.AreEqual(2, item3.OrderIndex);
        }

        [TestMethod]
        public void TestRemoveEnd()
        {
            MockupSelectableOrderedItem item1 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item2 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item3 = new MockupSelectableOrderedItem();

            OrderedCollection<MockupSelectableOrderedItem> collection = new OrderedCollection<MockupSelectableOrderedItem>();

            collection.Add(item1);
            collection.Add(item2);
            collection.Add(item3);
            collection.Remove(item3);

            Assert.AreEqual(1, item1.OrderIndex);
            Assert.AreEqual(2, item2.OrderIndex);
        }

        [TestMethod]
        public void TestClear()
        {
            MockupSelectableOrderedItem item1 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item2 = new MockupSelectableOrderedItem();

            OrderedCollection<MockupSelectableOrderedItem> collection = new OrderedCollection<MockupSelectableOrderedItem>();

            collection.Add(item1);
            collection.Add(item2);
            collection.Clear();

            Assert.AreEqual(1, item1.OrderIndex);
            Assert.AreEqual(2, item2.OrderIndex);
        }
    }
}
