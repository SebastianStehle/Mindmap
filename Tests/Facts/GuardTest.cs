// ==========================================================================
// GuardTest.cs
// Green Parrot Framework
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.ComponentModel.DataAnnotations;
using GP.Utils;
using Tests.Mockups;
using Xunit;

namespace Tests.Facts
{
    public class GuardTest
    {
        [Fact]
        public void Between_ValidTarget()
        {
            Guard.Between(25, 20, 30, "Parameter");
        }

        [Fact]
        public void Between_GreaterThan_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Guard.Between(40, 20, 30, "Parameter"));
        }

        [Fact]
        public void Between_LessThan_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Guard.Between(10, 20, 30, "Parameter"));
        }

        [Fact]
        public void GreaterThan_ValidTarget()
        {
            Guard.GreaterThan(30, 20, "Parameter");
        }

        [Fact]
        public void GreaterThan_LessThan_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Guard.GreaterThan(10, 20, "Parameter"));
        }

        [Fact]
        public void GreaterEquals_ValidTarget()
        {
            Guard.GreaterEquals(30, 20, "Parameter");
            Guard.GreaterEquals(20, 20, "Parameter");
        }

        [Fact]
        public void GreaterEquals_LessThan_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Guard.GreaterEquals(10, 20, "Parameter"));
        }

        [Fact]
        public void LessThan_ValidTarget()
        {
            Guard.LessThan(10, 20, "Parameter");
        }

        [Fact]
        public void LessThan_GreaterThan_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Guard.LessThan(30, 20, "Parameter"));
        }

        [Fact]
        public void LessEquals()
        {
            Guard.LessEquals(10, 20, "Parameter");
            Guard.LessEquals(20, 20, "Parameter");
        }

        [Fact]
        public void LessEquals_GreaterThan_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Guard.LessEquals(30, 20, "Parameter"));
        }

        [Fact]
        public void NotEmpty_ValidGuid()
        {
            Guard.NotEmpty(Guid.NewGuid(), "Parameter");
        }

        [Fact]
        public void NotEmpty_EmptyGuid_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Guard.NotEmpty(Guid.Empty, "Parameter"));
        }

        [Fact]
        public void NotEmpty_ValidTarget()
        {
            Guard.NotEmpty(new[] { 1, 2, 3 }, "Parameter");
        }

        [Fact]
        public void NotEmpty_EmptyArray_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Guard.NotEmpty(new int[0], "Parameter"));
        }

        [Fact]
        public void NotEmpty_NullArray_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => Guard.NotEmpty((int[])null, "Parameter"));
        }

        [Fact]
        public void NotNull_ValidTarget()
        {
            Guard.NotNull(string.Empty, "Parameter");
        }

        [Fact]
        public void NotNull_Null_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => Guard.NotNull(null, "Parameter"));
        }

        [Fact]
        public void NotNullOrEmpty_ValidTarget()
        {
            Guard.NotNullOrEmpty("Value", "Parameter");
        }

        [Fact]
        public void NotNullOrEmpty_Null_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => Guard.NotNullOrEmpty(null, "Parameter"));
        }

        [Fact]
        public void NotNullOrEmpty_Empty_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Guard.NotNullOrEmpty(string.Empty, "Parameter"));
        }

        [Fact]
        public void NotNullOrEmpty_Whitespace_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Guard.NotNullOrEmpty(" ", "Parameter"));
        }

        [Fact]
        public void IsType_ValidTarget()
        {
            Guard.IsType<bool>(true, "Parameter");
        }

        [Fact]
        public void IsType_OtherType_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Guard.IsType<bool>(123, "Parameter"));
        }

        [Fact]
        public void IsTypeNonGeneric_ValidTarget()
        {
            Guard.IsType(true, typeof(bool), "Parameter");
        }

        [Fact]
        public void IsTypeNonGeneric_OtherType_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Guard.IsType(123, typeof(bool), "Parameter"));
        }

        [Fact]
        public void ValidFileName_ValidTarget()
        {
            Guard.ValidFileName("FileName", "Parameter");
        }

        [Fact]
        public void ValidFileName_InvalidName_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Guard.ValidFileName("File/Name", "Parameter"));
        }

        [Fact]
        public void ValidModel_ValidTarget()
        {
            Guard.ValidModel(new MockupModel { Name  = "Name" }, "Parameter");
        }

        [Fact]
        public void ValidModel_InvalidModel_ValidTarget()
        {
            Assert.Throws<ValidationException>(() => Guard.ValidModel(new MockupModel(), "Parameter"));
        }

        [Fact]
        public void ValidModel_NullModel_ValidTarget()
        {
            Assert.Throws<ArgumentNullException>(() => Guard.ValidModel(null, "Parameter"));
        }
    }
}
