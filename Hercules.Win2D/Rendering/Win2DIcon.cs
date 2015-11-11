// ==========================================================================
// Win2DIcon.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using GP.Windows;
using GP.Windows.UI.Controls;
using Hercules.Model;
using Hercules.Model.Rendering;
using Microsoft.Graphics.Canvas;

namespace Hercules.Win2D.Rendering
{
    public sealed class Win2DIcon : DisposableObject, IRenderIcon
    {
        private readonly string name;
        private CanvasBitmap bitmap;

        public CanvasBitmap Bitmap
        {
            get { return bitmap; }
        }

        public string Name
        {
            get { return name; }
        }

        public Win2DIcon(AttachmentIcon icon, ICanvasControl canvasControl)
        {
            Guard.NotNull(icon, nameof(icon));

            name = icon.Name;

            LoadFile(icon.Base64Content, canvasControl.Device).ContinueWith(task => AttachBitmap(canvasControl, task));
        }

        public Win2DIcon(KeyIcon icon, ICanvasControl canvasControl)
        {
            Guard.NotNull(icon, nameof(icon));
            Guard.NotNull(canvasControl, nameof(canvasControl));

            name = $"{icon.Key}.png";

            LoadFile(icon, canvasControl.Device).ContinueWith(task => AttachBitmap(canvasControl, task));
        }

        public static Win2DIcon Create(INodeIcon icon, ICanvasControl canvasControl)
        {
            KeyIcon keyIcon = icon as KeyIcon;

            if (keyIcon != null)
            {
                return new Win2DIcon(keyIcon, canvasControl);
            }

            AttachmentIcon attachmentIcon = icon as AttachmentIcon;

            return attachmentIcon != null ? new Win2DIcon(attachmentIcon, canvasControl) : null;
        }

        private void AttachBitmap(ICanvasControl canvasControl, Task<CanvasBitmap> task)
        {
            canvasControl.Dispatcher.RunAsync(CoreDispatcherPriority.High, canvasControl.Invalidate).AsTask();

            if (IsDisposed)
            {
                task.Result.Dispose();
            }
            else
            {
                bitmap = task.Result;
            }
        }

        private static async Task<CanvasBitmap> LoadFile(string base64Content, ICanvasResourceCreator device)
        {
            MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(base64Content));

            using (IRandomAccessStream stream = memoryStream.AsRandomAccessStream())
            {
                return await CanvasBitmap.LoadAsync(device, stream).AsTask();
            }
        }

        private static async Task<CanvasBitmap> LoadFile(KeyIcon icon, ICanvasResourceCreator device)
        {
            string uri = string.Format(CultureInfo.InvariantCulture, "ms-appx://{0}", icon.Key);

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));

            using (IRandomAccessStream stream = await file.OpenReadAsync())
            {
                return await CanvasBitmap.LoadAsync(device, stream).AsTask();
            }
        }

        public async Task<Stream> ToStreamAsync()
        {
            MemoryStream memoryStream = new MemoryStream();

            await bitmap.SaveAsync(memoryStream.AsRandomAccessStream(), CanvasBitmapFileFormat.Png);

            return memoryStream;
        }

        protected override void DisposeObject(bool disposing)
        {
            CanvasBitmap currentBitmap = bitmap;

            if (currentBitmap != null)
            {
                currentBitmap.Dispose();
            }
        }
    }
}
