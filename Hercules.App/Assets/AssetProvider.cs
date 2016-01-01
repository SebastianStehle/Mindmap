// ==========================================================================
// AssetProvider.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;
using Hercules.Model;

namespace Hercules.App.Assets
{
    public sealed class AssetProvider
    {
        private readonly List<KeyIcon> icons;

        public List<KeyIcon> Icons
        {
            get
            {
                return icons;
            }
        }

        public AssetProvider()
        {
            icons = new List<KeyIcon>
            {
                new KeyIcon("/Assets/Icons/Alerts.png"),
                new KeyIcon("/Assets/Icons/Arrow_Left.png"),
                new KeyIcon("/Assets/Icons/Arrow_Up.png"),
                new KeyIcon("/Assets/Icons/Calendar_Public.png"),
                new KeyIcon("/Assets/Icons/Cancel.png"),
                new KeyIcon("/Assets/Icons/Check.png"),
                new KeyIcon("/Assets/Icons/Clock.png"),
                new KeyIcon("/Assets/Icons/Emoticon_Happy.png"),
                new KeyIcon("/Assets/Icons/Encrypt.png"),
                new KeyIcon("/Assets/Icons/Favorites.png"),
                new KeyIcon("/Assets/Icons/Fax.png"),
                new KeyIcon("/Assets/Icons/Games.png"),
                new KeyIcon("/Assets/Icons/Gift.png"),
                new KeyIcon("/Assets/Icons/Home.png"),
                new KeyIcon("/Assets/Icons/Important.png"),
                new KeyIcon("/Assets/Icons/Information.png"),
                new KeyIcon("/Assets/Icons/Note.png"),
                new KeyIcon("/Assets/Icons/Picture.png"),
                new KeyIcon("/Assets/Icons/Question.png"),
                new KeyIcon("/Assets/Icons/Status_Flag_Blue.png"),
                new KeyIcon("/Assets/Icons/Status_Flag_Green.png"),
                new KeyIcon("/Assets/Icons/Status_Flag_Red.png"),
                new KeyIcon("/Assets/Icons/Status_Flag_White.png"),
                new KeyIcon("/Assets/Icons/Status_Flag_Yellow.png"),
                new KeyIcon("/Assets/Icons/Symbol_Euro.png"),
                new KeyIcon("/Assets/Icons/Symbol_Dollar.png"),
                new KeyIcon("/Assets/Icons/Traffic_Light.png"),
                new KeyIcon("/Assets/Icons/Trash.png"),
                new KeyIcon("/Assets/Icons/User_Blue.png"),
                new KeyIcon("/Assets/Icons/User_Green.png"),
                new KeyIcon("/Assets/Icons/Video.png"),
                new KeyIcon("/Assets/Icons/Warning.png")
            };
        }
    }
}
