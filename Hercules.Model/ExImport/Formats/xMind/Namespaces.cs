// ==========================================================================
// Namespaces.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

namespace Hercules.Model.ExImport.Formats.XMind
{
    internal static class Namespaces
    {
        public static readonly string SVGNamespace = "http://www.w3.org/2000/svg";

        public static string Content(string name)
        {
            return "{urn:xmind:xmap:xmlns:content:2.0}" + name;
        }

        public static string Styles(string name)
        {
            return "{urn:xmind:xmap:xmlns:style:2.0}" + name;
        }

        public static string Manifest(string name)
        {
            return "{urn:xmind:xmap:xmlns:manifest:1.0}" + name;
        }

        public static string SVG(string name)
        {
            return "{http://www.w3.org/2000/svg}" + name;
        }
    }
}
