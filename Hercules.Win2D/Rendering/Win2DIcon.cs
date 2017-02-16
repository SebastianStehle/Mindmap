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
using Windows.UI.Core;
using GP.Utils;
using GP.Utils.UI.Controls;
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
            using (var stream = await icon.OpenAsStreamAsync())
            {
                if (stream != null)
                {
                    return await CanvasBitmap.LoadAsync(device, stream.AsRandomAccessStream()).AsTask();
                }

                return null;
            }
        }

        public async Task<Stream> ToStreamAsync()
        {
            var memoryStream = new MemoryStream();

            await bitmap.SaveAsync(memoryStream.AsRandomAccessStream(), CanvasBitmapFileFormat.Png);

            return memoryStream;
        }

        protected override void DisposeObject(bool disposing)
        {
            var currentBitmap = bitmap;

            if (currentBitmap != null)
            {
                currentBitmap.Dispose();
            }
        }
    }
}
