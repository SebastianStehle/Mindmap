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

namespace Hercules.Model.Storing.Json
{
    internal static class CommandFactory
    {
        private const string Suffix = "Command";

        private static readonly Dictionary<string, Type> typeByName = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<Type, string> nameByType = new Dictionary<Type, string>();
        private static readonly MultiValueDictionary<Type, LegacyName> legacyNameMappings = new MultiValueDictionary<Type, LegacyName>();

        static CommandFactory()
        {
            AddCommand<InsertChildCommand>();

            AddCommand<ChangeColorCommand>();
            AddCommand<ChangeIconCommand>();
            AddCommand<ChangeIconCommand>(LegacyNames.ChangeIcon);
            AddCommand<ChangeTextCommand>();

            AddCommand<RemoveChildCommand>();

            AddCommand<ToggleCollapseCommand>();
            AddCommand<ToggleHullCommand>();

            AddLegacyName<ChangeIconCommand>("IconKey", "Key");
            AddLegacyName<ChangeColorCommand>("Color", "Index");
            AddLegacyName<ChangeColorCommand>("ColorValue", "Value");
        }

        public static string ToTypeName(CommandBase command)
        {
            return nameByType[command.GetType()];
        }

        private static void AddLegacyName<T>(string oldName, string newName) where T : CommandBase
        {
            legacyNameMappings.Add(typeof (T), new LegacyName { OldName = oldName, NewName = newName });
        }

        private static void AddCommand<T>(string name = null, bool isDefaultName = false) where T : CommandBase
        {
            AddCommand(typeof(T), name, isDefaultName);
        }

        private static void AddCommand(Type type, string name = null, bool isDefaultName = false)
        {
            name = name ?? ResolveTypeName(type);

            typeByName[name] = type;

            if (!nameByType.ContainsKey(type) || isDefaultName)
            {
                nameByType[type] = name;
            }
        }

        private static string ResolveTypeName(Type type)
        {
            string result = type.Name;

            if (result.EndsWith(Suffix, StringComparison.OrdinalIgnoreCase))
            {
                result = result.Substring(0, result.Length - Suffix.Length);
            }

            return result;
        }

        public static CommandBase CreateCommand(string typeName, PropertiesBag properties, Document document)
        {
            Type type = ResolveType(typeName);

            MapProperties(properties, type);

            return CreateCommand(properties, document, type);
        }

        private static CommandBase CreateCommand(PropertiesBag properties, Document document, Type type)
        {
            return (CommandBase)Activator.CreateInstance(type, properties, document);
        }

        private static void MapProperties(PropertiesBag properties, Type type)
        {
            IReadOnlyCollection<LegacyName> mappings;

            if (legacyNameMappings.TryGetValue(type, out mappings))
            {
                foreach (var mapping in mappings)
                {
                    properties.Rename(mapping.OldName, mapping.NewName);
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
