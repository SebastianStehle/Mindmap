// ==========================================================================
// SerializationTest.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RavenMind.Model;

namespace RavenMind.Tests
{
    [TestClass]
    public class SerializationTest
    {
        [TestMethod]
        public void TestSerialization()
        {
            Guid id1 = Guid.NewGuid();
            Guid id2 = Guid.NewGuid();
            Guid id3 = Guid.NewGuid();
               
            MemoryStream memoryStream = new MemoryStream();

            XmlSerializer serializer = new XmlSerializer(typeof(Document));

            Document document = new Document();

            document.Root.LeftChildren.Add(new Node { Id = id1 });
            document.Root.LeftChildren.Add(new Node { Id = id2 });
            document.Root.RightChildren.Add(new Node { Id = id3 });

            serializer.Serialize(memoryStream, document);

            memoryStream.Position = 0;

            Document document2 = (Document)serializer.Deserialize(memoryStream);

            Assert.IsNotNull(document2);
            Assert.IsNotNull(document2.Root);
            Assert.AreEqual(2, document2.Root.LeftChildren.Count);
            Assert.AreEqual(1, document2.Root.RightChildren.Count);
            Assert.AreEqual(id1, document2.Root.LeftChildren[0].Id);
            Assert.AreEqual(id2, document2.Root.LeftChildren[1].Id);
            Assert.AreEqual(id3, document2.Root.RightChildren[0].Id);
       }
    }
}
