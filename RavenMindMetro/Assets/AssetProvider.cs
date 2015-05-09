// ==========================================================================
// AssetProvider.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;

namespace RavenMind.Model
{
    public sealed class AssetProvider
    {
        public const int DefaultColor = 0x7EA7D8;

        private readonly List<string> images;
        private readonly List<int> colors;

        public List<string> Images
        {
            get
            {
                return images;
            }
        }

        public List<int> Colors
        {
            get
            {
                return colors;
            }
        }
        
        public AssetProvider()
        {
            images = new List<string> 
            {
                "Assets/Icons/Alerts.png",
                "Assets/Icons/Arrow_Left.png",
                "Assets/Icons/Arrow_Up.png",
                "Assets/Icons/Calendar_Public.png",
                "Assets/Icons/Cancel.png",
                "Assets/Icons/Check.png",
                "Assets/Icons/Clock.png",
                "Assets/Icons/Emoticon_Happy.png",
                "Assets/Icons/Encrypt.png",
                "Assets/Icons/Favorites.png",
                "Assets/Icons/Fax.png",
                "Assets/Icons/Games.png",
                "Assets/Icons/Gift.png",
                "Assets/Icons/Home.png",
                "Assets/Icons/Important.png",
                "Assets/Icons/Information.png",
                "Assets/Icons/Note.png",
                "Assets/Icons/Picture.png",
                "Assets/Icons/Question.png",
                "Assets/Icons/Status_Flag_Blue.png",
                "Assets/Icons/Status_Flag_Green.png",
                "Assets/Icons/Status_Flag_Red.png",
                "Assets/Icons/Status_Flag_White.png",
                "Assets/Icons/Status_Flag_Yellow.png",
                "Assets/Icons/Symbol_Euro.png",
                "Assets/Icons/Symbol_Dollar.png",
                "Assets/Icons/Traffic_Light.png",
                "Assets/Icons/Trash.png",
                "Assets/Icons/User_Blue.png",
                "Assets/Icons/User_Green.png",
                "Assets/Icons/Video.png",
                "Assets/Icons/Warning.png"
            };

            colors = new List<int>
            {
                0xF7977A, 
                0xF9AD81,
                0xFDC68A,
                0xFFF79A,
                0xC4DF9B,
                0xF26C4F, 
                0xF68E55,
                0xFBAF5C,
                0xFFF467,
                0xACD372,
                0xA2D39C,
                0x82CA9D,
                0x7BCDC8,
                0x6ECFF6,
                0x7EA7D8,
                0x7CC576,
                0x3BB878,
                0x1ABBB4,
                0x00BFF3,
                0x438CCA,
                0x8493CA, 
                0x8882BE,
                0xBC8DBF,
                0xF49AC2,
                0xF6989D,
                0x605CA8,
                0x855FA8,
                0xA763A8,
                0xF06EA9,
                0xF26D7D
            };
        }
    }
}
