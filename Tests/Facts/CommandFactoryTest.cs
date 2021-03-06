﻿// ==========================================================================
// CommandFactoryTest.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using GP.Utils;
using Hercules.Model;
using Hercules.Model.Storing;
using Xunit;
// ReSharper disable ConvertToConstant.Local
// ReSharper disable PossibleNullReferenceException

namespace Tests.Facts
{
    public class CommandFactoryTest
    {
        private readonly Document document = new Document(Guid.NewGuid());
        private readonly PropertiesBag properties = new PropertiesBag();

        public CommandFactoryTest()
        {
            properties.Set("NodeId", document.Root.Id);
        }

        [Fact]
        public void InvalidTypeName_ThrowsException()
        {
            Assert.Throws<IOException>(() => CommandFactory.CreateCommand("INVALID", properties, document));
        }

        [Fact]
        public void ByPrettyName_ReturnsCommand()
        {
            var typeName = "ToggleHull";

            var command = CommandFactory.CreateCommand(typeName, properties, document);

            Assert.IsType<ToggleHullCommand>(command);
        }

        [Fact]
        public void ByTypeName_ReturnsCommand()
        {
            var typeName = typeof(ToggleHullCommand).AssemblyQualifiedName;

            var command = CommandFactory.CreateCommand(typeName, properties, document);

            Assert.IsType<ToggleHullCommand>(command);
        }

        [Fact]
        public void ByOldTypeName_ReturnsCommand()
        {
            var typeName = typeof(ToggleHullCommand).AssemblyQualifiedName.Replace("Hercules.Model.Shared,", "Hercules.Model,");

            var command = CommandFactory.CreateCommand(typeName, properties, document);

            Assert.IsType<ToggleHullCommand>(command);
        }
    }
}
