// ==========================================================================
// DocumentObject.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.ComponentModel;

namespace Hercules.Model
{
    public abstract class DocumentObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected DocumentObject()
        {
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
