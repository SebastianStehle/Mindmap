// ==========================================================================
// Extensions.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace GP.Utils
{
    /// <summary>
    /// A collection of simple helper extension methods.
    /// </summary>
    public static class PlattformExtensions
    {
        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="task">The task.</param>
        public static void Forget(this IAsyncAction task)
        {
        }

        /// <summary>
        /// Converts the specified stream to a memory stream asynchronously.
        /// </summary>
        /// <param name="stream">The stream to convert.</param>
        /// <returns>
        /// The resulting memory stream.
        /// </returns>
        public static async Task<MemoryStream> ToMemoryStreamAsync(this IRandomAccessStream stream)
        {
            MemoryStream memoryStream = new MemoryStream();

            using (Stream ioStream = stream.AsStreamForRead())
            {
                await ioStream.CopyToAsync(memoryStream).ConfigureAwait(false);
            }

            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}
