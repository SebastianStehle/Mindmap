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
using System.Reflection;

namespace Hercules.Model.Storing
{
    internal static class CommandFactory
    {
        private const string Suffix = "Command";
        private static readonly Dictionary<string, Type> TypesByName = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<Type, string> NamesByType = new Dictionary<Type, string>();
        private static readonly MultiValueDictionary<Type, LegacyPropertyAttribute> LegacyProperties = new MultiValueDictionary<Type, LegacyPropertyAttribute>();

        static CommandFactory()
        {
            Type commandBaseType = typeof(CommandBase);

            Assembly assembly = typeof(CommandFactory).GetTypeInfo().Assembly;

            foreach (Type type in assembly.GetTypes())
            {
                TypeInfo typeInfo = type.GetTypeInfo();

                if (typeInfo.IsSubclassOf(commandBaseType))
                {
                    string typeName = ResolveTypeName(type);

                    AddCommand(type, typeName);
                }

                LegacyNameAttribute legacyName = typeInfo.GetCustomAttribute<LegacyNameAttribute>();

                if (!string.IsNullOrWhiteSpace(legacyName?.OldName))
                {
                    AddCommand(type, legacyName.OldName);
                }

                IEnumerable<LegacyPropertyAttribute> legacyProperties = typeInfo.GetCustomAttributes<LegacyPropertyAttribute>();

                foreach (LegacyPropertyAttribute legacyProperty in legacyProperties)
                {
                    if (!string.IsNullOrWhiteSpace(legacyProperty.OldName) && !string.IsNullOrWhiteSpace(legacyProperty.NewName))
                    {
                        LegacyProperties.Add(typeInfo.AsType(), legacyProperty);
                    }
                }
            }
        }

        public static string ToTypeName(CommandBase command)
        {
            return NamesByType[command.GetType()];
        }

        private static void AddCommand(Type type, string name)
        {
            TypesByName[name] = type;

            if (!NamesByType.ContainsKey(type))
            {
                NamesByType[type] = name;
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
            IReadOnlyCollection<LegacyPropertyAttribute> mappings;

            if (LegacyProperties.TryGetValue(type, out mappings))
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

            if (!TypesByName.TryGetValue(typeName, out type))
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
