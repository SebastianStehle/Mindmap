// ==========================================================================
// JsonStoreTests.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using System.Threading.Tasks;
using Hercules.Model;
using Hercules.Model.Storing.Json;
using Tests.Properties;
using Xunit;

namespace Tests.Facts
{
    public class JsonStoreTests
    {
        [Fact]
        public void Format0_CorrectLoaded()
        {
            byte[] file = Resources.Format0;

            TestFileLoading(file);
        }

        [Fact]
        public void Format1_CorrectLoaded()
        {
            byte[] file = Resources.Format1;

            TestFileLoading(file);
        }

        [Fact]
        public void Format2_CorrectLoaded()
        {
            byte[] file = Resources.Format2;

            TestFileLoading(file);
        }

        private static void TestFileLoading(byte[] file)
        {
            Document document = JsonDocumentSerializer.DeserializeDocumentFromStream(new MemoryStream(file));

            Assert.Equal(2, document.Root.RightChildren.Count);

            Node rightA = document.Root.RightChildren[0];
            Node rightB = document.Root.RightChildren[1];

            Assert.Equal(2, rightA.Children.Count);
            Assert.Equal(2, rightB.Children.Count);

            Node rightA1 = rightA.Children[0];
            Node rightA2 = rightA.Children[1];
            Node rightB1 = rightB.Children[0];
            Node rightB2 = rightB.Children[1];

            Assert.Equal(1, document.Root.LeftChildren.Count);

            Node leftA = document.Root.LeftChildren[0];

            Assert.Equal(2, leftA.Children.Count);

            Node leftA1 = leftA.Children[0];
            Node leftA2 = leftA.Children[1];

            Assert.Equal("Test", document.Root.Text);
            Assert.Equal("R_A", rightA.Text);
            Assert.Equal("R_A1", rightA1.Text);
            Assert.Equal("R_A2", rightA2.Text);
            Assert.Equal("R_B", rightB.Text);
            Assert.Equal("R_B1", rightB1.Text);
            Assert.Equal("R_B2", rightB2.Text);
            Assert.Equal("L_A", leftA.Text);
            Assert.Equal("L_A1", leftA1.Text);
            Assert.Equal("L_A2", leftA2.Text);

            Assert.True(rightA.Icon is KeyIcon);
            Assert.True(rightB.Icon is KeyIcon);
        }
    }
}