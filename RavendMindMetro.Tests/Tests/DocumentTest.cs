// ==========================================================================
// DocumentTest.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RavenMind.Model;

namespace RavenMind.Tests
{
    [TestClass]
    public class DocumentTest
    {
        [TestMethod]
        public void TestSetRoot()
        {
            RootNode root = new RootNode();

            Document document = new Document();
            document.Root = root;

            Assert.AreEqual(1, document.Nodes.Count);
            Assert.AreEqual(root, document.Nodes[0]);
        }

        [TestMethod]
        public void TestSetRootWithChildren()
        {
            RootNode root = new RootNode();
            Node l1 = new Node();
            Node l2 = new Node();
            Node r1 = new Node();

            root.LeftChildren.Add(l1);
            root.LeftChildren.Add(l2);
            root.RightChildren.Add(r1);

            Document document = new Document();
            document.Root = root;

            Assert.AreEqual(4, document.Nodes.Count);
            Assert.AreEqual(root, document.Nodes[0]);
            Assert.AreEqual(l1, document.Nodes[1]);
            Assert.AreEqual(l2, document.Nodes[2]);
            Assert.AreEqual(r1, document.Nodes[3]);
        }

        [TestMethod]
        public void TestSelectRightOfSelectedNodeNoSelection()
        {
            Document document = new Document();

            Assert.AreEqual(document.Root, document.SelectRightOfSelectedNode());
            Assert.IsTrue(document.Root.IsSelected);
        }

        [TestMethod]
        public void TestSelectRightOfSelectedNodeOnlyRoot()
        {
            Document document = new Document();

            document.Root.IsSelected = true;

            Assert.AreEqual(document.Root, document.SelectRightOfSelectedNode());
            Assert.IsTrue(document.Root.IsSelected);
        }

        [TestMethod]
        public void TestRightOfSelectedRoot()
        {
            Document document = new Document();

            Node rightNode = new Node();

            document.Root.RightChildren.Add(rightNode);
            document.Root.IsSelected = true;

            Assert.AreEqual(rightNode, document.SelectRightOfSelectedNode());
            Assert.IsTrue(rightNode.IsSelected);
        }

        [TestMethod]
        public void TestRightOfSelectedRightNodeNoChild()
        {
            Document document = new Document();

            Node rightNode = new Node();

            document.Root.RightChildren.Add(rightNode);

            rightNode.IsSelected = true;

            Assert.AreEqual(rightNode, document.SelectRightOfSelectedNode());
            Assert.IsTrue(rightNode.IsSelected);
        }

        [TestMethod]
        public void TestRightOfSelectedRightNode()
        {
            Document document = new Document();

            Node rightNode11 = new Node();
            Node rightNode10 = new Node();

            rightNode10.Children.Add(rightNode11);

            document.Root.RightChildren.Add(rightNode10);

            rightNode10.IsSelected = true;

            Assert.AreEqual(rightNode11, document.SelectRightOfSelectedNode());
            Assert.IsTrue(rightNode11.IsSelected);
        }

        [TestMethod]
        public void TestRightOfSelectedLeftNode()
        {
            Document document = new Document();

            Node leftNode = new Node();

            document.Root.LeftChildren.Add(leftNode);

            leftNode.IsSelected = true;

            Assert.AreEqual(document.Root, document.SelectRightOfSelectedNode());
            Assert.IsTrue(document.Root.IsSelected);
        }

        [TestMethod]
        public void TestRightOfSelectedLeftNodeWithNonRootParent()
        {
            Document document = new Document();

            Node leftNode11 = new Node();
            Node leftNode10 = new Node();

            leftNode10.Children.Add(leftNode11);

            document.Root.LeftChildren.Add(leftNode10);

            leftNode11.IsSelected = true;

            Assert.AreEqual(leftNode10, document.SelectRightOfSelectedNode());
            Assert.IsTrue(leftNode10.IsSelected);
        }

        [TestMethod]
        public void TestSelectLeftOfSelectedNodeNoSelection()
        {
            Document document = new Document();

            Assert.AreEqual(document.Root, document.SelectLeftOfSelectedNode());
            Assert.IsTrue(document.Root.IsSelected);
        }

        [TestMethod]
        public void TestSelectLeftOfSelectedNodeOnlyRoot()
        {
            Document document = new Document();

            document.Root.IsSelected = true;

            Assert.AreEqual(document.Root, document.SelectLeftOfSelectedNode());
            Assert.IsTrue(document.Root.IsSelected);
        }

        [TestMethod]
        public void TestLeftOfSelectedRoot()
        {
            Document document = new Document();

            Node leftNode = new Node();

            document.Root.LeftChildren.Add(leftNode);
            document.Root.IsSelected = true;

            Assert.AreEqual(leftNode, document.SelectLeftOfSelectedNode());
            Assert.IsTrue(leftNode.IsSelected);
        }

        [TestMethod]
        public void TestLeftOfSelectedLeftNodeNoChild()
        {
            Document document = new Document();

            Node leftNode = new Node();

            document.Root.LeftChildren.Add(leftNode);

            leftNode.IsSelected = true;

            Assert.AreEqual(leftNode, document.SelectLeftOfSelectedNode());
            Assert.IsTrue(leftNode.IsSelected);
        }

        [TestMethod]
        public void TestLeftOfSelectedLeftNode()
        {
            Document document = new Document();

            Node leftNode11 = new Node();
            Node leftNode10 = new Node();

            leftNode10.Children.Add(leftNode11);

            document.Root.LeftChildren.Add(leftNode10);

            leftNode10.IsSelected = true;

            Assert.AreEqual(leftNode11, document.SelectLeftOfSelectedNode());
            Assert.IsTrue(leftNode11.IsSelected);
        }

        [TestMethod]
        public void TestLeftOfSelectedRightNode()
        {
            Document document = new Document();

            Node rightNode = new Node();

            document.Root.RightChildren.Add(rightNode);

            rightNode.IsSelected = true;

            Assert.AreEqual(document.Root, document.SelectLeftOfSelectedNode());
            Assert.IsTrue(document.Root.IsSelected);
        }

        [TestMethod]
        public void TestLeftOfSelectedRightNodeWithNonRootParent()
        {
            Document document = new Document();

            Node rightNode11 = new Node();
            Node rightNode10 = new Node();

            rightNode10.Children.Add(rightNode11);

            document.Root.RightChildren.Add(rightNode10);

            rightNode11.IsSelected = true;

            Assert.AreEqual(rightNode10, document.SelectLeftOfSelectedNode());
            Assert.IsTrue(rightNode10.IsSelected);
        }

        [TestMethod]
        public void TestSelectTopOfSelectedNodeNoSelection()
        {
            Document document = new Document();

            Assert.AreEqual(document.Root, document.SelectedTopOfSelectedNode());
            Assert.IsTrue(document.Root.IsSelected);
        }

        [TestMethod]
        public void TestSelectTopOfSelectedNodeOnlyRoot()
        {
            Document document = new Document();

            document.Root.IsSelected = true;

            Assert.AreEqual(document.Root, document.SelectedTopOfSelectedNode());
            Assert.IsTrue(document.Root.IsSelected);
        }

        [TestMethod]
        public void TestSelectTopOfSelectedNodeNoTop()
        {
            Document document = new Document();

            Node node1 = new Node();

            document.Root.RightChildren.Add(node1);

            node1.IsSelected = true;

            Assert.AreEqual(node1, document.SelectedTopOfSelectedNode());
            Assert.IsTrue(node1.IsSelected);
        }

        [TestMethod]
        public void TestSelectTopOfSelectedNodeSimpleTop()
        {
            Document document = new Document();

            Node node1 = new Node();
            Node node2 = new Node();

            document.Root.RightChildren.Add(node1);
            document.Root.RightChildren.Add(node2);

            node2.IsSelected = true;

            Assert.AreEqual(node1, document.SelectedTopOfSelectedNode());
            Assert.IsTrue(node1.IsSelected);
        }

        [TestMethod]
        public void TestSelectTopOfSelectedNodeComplexTop()
        {
            Document document = new Document();

            Node node10 = new Node();
            Node node11 = new Node();
            Node node12 = new Node();

            Node node20 = new Node();

            Node node30 = new Node();
            Node node31 = new Node();
            Node node32 = new Node();

            node10.Children.Add(node11);
            node10.Children.Add(node12);

            node30.Children.Add(node31);
            node30.Children.Add(node32);

            document.Root.RightChildren.Add(node10);
            document.Root.RightChildren.Add(node20);
            document.Root.RightChildren.Add(node30);

            node31.IsSelected = true;

            Assert.AreEqual(node12, document.SelectedTopOfSelectedNode());
            Assert.IsTrue(node12.IsSelected);
        }

        [TestMethod]
        public void TestSelectBottomOfSelectedNodeNoSelection()
        {
            Document document = new Document();

            Assert.AreEqual(document.Root, document.SelectedBottomOfSelectedNode());
            Assert.IsTrue(document.Root.IsSelected);
        }

        [TestMethod]
        public void TestSelectBottomOfSelectedNodeOnlyRoot()
        {
            Document document = new Document();

            document.Root.IsSelected = true;

            Assert.AreEqual(document.Root, document.SelectedBottomOfSelectedNode());
            Assert.IsTrue(document.Root.IsSelected);
        }

        [TestMethod]
        public void TestSelectBottomOfSelectedNodeNoBottom()
        {
            Document document = new Document();

            Node node1 = new Node();

            document.Root.RightChildren.Add(node1);

            node1.IsSelected = true;

            Assert.AreEqual(node1, document.SelectedBottomOfSelectedNode());
            Assert.IsTrue(node1.IsSelected);
        }

        [TestMethod]
        public void TestSelectBottomOfSelectedNodeSimpleBottom()
        {
            Document document = new Document();

            Node node1 = new Node();
            Node node2 = new Node();

            document.Root.RightChildren.Add(node1);
            document.Root.RightChildren.Add(node2);

            node1.IsSelected = true;

            Assert.AreEqual(node2, document.SelectedBottomOfSelectedNode());
            Assert.IsTrue(node2.IsSelected);
        }

        [TestMethod]
        public void TestSelectBottomOfSelectedNodeComplexBottom()
        {
            Document document = new Document();

            Node node10 = new Node();
            Node node11 = new Node();
            Node node12 = new Node();

            Node node20 = new Node();

            Node node30 = new Node();
            Node node31 = new Node();
            Node node32 = new Node();

            node10.Children.Add(node11);
            node10.Children.Add(node12);

            node30.Children.Add(node31);
            node30.Children.Add(node32);

            document.Root.RightChildren.Add(node10);
            document.Root.RightChildren.Add(node20);
            document.Root.RightChildren.Add(node30);

            node12.IsSelected = true;

            Assert.AreEqual(node31, document.SelectedBottomOfSelectedNode());
            Assert.IsTrue(node31.IsSelected);
        }
    }
}
