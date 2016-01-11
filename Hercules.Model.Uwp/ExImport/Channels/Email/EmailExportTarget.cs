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
        private readonly IMessageDialogService dialogService;

        public string NameKey
        {
            get { return "Email"; }
        }

        public EmailExportTarget(IMessageDialogService dialogService)
        {
            Guard.NotNull(dialogService, nameof(dialogService));

            this.dialogService = dialogService;
        }

        public async Task ExportAsync(string name, Document document, IExporter exporter, IRenderer renderer)
        {
            if (await dialogService.ConfirmAsync(LocalizationManager.GetString("Export_EmailConfirm")))
            {
                FileExtension extension = exporter.Extensions.FirstOrDefault();

                if (extension == null)
                {
                    throw new InvalidOperationException("The exporter needs to specify at least one file extension.");
                }

                byte[] buffer = await SerializeAsync(document, exporter, renderer);

                string downloadUri = await UploadAsync(name, extension, buffer);

                string subj = LocalizationManager.GetString("Export_EmailSubject");
                string body = LocalizationManager.GetFormattedString("Export_EmailBody", downloadUri);

                EmailMessage message = new EmailMessage { Subject = subj, Body = body };

                await EmailManager.ShowComposeNewEmailAsync(message);
            }
        }

        private static async Task<string> UploadAsync(string name, FileExtension extension, byte[] buffer)
        {
            object upload = new { ContentType = extension.MimeType, Content = Convert.ToBase64String(buffer), Name = name + extension.Extension };

            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.PostAsJsonAsync("http://upload.getmindapp.com/api/upload", upload);

            response.EnsureSuccessStatusCode();

            return response.Headers.Location.ToString();
        }

        private static async Task<byte[]> SerializeAsync(Document document, IExporter exporter, IRenderer renderer)
        {
            MemoryStream memoryStream = new MemoryStream();

            await exporter.ExportAsync(document, renderer, memoryStream);

            return memoryStream.ToArray();
        }
    }
}
