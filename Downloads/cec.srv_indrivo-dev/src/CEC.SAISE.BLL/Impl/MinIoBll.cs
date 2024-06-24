using Minio;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace CEC.SAISE.BLL.Impl
{
    public class MinIoBll : IMinIoBll
    {
        private readonly MinioClient minioClient;
        private readonly string bucketName;

        public MinIoBll()
        {
            string endpoint = ConfigurationManager.AppSettings["MinioEndpoint"];
            string accessKey = ConfigurationManager.AppSettings["MinioAccessKey"];
            string secretKey = ConfigurationManager.AppSettings["MinioSecretKey"];
            this.bucketName = ConfigurationManager.AppSettings["MinioBucketName"];

            this.minioClient = new MinioClient(endpoint, accessKey, secretKey);
            EnsureBucketExists();
        }

        private void EnsureBucketExists()
        {
            try
            {
                bool found = minioClient.BucketExists(bucketName);
                if (!found)
                {
                    minioClient.MakeBucket(bucketName);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
        }

        public string UploadFile(string objectName, Stream data, string contentType)
        {
            try
            {
                long size = data.Length;
                minioClient.PutObject(bucketName, objectName, size, contentType, data);
                return "File uploaded successfully.";
            }
            catch (Exception ex)
            {
                return $"Error during upload: {ex.Message}";
            }
        }

        public string DownloadFile(string objectName, string filePath)
        {
            try
            {
                Action<Stream> writeToFile = (stream) =>
                {
                    using (var fileStream = File.Create(filePath))
                    {
                        stream.CopyTo(fileStream);
                    }
                };

                minioClient.GetObject(bucketName, objectName, writeToFile);
                return "File downloaded successfully.";
            }
            catch (Exception ex)
            {
                return $"Error during download: {ex.Message}";
            }
        }

        public string DeleteFile(string objectName)
        {
            try
            {
                minioClient.RemoveObject(bucketName, objectName);
                return "File deleted successfully.";
            }
            catch (Exception ex)
            {
                return $"Error during deletion: {ex.Message}";
            }
        }
    }
}
