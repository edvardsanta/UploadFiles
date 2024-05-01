using Microsoft.AspNetCore.Http;
using UploadFiles.Services.Interfaces;
using UploadFiles.Shared.Contracts;
using FileTypeExt = (UploadFiles.Services.Utils.FileType, UploadFiles.Services.Utils.FileExtension);

namespace UploadFiles.Services.Services.Upload.Abstractions
{
    public abstract class ImageUpload : IFileHandler
    {
        public abstract FileTypeExt FileType { get; set; }

        public abstract Task<NormalizeTextMessage> HandleFileAsync(IFormFile file);
    }
}
