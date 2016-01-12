// ==========================================================================
// ImageExporterTest.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using GP.Utils;
using Hercules.Model;
using Hercules.Model.ExImport.Formats.Image;
using Hercules.Model.Rendering;
using Rhino.Mocks;
using Tests.Given;
using Xunit;

namespace Tests.Facts
{
    public sealed class ImageExporterTest : GivenMocks
    {
        private readonly ImageExporter exporter = new ImageExporter();
        private readonly IRenderer renderer;

        public ImageExporterTest()
        {
            renderer = Mocks.StrictMock<IRenderer>();
        }

        [Fact]
        public void CorrectMetadata()
        {
            Assert.Equal("Image", exporter.NameKey);

            Assert.Equal(FileExtension.PNG, exporter.Extensions.ToList()[0]);
        }

        [Fact]
        public async Task ExportAsync_WithDefaultDpi()
        {
            MemoryStream stream = new MemoryStream();

            using (Mocks.Record())
            {
                renderer.Expect(x => x.RenderScreenshotAsync(stream, new Vector3(1, 1, 1), 300)).Return(Task.FromResult(300));
            }

            using (Mocks.Playback())
            {
                await exporter.ExportAsync(new Document(Guid.NewGuid()), renderer, stream);
            }
        }

        [Fact]
        public async Task ExportAsync_WithDpi()
        {
            PropertiesBag properties = new PropertiesBag();
            properties.Set("DPI", 500);

            MemoryStream stream = new MemoryStream();

            using (Mocks.Record())
            {
                renderer.Expect(x => x.RenderScreenshotAsync(stream, new Vector3(1, 1, 1), 500)).Return(Task.FromResult(300));
            }

            using (Mocks.Playback())
            {
                await exporter.ExportAsync(new Document(Guid.NewGuid()), renderer, stream, properties);
            }
        }
    }
}
