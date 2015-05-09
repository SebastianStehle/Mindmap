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
        private Guid mindmapId;
        private string name;
        private string lastUpdate;

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
    }
}
