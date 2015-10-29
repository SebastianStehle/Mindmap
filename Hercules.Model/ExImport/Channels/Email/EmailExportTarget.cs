// ==========================================================================
// EmailExportTarget.cs
// Hercules Mindmap App
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Email;
using GP.Windows;
using Hercules.Model.Rendering;
using Hercules.Model.Utils;

namespace Hercules.Model.ExImport.Channels.Email
{
    public sealed class EmailExportTarget : IExportTarget
    {
        public string NameKey
        {
            get { return "Email"; }
        }

        public async Task ExportAsync(Document document, IExporter exporter, IRenderer renderer)
        {
            FileExtension extension = exporter.Extensions.FirstOrDefault();

            if (extension == null)
            {
                throw new InvalidOperationException("The exporter needs to specify at least one file extension.");
            }

            string subj = ResourceManager.GetString("Export_EmailSubject");
            string file = ResourceManager.GetString("Export_EmailAttachment") + extension.Extension;

            EmailMessage message = new EmailMessage {  Subject = subj };

            message.Attachments.Add(new EmailAttachment(file, new EmailAttachmentSource(document, exporter, renderer)));

            await EmailManager.ShowComposeNewEmailAsync(message);
        }
    }
}
