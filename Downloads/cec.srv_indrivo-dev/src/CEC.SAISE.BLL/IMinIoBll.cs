using System.IO;

namespace CEC.SAISE.BLL
{
    public interface IMinIoBll
    {
        string UploadFile(string objectName, Stream data, string contentType);
        string DownloadFile(string objectName, string filePath);
        string DeleteFile(string objectName);
    }
}
