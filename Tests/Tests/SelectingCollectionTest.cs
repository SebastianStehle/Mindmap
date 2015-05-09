// ==========================================================================
// SelectingCollectionTest.cs
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
    public class SelectingCollectionTest
    {
        [TestMethod]
        public void TestUnselectByCollection()
        {
            MockupSelectableOrderedItem item1 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item2 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item3 = new MockupSelectableOrderedItem();

            SelectingCollection<MockupSelectableOrderedItem> collection = new SelectingCollection<MockupSelectableOrderedItem>();

            collection.Add(item1);
            collection.Add(item2);
            collection.Add(item3);

            collection.SelectedItem = item2;
            collection.SelectedItem = null;

            Assert.IsFalse(item2.IsSelected);
        }

        [TestMethod]
        public void TestSelectByCollection()
        {
            MockupSelectableOrderedItem item1 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item2 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item3 = new MockupSelectableOrderedItem();

            SelectingCollection<MockupSelectableOrderedItem> collection = new SelectingCollection<MockupSelectableOrderedItem>();

            collection.Add(item1);
            collection.Add(item2);
            collection.Add(item3);

            collection.SelectedItem = item2;

            Assert.IsTrue(item2.IsSelected);
        }

        [TestMethod]
        public void TestSelectByCollectionAlreadySelected()
        {
            MockupSelectableOrderedItem item1 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item2 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item3 = new MockupSelectableOrderedItem();

            SelectingCollection<MockupSelectableOrderedItem> collection = new SelectingCollection<MockupSelectableOrderedItem>();

            collection.Add(item1);
            collection.Add(item2);
            collection.Add(item3);

            collection.SelectedItem = item1;
            collection.SelectedItem = item2;

            Assert.IsFalse(item1.IsSelected);
            Assert.IsTrue(item2.IsSelected);
        }

        [TestMethod]
        public void TestUnselectByItem()
        {
            MockupSelectableOrderedItem item1 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item2 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item3 = new MockupSelectableOrderedItem();

            SelectingCollection<MockupSelectableOrderedItem> collection = new SelectingCollection<MockupSelectableOrderedItem>();

            collection.Add(item1);
            collection.Add(item2);
            collection.Add(item3);

            item1.IsSelected = true;
            item1.IsSelected = false;

            Assert.IsNull(collection.SelectedItem);
            Assert.IsFalse(item1.IsSelected);
        }

        [TestMethod]
        public void TestSelectByItem()
        {
            MockupSelectableOrderedItem item1 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item2 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item3 = new MockupSelectableOrderedItem();

            SelectingCollection<MockupSelectableOrderedItem> collection = new SelectingCollection<MockupSelectableOrderedItem>();

            collection.Add(item1);
            collection.Add(item2);
            collection.Add(item3);

            item1.IsSelected = true;

            Assert.AreEqual(item1, collection.SelectedItem);
        }

        [TestMethod]
        public void TestSelectByItemAlreadySelected()
        {
            MockupSelectableOrderedItem item1 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item2 = new MockupSelectableOrderedItem();
            MockupSelectableOrderedItem item3 = new MockupSelectableOrderedItem();

            SelectingCollection<MockupSelectableOrderedItem> collection = new SelectingCollection<MockupSelectableOrderedItem>();

            collection.Add(item1);
            collection.Add(item2);
            collection.Add(item3);

            item1.IsSelected = true;
            item2.IsSelected = true;

            Assert.AreEqual(item2, collection.SelectedItem);
            Assert.IsFalse(item1.IsSelected);
        }
    }
}
