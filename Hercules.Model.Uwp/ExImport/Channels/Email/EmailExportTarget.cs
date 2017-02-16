// ==========================================================================
// EmailExportTarget.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using GP.Utils;
using GP.Utils.Mvvm;
using Hercules.Model.Rendering;

namespace Hercules.Model.ExImport.Channels.Email
{
    public sealed class EmailExportTarget : IExportTarget
    {
        private readonly IDialogService dialogService;

        public string NameKey
        {
            get { return "Email"; }
        }

        public EmailExportTarget(IDialogService dialogService)
        {
            Guard.NotNull(dialogService, nameof(dialogService));

            this.dialogService = dialogService;
        }

        public async Task ExportAsync(string name, Document document, IExporter exporter, IRenderer renderer)
        {
            if (await dialogService.ConfirmAsync(LocalizationManager.GetString("Export_EmailConfirm")))
            {
                var extension = exporter.Extensions.FirstOrDefault();

                if (extension == null)
                {
                    throw new InvalidOperationException("The exporter needs to specify at least one file extension.");
                }

                var buffer = await SerializeAsync(document, exporter, renderer);

                var downloadUri = await UploadAsync(name, extension, buffer);

                var subj = LocalizationManager.GetFormattedString("Export_EmailSubject");
                var body = LocalizationManager.GetFormattedString("Export_EmailBody", downloadUri);

                var message = new EmailMessage { Subject = subj, Body = body };

                await EmailManager.ShowComposeNewEmailAsync(message);
            }
        }

        private static async Task<string> UploadAsync(string name, FileExtension extension, byte[] buffer)
        {
            object upload = new { ContentType = extension.MimeType, Content = Convert.ToBase64String(buffer), Name = name + extension.Extension };

            var httpClient = new HttpClient();

            var response = await httpClient.PostAsJsonAsync("http://upload.getmindapp.com/api/upload", upload);

            response.EnsureSuccessStatusCode();

            return response.Headers.Location.ToString();
        }

        private static async Task<byte[]> SerializeAsync(Document document, IExporter exporter, IRenderer renderer)
        {
            var memoryStream = new MemoryStream();

            await exporter.ExportAsync(document, renderer, memoryStream);

            return memoryStream.ToArray();
        }
    }
}
