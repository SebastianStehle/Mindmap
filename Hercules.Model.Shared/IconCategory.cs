// ==========================================================================
// IconCategory.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using System.Linq;
using GP.Utils;

namespace Hercules.Model
{
    public sealed class IconCategory
    {
        private readonly List<INodeIcon> icons = new List<INodeIcon>();
        private readonly string name;

        public IReadOnlyList<INodeIcon> Icons
        {
            get { return icons; }
        }

        public string Name
        {
            get { return name; }
        }

        public IconCategory(string name, params string[] icons)
        {
            Guard.NotNullOrEmpty(name, nameof(name));
            Guard.NotNull(icons, nameof(icons));

            this.name = name;

            this.icons.AddRange(icons.Select(x => new KeyIcon(x)));
        }
    }
}
