using AMiON.Helper.ConfigurationManagerHelper;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace AMiON.Helper
{
    public class AzureBlobStorage
    {
        //string containerName = "mappingfiles";
        string StorageConnection = AppSetting.BlobStorageConnection;

        public void UploadFileToBlobStorage(string containerName, string filePath, Stream stream)
        {
            var blockBlob = GetBlobInstance(containerName).GetBlockBlobReference(filePath);
            blockBlob.UploadFromStream(stream);
        }

        public Stream DownloadFileFromBlobStorage(string containerName, string filePath)
        {
            //  var filePath = $"Amion/{tenantId}/{internalTeamId}/{fileName}";
            var reader = new MemoryStream();
            GetBlobInstance(containerName).GetBlockBlobReference(filePath).DownloadToStreamAsync(reader).GetAwaiter().GetResult();
            return reader;
        }

        private CloudBlobContainer GetBlobInstance(string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(StorageConnection);
            var myClient = storageAccount.CreateCloudBlobClient();
            var container = myClient.GetContainerReference(containerName);
            container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);
            return container;
        }
    }
}
