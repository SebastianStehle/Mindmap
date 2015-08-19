// ==========================================================================
// DocumentRef.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================
using GP.Windows;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Hercules.Model.Storing
{
    public sealed class DocumentRef
    {
        private readonly Guid documentId;
        private readonly string documentTitle;
        private readonly DateTimeOffset lastUpdate;
        private readonly Func<Guid, BitmapImage, Task<bool>> imageLoader;
        private BitmapImage screenshot;

        public Guid DocumentId
        {
            get
            {
                return documentId;
            }
        }

        public string DocumentTitle
        {
            get
            {
                return documentTitle;
            }
        }

        public ImageSource Screenshot
        {
            get
            {
                return screenshot;
            }
        }

        public DateTimeOffset LastUpdate
        {
            get
            {
                return lastUpdate;
            }
        }

        public DocumentRef(Guid documentId, string documentTitle, DateTimeOffset lastUpdate, Func<Guid, BitmapImage, Task<bool>> imageLoader)
        {
            Guard.NotNull(documentTitle, nameof(documentTitle));

            this.documentTitle = documentTitle;
            this.documentId = documentId;
            this.lastUpdate = lastUpdate;
            this.imageLoader = imageLoader;
        }

        public async Task<bool> LoadImageAsync()
        {
            if (screenshot == null)
            {
                screenshot = new BitmapImage();
            }

            return await imageLoader(documentId, screenshot);
        }
    }
}
