// ==========================================================================
// DocumentFileModel.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using GalaSoft.MvvmLight;
using GP.Utils;
using GP.Utils.Mvvm;
using Hercules.Model;
using Hercules.Model.Storing;
using Microsoft.HockeyApp;

// ReSharper disable RedundantIfElseBlock
// ReSharper disable InvertIf
// ReSharper disable ConvertIfStatementToReturnStatement

namespace Hercules.App.Components.Implementations
{
    public sealed class DocumentFileModel : ViewModelBase, IDocumentFileModel
    {
        private readonly IDialogService dialogService;
        private readonly DocumentFile documentFile;

        public string DisplayPath
        {
            get
            {
                if (documentFile.IsInLocalFolder)
                {
                    return string.Concat(LocalizationManager.GetString("Paths_AppFolder"), "\\", Name);
                }
                else
                {
                    return Path;
                }
            }
        }

        public string Name
        {
            get { return documentFile.Name; }
        }

        public string Path
        {
            get { return documentFile.Path; }
        }

        public string ModifiedLocal
        {
            get { return documentFile.ModifiedUtc.ToLocalTime().ToString("g", CultureInfo.CurrentCulture); }
        }

        public bool HasChanges
        {
            get { return documentFile.HasChanges; }
        }

        public Document Document
        {
            get { return documentFile.Document; }
        }

        public DocumentFile File
        {
            get { return documentFile; }
        }

        public DocumentFileModel(DocumentFile documentFile, IDialogService dialogService)
        {
            this.documentFile = documentFile;
            this.dialogService = dialogService;

            documentFile.Changed += DocumentFile_Changed;
        }

        public override void Cleanup()
        {
            documentFile.Changed -= DocumentFile_Changed;
        }

        private void DocumentFile_Changed(object sender, EventArgs e)
        {
            RaisePropertiesChanged();
        }

        public void Close()
        {
            try
            {
                documentFile.Close();
            }
            finally
            {
                RaisePropertiesChanged();
            }
        }

        public async Task<bool> SaveAsAsync(bool hideDialogs = false)
        {
            var file = await PickSaveAsync(DocumentFile.Extension);

            return await SaveInternalAsync(hideDialogs, () => documentFile.SaveAsAsync(file));
        }

        public Task<bool> SaveSilentAsync()
        {
            return SaveInternalAsync(true, () => documentFile.SaveAsync());
        }

        public Task<bool> SaveAsync()
        {
            return documentFile.File == null ? SaveAsAsync() : SaveInternalAsync(false, () => documentFile.SaveAsync());
        }

        private Task<bool> SaveInternalAsync(bool hideDialogs, Func<Task<bool>> save)
        {
            return DoAsync("Saving", save, hideDialogs);
        }

        public Task<bool> OpenAsync(bool hideDialogs = false)
        {
            return DoAsync("Opening", () => documentFile.OpenAsync(), hideDialogs);
        }

        public Task<bool> RenameAsync(string newName, bool hideDialogs = false)
        {
            return DoAsync("Renaming", () => documentFile.RenameAsync(newName), hideDialogs);
        }

        private async Task<bool> DoAsync(string actionName, Func<Task<bool>> action, bool hideDialogs = false)
        {
            try
            {
                return await action();
            }
            catch (FileNotFoundException)
            {
                if (!hideDialogs)
                {
                    await dialogService.AlertLocalizedAsync($"Mindmap_{actionName}Failed_NotFound_Alert");
                }
            }
            catch (Exception e)
            {
                if (!hideDialogs)
                {
#if DEBUG
                    await dialogService.AlertAsync($"The operation {actionName} failed with: {e}");
#else
                    await dialogService.AlertLocalizedAsync($"Mindmap_{actionName}Failed_Unknown_Alert");
#endif
                }

                HockeyClient.Current.TrackException(e);
            }
            finally
            {
                RaisePropertiesChanged();
            }

            return false;
        }

        public async Task<bool> RemoveAsync()
        {
            var canRemove = !HasChanges || await dialogService.ConfirmLocalizedAsync("Mindmap_Remove_Confirm");

            if (canRemove)
            {
                await documentFile.DeleteAsync();

                Close();
                Cleanup();
            }

            return canRemove;
        }

        private void RaisePropertiesChanged()
        {
            RaisePropertyChanged(nameof(DisplayPath));
            RaisePropertyChanged(nameof(HasChanges));
            RaisePropertyChanged(nameof(ModifiedLocal));
            RaisePropertyChanged(nameof(Name));
            RaisePropertyChanged(nameof(Path));
        }

        private async Task<StorageFile> PickSaveAsync(params string[] extensions)
        {
            var fileSavePicker = new FileSavePicker();

            if (extensions != null && extensions.Length > 0)
            {
                foreach (var extension in extensions)
                {
                    fileSavePicker.FileTypeChoices.Add(extension, new List<string> { extension });
                }

                fileSavePicker.DefaultFileExtension = extensions[0];
            }

            if (documentFile.File != null && !documentFile.IsInLocalFolder)
            {
                fileSavePicker.SuggestedSaveFile = documentFile.File;
            }
            else if (extensions?.Length > 0)
            {
                fileSavePicker.SuggestedFileName = Name + extensions[0];
            }

            var file = await fileSavePicker.PickSaveFileAsync();

            return file;
        }
    }
}
