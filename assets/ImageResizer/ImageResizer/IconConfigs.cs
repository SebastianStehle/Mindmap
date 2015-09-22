// ==========================================================================
// IconConfigs.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Drawing;

namespace ImageResizer
{
    internal static class IconConfigs
    {
        public static readonly IconConfig[] Configs =
        {
            new IconConfig("Logo_Wide2.png", "Wide310x150Logo.scale-{scale}.png", new Size(310, 150)),
            new IconConfig("Logo_AppList2.png", "Square44x44Logo.scale-{scale}.png", new Size(44, 44)),
            new IconConfig("Logo_Small2.png", "StoreLogo.scale-{scale}.png", new Size(50, 50)),
            new IconConfig("Logo_Small2.png", "Square71x71Logo.scale-{scale}.png", new Size(71, 71)),
            new IconConfig("Logo_Medium2.png", "Square150x150Logo.scale-{scale}.png", new Size(150, 150)),
            new IconConfig("Logo_Medium2.png", "Square310x310Logo.scale-{scale}.png", new Size(310, 310)),
            new IconConfig("Logo_SplashScreen2.png", "SplashScreen.scale-{scale}.png", new Size(620, 300))
        };
    }
}
