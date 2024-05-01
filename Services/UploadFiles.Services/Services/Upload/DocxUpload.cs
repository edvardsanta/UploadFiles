using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using UploadFiles.Services.Services.Upload.Abstractions;
using UploadFiles.Services.Utils;
using UploadFiles.Shared.Contracts;
using FileTypeExt = (UploadFiles.Services.Utils.FileType, UploadFiles.Services.Utils.FileExtension);

namespace UploadFiles.Services.Services.Upload
{
    public class DocxUpload : DocumentUpload
    {
        public override FileTypeExt FileType { get; set; } = (Utils.FileType.Document, FileExtension.DOCX);

        public override async Task<NormalizeTextMessage> HandleFileAsync(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Open(stream, true))
                {
                    StringBuilder text = new StringBuilder();
                    foreach (Paragraph para in doc.MainDocumentPart.Document.Body.Elements<Paragraph>())
                    {
                        foreach (Run run in para.Elements<Run>())
                        {
                            text.Append(run.InnerText);
                        }
                        text.AppendLine();
                    }
                }
            }
            return new("");
        }
    }
}