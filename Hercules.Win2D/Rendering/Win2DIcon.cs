// ==========================================================================
// Win2DIcon.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using System.Threading.Tasks;
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

        public Win2DIcon(INodeIcon icon, ICanvasControl canvasControl)
        {
            Guard.NotNull(icon, nameof(icon));

            name = icon.Name;

            LoadFile(icon, canvasControl.Device).ContinueWith(task => AttachBitmap(canvasControl, task));
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
        
        private static async Task<CanvasBitmap> LoadFile(INodeIcon icon, ICanvasResourceCreator device)
        {
            using (IRandomAccessStream stream = await icon.OpenAsStreamAsync())
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
