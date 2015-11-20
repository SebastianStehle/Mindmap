using System;
using System.Threading.Tasks;
using Windows.Storage;
using Hercules.Model.Storing.Json;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace UnitTests.Tests
{
    [TestClass]
    public class JsonStoreTests
    {
        [TestMethod]
        public async Task Format1_CorrectLoaded()
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Files/Format0.mmd"));

            var document = await JsonDocumentSerializer.DeserializeFromFileAsync(file);

            Assert.AreEqual("Test", document.Root.Text);

            Assert.AreEqual(2, document.Root.RightChildren.Count);

            Assert.AreEqual("RightA", document.Root.RightChildren[0]);
            Assert.AreEqual("RightA1", document.Root.RightChildren[0].Children[0]);
            Assert.AreEqual("RightA2", document.Root.RightChildren[0].Children[1]);

            Assert.AreEqual("RightB", document.Root.RightChildren[1]);
            Assert.AreEqual("RightB1", document.Root.RightChildren[1].Children[0]);
            Assert.AreEqual("RightB2", document.Root.RightChildren[1].Children[1]);

            Assert.AreEqual(1, document.Root.RightChildren.Count);

            Assert.AreEqual("LeftA", document.Root.LeftChildren[0]);
            Assert.AreEqual("LeftA1", document.Root.LeftChildren[0].Children[0]);
            Assert.AreEqual("LeftA2", document.Root.LeftChildren[0].Children[1]);
        }
    }
}