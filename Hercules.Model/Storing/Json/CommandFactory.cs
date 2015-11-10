// ==========================================================================
// CommandFactory.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using GP.Windows;

namespace Hercules.Model.Storing.Json
{
    public static class CommandFactory
    {
        private const string Suffix = "Command";

        private static readonly Dictionary<string, Type> typeByName = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<Type, string> nameByType = new Dictionary<Type, string>();
        private static readonly Dictionary<Type, List<Tuple<string, string>>> legacyNameMappings = new Dictionary<Type, List<Tuple<string, string>>>();

        static CommandFactory()
        {
            AddCommand<InsertChildCommand>();

            AddCommand<ChangeColorCommand>();
            AddCommand<ChangeIconCommand>(LegacyNames.ChangeIcon);
            AddCommand<ChangeIconCommand>();
            AddCommand<ChangeTextCommand>();

            AddCommand<RemoveChildCommand>();

            AddCommand<ToggleCollapseCommand>();
            AddCommand<ToggleHullCommand>();

            AddLegacyName<ChangeIconCommand>("IconKey", "Key");
            AddLegacyName<ChangeColorCommand>("Color", "Index");
            AddLegacyName<ChangeColorCommand>("ColorValue", "Value");
        }

        private static void AddLegacyName<T>(string oldName, string newName) where T : CommandBase
        {
            List<Tuple<string, string>> mappings = legacyNameMappings.GetOrCreateDefault(typeof (T), () => new List<Tuple<string, string>>());

            mappings.Add(new Tuple<string, string>(oldName, newName));
        }

        private static void AddCommand<T>(string name = null) where T : CommandBase
        {
            Type type = typeof(T);

            name = ResolveTypeName(name, type);

            typeByName[name] = type;

            nameByType[type] = name;
        }

        private static string ResolveTypeName(string typeName, Type type)
        {
            if (typeName == null)
            {
                typeName = type.Name;

                if (typeName.EndsWith(Suffix, StringComparison.OrdinalIgnoreCase))
                {
                    typeName = typeName.Substring(0, typeName.Length - Suffix.Length);
                }
            }

            return typeName;
        }

        public static string ToTypeName(CommandBase command)
        {
            Guard.NotNull(command, nameof(command));

            return nameByType[command.GetType()];
        }

        public static CommandBase CreateCommand(string typeName, PropertiesBag properties, Document document)
        {
            Guard.NotNull(typeName, nameof(typeName));
            Guard.NotNull(document, nameof(document));
            Guard.NotNull(properties, nameof(properties));

            Type type = ResolveType(typeName);

            MapProperties(properties, type);

            CommandBase command = (CommandBase)Activator.CreateInstance(type, properties, document);

            return command;
        }

        private static void MapProperties(PropertiesBag properties, Type type)
        {
            List<Tuple<string, string>> mappings;

            if (legacyNameMappings.TryGetValue(type, out mappings))
            {
                foreach (var mapping in mappings)
                {
                    properties.Rename(mapping.Item1, mapping.Item2);
                }
            }
        }

        private static Type ResolveType(string typeName)
        {
            Type type;

            if (!typeByName.TryGetValue(typeName, out type))
            {
                type = Type.GetType(typeName);
            }

            if (type == null)
            {
                throw new IOException($"Invalid type name: '{typeName}");
            }

            return type;
        }
    }
}
