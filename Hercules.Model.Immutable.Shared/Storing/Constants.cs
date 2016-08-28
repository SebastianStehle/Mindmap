// ==========================================================================
// Constants.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GP.Utils;

namespace Hercules.Model.Storing
{
    public static class Constants
    {
        public static FileExtension FileExtension
        {
            get
            {
                return new FileExtension(".mmd", "application/json");
            }
        }
    }
}
