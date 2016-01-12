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
using GP.Utils;
using Hercules.Model;
using Hercules.Model.ExImport.Formats.XMind;
using Hercules.Model.Rendering;
using Hercules.Model.Storing.Json;
using Rhino.Mocks;
using Tests.Given;
using Tests.Properties;
using Xunit;

namespace Tests.Facts
{
    public class JsonStoreTests : GivenMocks
    {
        private readonly IRenderer renderer;
        private readonly IRenderColor color;

        public JsonStoreTests()
        {
            LocalizationManager.Provider = new NoopLocalizationProvider();

            renderer = Mocks.StrictMock<IRenderer>();

            color = Mocks.StrictMock<IRenderColor>();
        }

        [Fact]
        public void FileExtension()
        {
            Assert.Equal(".mmd", JsonDocumentSerializer.FileExtension.Extension);
        }

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

        [Fact]
        public void Format2_LoadAndWrite()
        {
            byte[] file = Resources.Format2;

            Document document = JsonDocumentSerializer.DeserializeDocumentFromStream(new MemoryStream(file));

            JsonDocumentSerializer.SerializeToStream(new MemoryStream(), document);
        }

        [Fact]
        public async Task Format2_LoadAndWrite_XMind()
        {
            using (Record())
            {
                renderer.Expect(x => x.FindColor(null)).Repeat.Any().IgnoreArguments().Return(color);

                color.Expect(x => x.Normal).Repeat.Any().Return(new Vector3(1, 0, 0));
            }

            using (Playback())
            {
                byte[] file = Resources.Format2;

                Document document = JsonDocumentSerializer.DeserializeDocumentFromStream(new MemoryStream(file));

                XMindExporter exporter = new XMindExporter();
                XMindImporter importer = new XMindImporter();

                MemoryStream stream = new MemoryStream();

                await exporter.ExportAsync(document, renderer, stream);

                stream = new MemoryStream(stream.ToArray());

                var imported = await importer.ImportAsync(stream, "Name");

                Assert.Equal(1, imported.Count);
                Assert.Equal("Test", imported[0].Name);

                document = imported[0].Document;

                AssertDocument(document, false);
            }
        }

        private static void TestFileLoading(byte[] file)
        {
            Document document = JsonDocumentSerializer.DeserializeDocumentFromStream(new MemoryStream(file));

            AssertDocument(document);
        }

        private static void AssertDocument(Document document, bool checkIcons = true)
        {
            Assert.Equal(2, document.Root.RightChildren.Count);

            var allChildren = document.Root.RightChildren.Union(document.Root.LeftChildren).ToList();

            Node rightA = allChildren.Single(x => x.Text == "R_A");
            Node rightB = allChildren.Single(x => x.Text == "R_B");

            Assert.Equal(2, rightA.Children.Count);
            Assert.Equal(2, rightB.Children.Count);

            Node rightA1 = rightA.Children[0];
            Node rightA2 = rightA.Children[1];
            Node rightB1 = rightB.Children[0];
            Node rightB2 = rightB.Children[1];

            Assert.Equal(1, document.Root.LeftChildren.Count);

            Node leftA = allChildren.Single(x => x.Text == "L_A");

            Assert.Equal(2, leftA.Children.Count);

            Node leftA1 = leftA.Children[0];
            Node leftA2 = leftA.Children[1];

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