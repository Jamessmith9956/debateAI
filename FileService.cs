using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace DebateAIApi
{
    // ripped from https://www.youtube.com/watch?v=DzQ7CNnb9yM
    public class FileService
    {
        private readonly string _storageAccount;
        private readonly string _key;
        private readonly BlobContainerClient _filesContainter;

        public FileService(IConfiguration configuration)
        {
            // pull from config 
            _storageAccount = configuration.GetValue<string>("StorageAccount");
            _key = configuration.GetValue<string>("StorageKey");
            var credential = new StorageSharedKeyCredential(_storageAccount, _key);
            var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
            var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            _filesContainter = blobServiceClient.GetBlobContainerClient("files");
        }
        
        public async Task<List<BlobDto>> ListAsync()
        {
            List<BlobDto> files = new List<BlobDto>();

            await foreach (var blobItem in _filesContainter.GetBlobsAsync())
            {
                var blobClient = _filesContainter.GetBlobClient(blobItem.Name);
                var blobDownloadInfo = await blobClient.DownloadAsync();
                var blob = new BlobDto
                {
                    Name = blobItem.Name,
                    Uri = blobClient.Uri.AbsoluteUri,
                    ContentType = blobDownloadInfo.Value.ContentType,
                    Content = blobDownloadInfo.Value.Content // not sure if this is correct
                };
                files.Add(blob);
            }
            return files;
        }
        
        public async Task<BlobResponseDto> UploadAsync(IFormFile blob)
        {
            BlobResponseDto response = new();
            // Try a Try/Catch block here
            BlobClient client = _filesContainter.GetBlobClient(blob.FileName);

            // stream is necessary as we don't have acess to the file in the file system
            await using (Stream? data = blob.OpenReadStream()) 
            {
                await client.UploadAsync(data);
            }

            response.Status = $"File {blob.FileName} uploaded successfully";
            response.Error = false;
            response.Blob.Uri = client.Uri.AbsoluteUri;
            response.Blob.Name = client.Name;

            return response; 
        }


        public async Task<BlobDto> DownloadAsync(string blobFileName)
        {
            BlobClient file = _filesContainter.GetBlobClient(blobFileName);

            if (await file.ExistsAsync())
            {
                // not sure if .Download or .openread is prefereable
                //var blobDownloadInfo = await file.DownloadAsync();
                var data = await file.OpenReadAsync();
                Stream blobContent = data;

                var content = await file.DownloadContentAsync();

                return new BlobDto
                {
                    Content = blobContent,
                    Name = blobFileName,
                    ContentType = content.Value.Details.ContentType
                };
            }
            return null;
        }

        public async Task<BlobResponseDto> DeleteAsync(string blobFileName)
        {
            BlobClient file = _filesContainter.GetBlobClient(blobFileName);
            // need to give 404 if the file doesn't exist
            
            await file.DeleteIfExistsAsync();

            return new BlobResponseDto
            {
                Status = $"File {blobFileName} deleted successfully",
                Error = false
            };

        }   

    }

}
