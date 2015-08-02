// ==========================================================================
// ExtensionsTest.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RavenMind.Mockups;
using SE.Metro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace RavenMind.Tests
{
    [TestClass]
    public class ExtensionsTest
    {
        [TestMethod]
        public void TestWrite()
        {
            MemoryStream target = new MemoryStream();
            MemoryStream source = new MemoryStream();

            source.WriteByte(1);
            source.WriteByte(2);
            source.WriteByte(3);
            source.WriteByte(4);
            source.Position = 0;

            target.Write(source);

            Assert.AreEqual(4, target.Length);

            byte[] buffer = target.ToArray();

            Assert.AreEqual((byte)1, buffer[0]);
            Assert.AreEqual((byte)2, buffer[1]);
            Assert.AreEqual((byte)3, buffer[2]);
            Assert.AreEqual((byte)4, buffer[3]);
        }

        [TestMethod]
        public void TestWriteNullTargetException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Extensions.Write(null, new MemoryStream()));
        }
        
        [TestMethod]
        public void TestWriteNullSourceException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Extensions.Write(new MemoryStream(), null));
        }

        [TestMethod]
        public void TestGetOrCreateDefaultWithDictionary()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();

            int r1 = dictionary.GetOrCreateDefault(1);
            int r2 = dictionary.GetOrCreateDefault(1);

            Assert.AreEqual(0, r1);
            Assert.AreEqual(0, r2);
            Assert.AreEqual(1, dictionary.Count);
        } 

        [TestMethod]
        public void TestGetOrCreateDefaultWithDictionaryNullFunctionException()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();

            Assert.ThrowsException<ArgumentNullException>(() => Extensions.GetOrCreateDefault<int, int>(dictionary, 1, null));
        }

        [TestMethod]
        public void TestGetOrCreateDefaultWithDictionaryNullCollectionException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Extensions.GetOrCreateDefault<int, int>((Dictionary<int, int>)null, 1));
        }

        [TestMethod]
        public void TestAddRange()
        {
            Collection<int> target = new Collection<int>();
            Collection<int> source = new Collection<int>();

            source.Add(123);
            source.Add(456);
            source.Add(789);

            target.AddRange(source);

            Assert.AreEqual(3, target.Count);
            Assert.AreEqual(123, target[0]);
            Assert.AreEqual(456, target[1]);
            Assert.AreEqual(789, target[2]);
        }

        [TestMethod]
        public void TestAddRangeNullSourceExeption()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Extensions.AddRange(new Collection<int>(), null));
        }

        [TestMethod]
        public void TestAddRangeNullTargetExeption()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Extensions.AddRange(null, new Collection<int>()));
        }

        [TestMethod]
        public void TestForeach()
        {
            Collection<int> source = new Collection<int>();

            source.Add(123);
            source.Add(456);
            source.Add(789);

            int i = 0;

            source.Foreach(x =>
            {
                i++;

                switch (i)
                {
                case 1:
                    Assert.AreEqual(123, x);
                    break;
                case 2:
                    Assert.AreEqual(456, x);
                    break;
                case 3:
                    Assert.AreEqual(789, x);
                    break;
                }
            });
        }

        [TestMethod]
        public void TestForeachNullCollectionException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Extensions.Foreach<int>(null, x => { }));
        }

        [TestMethod]
        public void TestForeachNullActionException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Extensions.Foreach<int>(new List<int>(), null));
        }
    }
}
