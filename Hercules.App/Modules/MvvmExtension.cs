// ==========================================================================
// DependentRelayCommand.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.ComponentModel;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;

namespace Hercules.App.Modules
{
    public static class MvvmExtensions
    {
        public static RelayCommand DependentOn(this RelayCommand command, INotifyPropertyChanged owner, params string[] properties)
        {
            if (owner != null && properties != null && properties.Length > 0)
            {
                HashSet<string> propertiesSet = new HashSet<string>(properties);

                owner.PropertyChanged += (sender, e) =>
                {
                    if (propertiesSet.Contains(e.PropertyName))
                    {
                        command.RaiseCanExecuteChanged();
                    }
                };
            }

            return command;
        }

        public static RelayCommand<T> DependentOn<T>(this RelayCommand<T> command, INotifyPropertyChanged owner, params string[] properties)
        {
            if (owner != null && properties != null && properties.Length > 0)
            {
                HashSet<string> propertiesSet = new HashSet<string>(properties);

                owner.PropertyChanged += (sender, e) =>
                {
                    if (propertiesSet.Contains(e.PropertyName))
                    {
                        command.RaiseCanExecuteChanged();
                    }
                };
            }

            return command;
        }
    }
}
