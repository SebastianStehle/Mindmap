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

namespace Hercules.App.Components
{
    public sealed class DocumentFileModel : ViewModelBase
    {
        private readonly IDialogService dialogService;
        private readonly DocumentFile documentFile;

        public string Name
        {
            get { return documentFile.Name; }
        }

        public string Path
        {
            get { return documentFile.Path; }
        }

        public string ModifiedUtc
        {
            get { return documentFile.ModifiedUtc.ToString("g", CultureInfo.CurrentCulture); }
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

        public string DisplayPath
        {
            get
            {
                if (documentFile.IsLocalFolder)
                {
                    return string.Concat(LocalizationManager.GetString("Paths_AppFolder"), "\\", Name);
                }

                return Path;
            }
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

        public bool CanRenameTo(string newName)
        {
            return newName.IsValidFileName();
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
            StorageFile file = await PickSaveAsync(DocumentFile.Extension);

            return file != null && await SaveInternalAsync(hideDialogs, () => documentFile.SaveAsAsync(file));
        }

        public Task<bool> SaveAsync(bool hideDialogs = false)
        {
            return documentFile.File == null ? SaveAsAsync(hideDialogs) : SaveInternalAsync(hideDialogs, () => documentFile.SaveAsync());
        }

        private async Task<bool> SaveInternalAsync(bool hideDialogs, Func<Task<bool>> save)
        {
            try
            {
                return await save();
            }
            catch (FileNotFoundException)
            {
                if (!hideDialogs)
                {
                    await dialogService.AlertLocalizedAsync("Mindmap_SavingFailed_NotFound_Content", "Mindmap_SavingFailed_Heading");
                }
            }
            catch (Exception e)
            {
                if (!hideDialogs)
                {
                    await dialogService.AlertLocalizedAsync("Mindmap_SavingFailed_Unknown_Content", "Mindmap_SavingFailed_Heading");
                }

                App.TelemetryClient?.TrackException(e);
            }
            finally
            {
                RaisePropertiesChanged();
            }

            return false;
        }

        public async Task<bool> OpenAsync(bool hideDialogs = false)
        {
            try
            {
                await documentFile.OpenAsync();

                return true;
            }
            catch (FileNotFoundException)
            {
                if (!hideDialogs)
                {
                    await dialogService.AlertLocalizedAsync("Mindmap_OpeningFailed_NotFound_Content", "Mindmap_OpeningFailed_Heading");
                }
            }
            catch (Exception e)
            {
                if (!hideDialogs)
                {
                    await dialogService.AlertLocalizedAsync("Mindmap_OpeningFailed_Unknown_Content", "Mindmap_OpeningFailed_Heading");
                }

                App.TelemetryClient?.TrackException(e);
            }
            finally
            {
                RaisePropertiesChanged();
            }

            return false;
        }

        public async Task<bool> RenameAsync(string newName, bool hideDialogs = false)
        {
            try
            {
                await documentFile.RenameAsync(newName);

                return true;
            }
            catch (FileNotFoundException)
            {
                if (!hideDialogs)
                {
                    await dialogService.AlertLocalizedAsync("Mindmap_RenamingFailed_NotFound_Content", "Mindmap_RenamingFailed_Heading");
                }
            }
            finally
            {
                RaisePropertiesChanged();
            }

            return false;
        }

        public async Task<bool> RemoveAsync()
        {
            bool canRemove = !HasChanges;

            if (!canRemove)
            {
                canRemove = await dialogService.ConfirmLocalizedAsync("Mindmap_Remove_Content", "Mindmap_Remove_Heading");
            }

            if (canRemove)
            {
                Close();

                Cleanup();
            }

            return canRemove;
        }

        private void RaisePropertiesChanged()
        {
            RaisePropertyChanged(nameof(Name));
            RaisePropertyChanged(nameof(Path));
            RaisePropertyChanged(nameof(DisplayPath));
            RaisePropertyChanged(nameof(ModifiedUtc));
            RaisePropertyChanged(nameof(HasChanges));
        }

        private async Task<StorageFile> PickSaveAsync(params string[] extensions)
        {
            FileSavePicker fileSavePicker = new FileSavePicker();

            if (extensions != null)
            {
                foreach (string extension in extensions)
                {
                    fileSavePicker.FileTypeChoices.Add(extension, new List<string> { extension });
                }

                fileSavePicker.DefaultFileExtension = extensions[0];
            }

            if (documentFile.File != null && !documentFile.IsLocalFolder)
            {
                fileSavePicker.SuggestedSaveFile = documentFile.File;
            }
            else if (extensions?.Length > 0)
            {
                fileSavePicker.SuggestedFileName = Name + extensions[0];
            }

            StorageFile file = await fileSavePicker.PickSaveFileAsync();

            return file;
        }
    }
}
