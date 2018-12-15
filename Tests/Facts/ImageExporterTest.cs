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
using FakeItEasy;
using GP.Utils;
using Hercules.Model;
using Hercules.Model.ExImport.Formats.Image;
using Hercules.Model.Rendering;
using Xunit;

namespace Tests.Facts
{
    public sealed class ImageExporterTest
    {
        private readonly ImageExporter exporter = new ImageExporter();
        private readonly IRenderer renderer = A.Fake<IRenderer>();

        [Fact]
        public void CorrectMetadata()
        {
            Assert.Equal("Image", exporter.NameKey);

            Assert.Equal(FileExtension.PNG, exporter.Extensions.ToList()[0]);
        }

        [Fact]
        public async Task ExportAsync_WithDefaultDpi()
        {
            var stream = new MemoryStream();

            A.CallTo(() => renderer.RenderScreenshotAsync(stream, new Vector3(1, 1, 1), 300, 20))
                .Returns(Task.FromResult(300));

            await exporter.ExportAsync(new Document(Guid.NewGuid()), renderer, stream);
        }

        [Fact]
        public async Task ExportAsync_WithDpi()
        {
            var properties = 
                new PropertiesBag()
                    .Set("DPI", 500);

            var stream = new MemoryStream();

            A.CallTo(() => renderer.RenderScreenshotAsync(stream, new Vector3(1, 1, 1), 500, 20))
                .Returns(Task.FromResult(300));

            await exporter.ExportAsync(new Document(Guid.NewGuid()), renderer, stream, properties);
        }
    }
}
