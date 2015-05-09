// ==========================================================================
// MindmapItem.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using GalaSoft.MvvmLight;
using RavenMind.Model;
using System;

namespace RavenMind.ViewModels
{
    public sealed class MindmapItem : ViewModelBase
    {
        #region Properties

        private Guid mindmapId;
        public Guid MindmapId
        {
            get
            {
                return mindmapId;
            }
            set
            {
                if (mindmapId != value)
                {
                    mindmapId = value;
                    RaisePropertyChanged("MindmapId");
                }
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        private string lastUpdate;
        public string LastUpdate
        {
            get
            {
                return lastUpdate;
            }
            set
            {
                if (lastUpdate != value)
                {
                    lastUpdate = value;
                    RaisePropertyChanged("LastUpdate");
                }
            }
        }

        #endregion
    }
}
