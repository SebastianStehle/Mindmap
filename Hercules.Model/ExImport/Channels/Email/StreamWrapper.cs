// ==========================================================================
// StreamWrapper.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.IO;
using Windows.Foundation;
using Windows.Storage.Streams;
using GP.Windows;

namespace Hercules.Model.ExImport.Channels.Email
{
    internal sealed class StreamWrapper : IRandomAccessStreamWithContentType
    {
        private readonly IRandomAccessStream inner;
        private readonly string contentType;

        public ulong Size
        {
            get { return inner.Size; }
            set { inner.Size = value; }
        }

        public bool CanRead
        {
            get { return inner.CanRead; }
        }

        public bool CanWrite
        {
            get { return inner.CanWrite; }
        }

        public ulong Position
        {
            get { return inner.Position; }
        }

        public string ContentType
        {
            get { return contentType; }
        }

        public void Dispose()
        {
            inner.Dispose();
        }

        public StreamWrapper(MemoryStream stream, string contentType)
        {
            Guard.NotNull(stream, nameof(stream));
            Guard.NotNullOrEmpty(contentType, nameof(contentType));

            this.inner = stream.AsRandomAccessStream();

            this.contentType = contentType;
        }

        public StreamWrapper(IRandomAccessStream inner, string contentType)
        {
            Guard.NotNull(inner, nameof(inner));
            Guard.NotNullOrEmpty(contentType, nameof(contentType));

            this.inner = inner;

            this.contentType = contentType;
        }

        public IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
        {
            return inner.ReadAsync(buffer, count, options);
        }

        public IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer)
        {
            return inner.WriteAsync(buffer);
        }

        public IAsyncOperation<bool> FlushAsync()
        {
            return inner.FlushAsync();
        }

        public IInputStream GetInputStreamAt(ulong position)
        {
            return inner.GetInputStreamAt(position);
        }

        public IOutputStream GetOutputStreamAt(ulong position)
        {
            return inner.GetOutputStreamAt(position);
        }

        public IRandomAccessStream CloneStream()
        {
            return inner.CloneStream();
        }

        public void Seek(ulong position)
        {
            inner.Seek(position);
        }
    }
}
