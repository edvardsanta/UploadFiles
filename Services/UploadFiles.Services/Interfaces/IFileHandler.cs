using Microsoft.AspNetCore.Http;
using UploadFiles.Shared.Contracts;
using FileTypeExt = (UploadFiles.Services.Utils.FileType, UploadFiles.Services.Utils.FileExtension);

namespace UploadFiles.Services.Interfaces
{
    public interface IFileHandler
    {
        FileTypeExt FileType { get; }
        Task<NormalizeTextMessage> HandleFileAsync(IFormFile file);
    }
}
