// ==========================================================================
// JsonStoreTests.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using FakeItEasy;
using GP.Utils;
using Hercules.Model;
using Hercules.Model.ExImport.Formats.XMind;
using Hercules.Model.Rendering;
using Hercules.Model.Storing;
using Hercules.Model.Storing.Json;
using Tests.Given;
using Tests.Properties;
using Xunit;

namespace Tests.Facts
{
    public class JsonStoreTests : GivenLocalization
    {
        private readonly IRenderer renderer = A.Fake<IRenderer>();
        private readonly IRenderColor color = A.Fake<IRenderColor>();

        public JsonStoreTests()
        {
            LocalizationManager.Provider = new NoopLocalizationProvider();
        }

        [Fact]
        public void FileExtension()
        {
            Assert.Equal(".mmd", Constants.FileExtension.Extension);
        }

        [Fact]
        public void Format0_CorrectLoaded()
        {
            var file = Resources.Format0;

            TestFileLoading(file);
        }

        [Fact]
        public void Format1_CorrectLoaded()
        {
            var file = Resources.Format1;

            TestFileLoading(file);
        }

        [Fact]
        public void Format2_CorrectLoaded()
        {
            var file = Resources.Format2;

            TestFileLoading(file);
        }

        [Fact]
        public void Format2_LoadAndWrite()
        {
            var file = Resources.Format2;

            var document = JsonDocumentSerializer.Deserialize(file);

            JsonDocumentSerializer.Serialize(new JsonHistory(document));
        }

        [Fact]
        public async Task Format2_LoadAndWrite_XMind()
        {
            A.CallTo(() => renderer.FindColor(A<NodeBase>.Ignored))
                .Returns(color);

            A.CallTo(() => color.Normal)
                .Returns(new Vector3(1, 0, 0));

            var file = Resources.Format2;

            var document = JsonDocumentSerializer.Deserialize(file);

            var exporter = new XMindExporter();
            var importer = new XMindImporter();

            var stream = new MemoryStream();

            await exporter.ExportAsync(document, renderer, stream);

            stream = new MemoryStream(stream.ToArray());

            var imported = await importer.ImportAsync(stream, "Name");

            Assert.Single(imported);
            Assert.Equal("Test", imported[0].Name);

            document = imported[0].Document;

            AssertDocument(document, false);
        }

        private static void TestFileLoading(byte[] file)
        {
            var document = JsonDocumentSerializer.Deserialize(file);

            AssertDocument(document);
        }

        private static void AssertDocument(Document document, bool checkIcons = true)
        {
            Assert.Equal(2, document.Root.RightChildren.Count);

            var allChildren = document.Root.RightChildren.Union(document.Root.LeftChildren).ToList();

            var rightA = allChildren.Single(x => x.Text == "R_A");
            var rightB = allChildren.Single(x => x.Text == "R_B");

            Assert.Equal(2, rightA.Children.Count);
            Assert.Equal(2, rightB.Children.Count);

            var rightA1 = rightA.Children[0];
            var rightA2 = rightA.Children[1];
            var rightB1 = rightB.Children[0];
            var rightB2 = rightB.Children[1];

            Assert.Equal(1, document.Root.LeftChildren.Count);

            var leftA = allChildren.Single(x => x.Text == "L_A");

            Assert.Equal(2, leftA.Children.Count);

            var leftA1 = leftA.Children[0];
            var leftA2 = leftA.Children[1];

            Assert.Equal("Test", document.Root.Text);

            Assert.Equal("R_A1", rightA1.Text);
            Assert.Equal("R_A2", rightA2.Text);

            Assert.Equal("R_B1", rightB1.Text);
            Assert.Equal("R_B2", rightB2.Text);

            Assert.Equal("L_A1", leftA1.Text);
            Assert.Equal("L_A2", leftA2.Text);

            if (checkIcons)
            {
                Assert.True(rightA.Icon is KeyIcon);
                Assert.True(rightB.Icon is KeyIcon);
            }
        }
    }
}