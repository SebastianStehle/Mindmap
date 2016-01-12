// ==========================================================================
// ExtensionsTest.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using GP.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

// ReSharper disable InvokeAsExtensionMethod

namespace Tests.Facts
{
    public class ExtensionsTest
    {
        [Fact]
        public void Forget_DoesNothing()
        {
            Extensions.Forget(new Task(() => { }));
        }

        [Fact]
        public void AddAndReturn_Added()
        {
            IList<int> list = new List<int> { 1, 5, 7, 9 };

            Assert.Equal(11, list.AddAndReturn(11));
            Assert.Equal(11, list.Last());
        }

        [Fact]
        public void WriteFromStream()
        {
            byte[] buffer = { 1, 5, 6, 7 };

            MemoryStream source = new MemoryStream(buffer);
            MemoryStream target = new MemoryStream();

            Extensions.Write(target, source);

            Assert.Equal(buffer, target.ToArray());
        }

        [Fact]
        public async Task ToMemoryStreamAsync()
        {
            byte[] buffer = { 1, 5, 6, 7 };

            MemoryStream result = await Extensions.ToMemoryStreamAsync(new MemoryStream(buffer));

            Assert.Equal(buffer, result.ToArray());
        }

        [Fact]
        public void IsBase64Encoded_Valid_ReturnsTrue()
        {
            Assert.True(Extensions.IsBase64Encoded(Convert.ToBase64String(new byte[4])));
        }

        [Fact]
        public void IsBase64Encoded_NotValid_ReturnsFalse()
        {
            Assert.False(Extensions.IsBase64Encoded("%INVALID%"));
        }

        [Fact]
        public void GetOrCreateDefault_Found_ReturnsValue()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int> { { 1, 5 } };

            Assert.Equal(5, dictionary.GetOrCreateDefault(1));
        }

        [Fact]
        public void GetOrCreateDefault_NotFound_ReturnsDefault()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();

            Assert.Equal(0, dictionary.GetOrCreateDefault(1));
        }

        [Fact]
        public void IndexOf_ReturnsIndex()
        {
            IReadOnlyList<int> list = new List<int> { 1, 5, 7, 9 };

            Assert.Equal(2, list.IndexOf(7));
        }

        [Fact]
        public void IndexOf_NotFound_ReturnsMinusOne()
        {
            IReadOnlyList<int> list = new List<int> { 1, 5, 7, 9 };

            Assert.Equal(-1, list.IndexOf(17));
        }

        [Fact]
        public void AddRange_Added()
        {
            var itemsToAdd = new[] { 1, 3, 5 };

            ObservableCollection<int> list = new ObservableCollection<int>();

            list.AddRange(itemsToAdd);

            Assert.Equal(itemsToAdd, list);
        }

        [Fact]
        public void Foreach_Called()
        {
            var items = new[] { 1, 3, 5 };

            List<int> itemsHandled = new List<int>();

            items.Foreach(x => itemsHandled.Add(x));

            Assert.Equal(itemsHandled, items);
        }

        [Fact]
        public void IsBetween_DifferentCases()
        {
            Assert.True(Extensions.IsBetween("B", "A", "C"));
            Assert.True(Extensions.IsBetween(2, 1, 3));

            Assert.False(Extensions.IsBetween("A", "B", "C"));
            Assert.False(Extensions.IsBetween(1, 2, 3));
        }

        [Fact]
        public void SeparateByUpperLetters_Null_ReturnsNull()
        {
            Assert.Null(Extensions.SeparateByUpperLetters(null));
        }

        [Fact]
        public void SeparateByUpperLetters_Empty_ReturnsEmpty()
        {
            Assert.Equal(string.Empty, Extensions.SeparateByUpperLetters(string.Empty));
        }

        [Fact]
        public void SeparateByUpperLetters_SimpleStrings()
        {
            Assert.Equal("Hello", Extensions.SeparateByUpperLetters("Hello"));
            Assert.Equal("Hello World", Extensions.SeparateByUpperLetters("HelloWorld"));
            Assert.Equal("Hello My World", Extensions.SeparateByUpperLetters("HelloMyWorld"));
        }

        [Fact]
        public void SeparateByUpperLetters_ComplexStrings()
        {
            Assert.Equal("GPS", Extensions.SeparateByUpperLetters("GPS"));
            Assert.Equal("GPS Solution", Extensions.SeparateByUpperLetters("GPSSolution"));
            Assert.Equal("GPS Solution V2", Extensions.SeparateByUpperLetters("GPSSolutionV2"));
            Assert.Equal("GPS Solution EXT", Extensions.SeparateByUpperLetters("GPSSolutionEXT"));
        }
    }
}
