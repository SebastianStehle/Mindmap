// ==========================================================================
// AssetProvider.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;

namespace Hercules.App.Assets
{
    public sealed class AssetProvider
    {
        private readonly List<string> images;

        public List<string> Images
        {
            get
            {
                return images;
            }
        }

        public AssetProvider()
        {
            images = new List<string>
            {
                "/Assets/Icons/Alerts.png",
                "/Assets/Icons/Arrow_Left.png",
                "/Assets/Icons/Arrow_Up.png",
                "/Assets/Icons/Calendar_Public.png",
                "/Assets/Icons/Cancel.png",
                "/Assets/Icons/Check.png",
                "/Assets/Icons/Clock.png",
                "/Assets/Icons/Emoticon_Happy.png",
                "/Assets/Icons/Encrypt.png",
                "/Assets/Icons/Favorites.png",
                "/Assets/Icons/Fax.png",
                "/Assets/Icons/Games.png",
                "/Assets/Icons/Gift.png",
                "/Assets/Icons/Home.png",
                "/Assets/Icons/Important.png",
                "/Assets/Icons/Information.png",
                "/Assets/Icons/Note.png",
                "/Assets/Icons/Picture.png",
                "/Assets/Icons/Question.png",
                "/Assets/Icons/Status_Flag_Blue.png",
                "/Assets/Icons/Status_Flag_Green.png",
                "/Assets/Icons/Status_Flag_Red.png",
                "/Assets/Icons/Status_Flag_White.png",
                "/Assets/Icons/Status_Flag_Yellow.png",
                "/Assets/Icons/Symbol_Euro.png",
                "/Assets/Icons/Symbol_Dollar.png",
                "/Assets/Icons/Traffic_Light.png",
                "/Assets/Icons/Trash.png",
                "/Assets/Icons/User_Blue.png",
                "/Assets/Icons/User_Green.png",
                "/Assets/Icons/Video.png",
                "/Assets/Icons/Warning.png"
            };
        }
    }
}
