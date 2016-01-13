// ==========================================================================
// DocumentStoreTypesTest.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using Hercules.Model.Storing;
using Xunit;
// ReSharper disable ConvertToConstant.Local

namespace Tests.Facts
{
    public class DocumentStoreTypesTest
    {
        [Fact]
        public void DocumentRef_Constructor_ValuesPassed()
        {
            DateTimeOffset lastYear = DateTimeOffset.Now.Date.AddYears(-1);

            DocumentRef document = new DocumentRef("Name", lastYear);

            Assert.Equal("Name", document.DocumentName);
            Assert.Equal(lastYear, document.LastUpdate);
        }

        [Fact]
        public void DocumentRef_Renamed()
        {
            DocumentRef document = new DocumentRef("Name", DateTime.Now).Rename("NewName");

            Assert.Equal("NewName", document.DocumentName);
        }

        [Fact]
        public void DocumentRef_Updated_CorrectTime()
        {
            DocumentRef document = new DocumentRef("Name", DateTimeOffset.Now.Date.AddYears(-1)).Updated();

            Assert.True(document.LastUpdate >= DateTime.UtcNow.AddSeconds(-5) &&
                        document.LastUpdate <= DateTime.UtcNow.AddSeconds(10));
        }

        [Fact]
        public void DocumentRef_NullName_ThrowsException()
        {
            DocumentRef document = new DocumentRef("Name", DateTime.Now);

            Assert.Throws<ArgumentNullException>(() => document.Rename(null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void DocumentRef_EmptyNames_ThrowsException(string name)
        {
            DocumentRef document = new DocumentRef("Name", DateTime.Now);

            Assert.Throws<ArgumentException>(() => document.Rename(name));
        }

        [Fact]
        public void DocumentNotFoundException_Constructors()
        {
            string message = "Message";

            DocumentNotFoundException empty = new DocumentNotFoundException();

            Assert.NotNull(empty.Message);
            Assert.Null(empty.InnerException);

            DocumentNotFoundException withMessage = new DocumentNotFoundException(message);

            Assert.Equal(message, withMessage.Message);
            Assert.Null(withMessage.InnerException);

            DocumentNotFoundException withInner = new DocumentNotFoundException(message, withMessage);

            Assert.Equal(message, withInner.Message);
            Assert.Equal(withMessage, withInner.InnerException);
        }
    }
}
