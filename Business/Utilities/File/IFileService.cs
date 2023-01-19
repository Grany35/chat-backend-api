using Core.Utilities.Result.Abstract;
using Microsoft.AspNetCore.Http;

namespace Business.Abstract
{
    public interface IFileService
    {
        IResult SaveFile(IFormFile file);
        string GetFilePath(string fileName);
        string FileSaveToServer(IFormFile file, string filePath);
        string FileSaveToFtp(IFormFile file);
        byte[] FileConvertByteArrayToDatabase(IFormFile file);
        void FileDeleteToServer(string path);
        void FileDeleteToFtp(string path);
    }
}
