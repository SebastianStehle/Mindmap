// ==========================================================================
// AssetProvider.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.Collections.Generic;

namespace RavenMind.Assets
{
    public sealed class AssetProvider
    {
        #region Constants

        public const int DefaultColor = 0x7EA7D8;

        #endregion

        #region Properties

        public List<string> Images { get; private set; }

        public List<int> Colors { get; private set; }

        #endregion

        #region Constructors

        public AssetProvider()
        {
            Images = new List<string>();
            Images.Add("Assets/Icons/Alerts.png");
            Images.Add("Assets/Icons/Arrow_Left.png");
            Images.Add("Assets/Icons/Arrow_Up.png");
            Images.Add("Assets/Icons/Calendar_Public.png");
            Images.Add("Assets/Icons/Cancel.png");
            Images.Add("Assets/Icons/Check.png");
            Images.Add("Assets/Icons/Clock.png");
            Images.Add("Assets/Icons/Emoticon_Happy.png");
            Images.Add("Assets/Icons/Encrypt.png");
            Images.Add("Assets/Icons/Favorites.png");
            Images.Add("Assets/Icons/Fax.png");
            Images.Add("Assets/Icons/Games.png");
            Images.Add("Assets/Icons/Gift.png");
            Images.Add("Assets/Icons/Home.png");
            Images.Add("Assets/Icons/Important.png");
            Images.Add("Assets/Icons/Information.png");
            Images.Add("Assets/Icons/Note.png");
            Images.Add("Assets/Icons/Picture.png");
            Images.Add("Assets/Icons/Question.png");
            Images.Add("Assets/Icons/Status_Flag_Blue.png");
            Images.Add("Assets/Icons/Status_Flag_Green.png");
            Images.Add("Assets/Icons/Status_Flag_Red.png");
            Images.Add("Assets/Icons/Status_Flag_White.png");
            Images.Add("Assets/Icons/Status_Flag_Yellow.png");
            Images.Add("Assets/Icons/Symbol_Euro.png");
            Images.Add("Assets/Icons/Symbol_Dollar.png");
            Images.Add("Assets/Icons/Traffic_Light.png");
            Images.Add("Assets/Icons/Trash.png");
            Images.Add("Assets/Icons/User_Blue.png");
            Images.Add("Assets/Icons/User_Green.png");
            Images.Add("Assets/Icons/Video.png");
            Images.Add("Assets/Icons/Warning.png");

            Colors = new List<int>();
            Colors.Add(0xF7977A); 
            Colors.Add(0xF9AD81);
            Colors.Add(0xFDC68A);
            Colors.Add(0xFFF79A);
            Colors.Add(0xC4DF9B);
            Colors.Add(0xF26C4F); 
            Colors.Add(0xF68E55);
            Colors.Add(0xFBAF5C);
            Colors.Add(0xFFF467);
            Colors.Add(0xACD372);
            Colors.Add(0xA2D39C);
            Colors.Add(0x82CA9D);
            Colors.Add(0x7BCDC8);
            Colors.Add(0x6ECFF6);
            Colors.Add(0x7EA7D8);
            Colors.Add(0x7CC576);
            Colors.Add(0x3BB878);
            Colors.Add(0x1ABBB4);
            Colors.Add(0x00BFF3);
            Colors.Add(0x438CCA);
            Colors.Add(0x8493CA); 
            Colors.Add(0x8882BE);
            Colors.Add(0xBC8DBF);
            Colors.Add(0xF49AC2);
            Colors.Add(0xF6989D);
            Colors.Add(0x605CA8);
            Colors.Add(0x855FA8);
            Colors.Add(0xA763A8);
            Colors.Add(0xF06EA9);
            Colors.Add(0xF26D7D);
        }

        #endregion
    }
}
