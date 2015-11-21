// ==========================================================================
// JsonStoreTests.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Threading.Tasks;
using Windows.Storage;
using Hercules.Model;
using Hercules.Model.Storing.Json;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace UnitTests.Tests
{
    [TestClass]
    public class JsonStoreTests
    {
        [TestMethod]
        public async Task Format0_CorrectLoaded()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Files/Format0.mmd"));

            await TestFileLoading(file);
        }

        [TestMethod]
        public async Task Format1_CorrectLoaded()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Files/Format1.mmd"));

            await TestFileLoading(file);
        }

        [TestMethod]
        public async Task Format2_CorrectLoaded()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Files/Format2.mmd"));

            await TestFileLoading(file);
        }

        private static async Task TestFileLoading(StorageFile file)
        {
            Document document = await JsonDocumentSerializer.DeserializeFromFileAsync(file);

            Assert.AreEqual(2, document.Root.RightChildren.Count);

            Node rightA = document.Root.RightChildren[0];
            Node rightB = document.Root.RightChildren[1];

            Assert.AreEqual(2, rightA.Children.Count);
            Assert.AreEqual(2, rightB.Children.Count);

            Node rightA1 = rightA.Children[0];
            Node rightA2 = rightA.Children[1];
            Node rightB1 = rightB.Children[0];
            Node rightB2 = rightB.Children[1];

            Assert.AreEqual(1, document.Root.LeftChildren.Count);

            Node leftA = document.Root.LeftChildren[0];

            Assert.AreEqual(2, leftA.Children.Count);

            Node leftA1 = leftA.Children[0];
            Node leftA2 = leftA.Children[1];

            Assert.AreEqual("Test", document.Root.Text);
            Assert.AreEqual("R_A", rightA.Text);
            Assert.AreEqual("R_A1", rightA1.Text);
            Assert.AreEqual("R_A2", rightA2.Text);
            Assert.AreEqual("R_B", rightB.Text);
            Assert.AreEqual("R_B1", rightB1.Text);
            Assert.AreEqual("R_B2", rightB2.Text);
            Assert.AreEqual("L_A", leftA.Text);
            Assert.AreEqual("L_A1", leftA1.Text);
            Assert.AreEqual("L_A2", leftA2.Text);

            Assert.IsTrue(rightA.Icon is KeyIcon);
            Assert.IsTrue(rightB.Icon is KeyIcon);
        }
    }
}