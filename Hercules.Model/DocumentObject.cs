// ==========================================================================
// DocumentObject.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.ComponentModel;

namespace Hercules.Model
{
    public abstract class DocumentObject : INotifyPropertyChanged
    {
        private readonly Guid id;

        public event PropertyChangedEventHandler PropertyChanged;

        public Guid Id
        {
            get { return id; }
        }

        protected DocumentObject(Guid id)
        {
            this.id = id;
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            PropertyChangedEventHandler eventHandler = PropertyChanged;

            if (eventHandler != null)
            {
                eventHandler(this, eventArgs);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}
