using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureBlobClient.Interfaces;
using AzureBlobClient.Model;

namespace AzureBlobClient.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<ResponseModel> CreateBlobContainerAsync(string blobContainerName)
        {
            BlobContainerClient blobContainer
                = _blobServiceClient.GetBlobContainerClient(blobContainerName);

            if (!blobContainer.Exists().Value)
            {
                Response<BlobContainerClient> rs = await _blobServiceClient.CreateBlobContainerAsync(blobContainerName);

                return new ResponseModel()
                {
                    Status = rs.GetRawResponse().Status,
                    ResponsePhrase = rs.GetRawResponse().ReasonPhrase
                };
            }
            else return new ResponseModel()
            {
                Status = 1,
                ResponsePhrase = "Blob container already exist"
            };
        }

        public async Task<ResponseModel> CreateBlobAsync(string blobContainerName, string blobName, string path)
        {
            BlobContainerClient blobContainer
                = _blobServiceClient.GetBlobContainerClient(blobContainerName);

            if (blobContainer.Exists().Value)
            {
                BlobClient blobClient = blobContainer.GetBlobClient(blobName);

                if (!blobClient.Exists().Value)
                {
                    Response<BlobContentInfo> rs = await blobClient.UploadAsync(path, new BlobHttpHeaders
                    {
                        ContentType = "application/pdf"
                    });

                    return new ResponseModel()
                    {
                        Status = rs.GetRawResponse().Status,
                        ResponsePhrase = rs.GetRawResponse().ReasonPhrase
                    };
                }
                else return new ResponseModel()
                {
                    Status = 2,
                    ResponsePhrase = "Blob file already exist"
                };
            }
            else return new ResponseModel()
            {
                Status = 1,
                ResponsePhrase = "Blob container does not exist"
            };
        }

        public ResponseModel GetBlobContainerAsync(string blobContainerName)
        {
            BlobContainerClient blobContainer
                = _blobServiceClient.GetBlobContainerClient(blobContainerName);

            if (blobContainer.Exists().Value)
            {
                return new ResponseModel()
                {
                    Status = 200,
                    ResponsePhrase = "OK",
                    BlobContainer = blobContainerName
                };
            }
            else return new ResponseModel()
            {
                Status = 1,
                ResponsePhrase = "Blob container does not exist"
            };
        }

        public async Task<ResponseModel> GetBlobAsync(string blobContainerName, string blobName)
        {
            var blobContainer
                = _blobServiceClient.GetBlobContainerClient(blobContainerName);

            if (blobContainer.Exists().Value)
            {
                BlobClient blobClient = blobContainer.GetBlobClient(blobName);

                if (blobClient.Exists().Value)
                {
                    Response<BlobDownloadInfo> downloadBlob = await blobClient.DownloadAsync();
                    using MemoryStream memoryStream = new();
                    downloadBlob.Value.Content.CopyTo(memoryStream);

                    return new ResponseModel()
                    {
                        Blob = memoryStream.ToArray(),
                        Status = 200,
                        ResponsePhrase = "OK"
                    };
                }
                else return new ResponseModel()
                {
                    Status = 2,
                    ResponsePhrase = "Blob file does not exist"
                };
            }
            else return new ResponseModel()
            {
                Status = 1,
                ResponsePhrase = "Blob container does not exist"
            };
        }

        public async Task<ResponseModel> DeleteContainerAsync(string blobContainerName, string blobName)
        {
            BlobContainerClient blobContainer
                = _blobServiceClient.GetBlobContainerClient(blobContainerName);

            if (blobContainer.Exists().Value)
            {
                Response rs = await _blobServiceClient.DeleteBlobContainerAsync(blobContainerName);

                return new ResponseModel()
                {
                    Status = rs.Status,
                    ResponsePhrase = rs.ReasonPhrase
                };
            }
            else return new ResponseModel()
            {
                Status = 1,
                ResponsePhrase = "Blob container does not exist"
            };

        }

        public async Task<ResponseModel> DeleteBlobAsync(string blobContainerName, string blobName)
        {
            BlobContainerClient blobContainer
                = _blobServiceClient.GetBlobContainerClient(blobContainerName);

            if (blobContainer.Exists().Value)
            {
                BlobClient blobClient = blobContainer.GetBlobClient(blobName);

                if (blobClient.Exists().Value)
                {
                    Response rs = await blobClient.DeleteAsync();

                    return new ResponseModel()
                    {
                        Status = rs.Status,
                        ResponsePhrase = rs.ReasonPhrase
                    };
                }
                else return new ResponseModel()
                {
                    Status = 2,
                    ResponsePhrase = "Blob file does not exist"
                };
            }
            else return new ResponseModel()
            {
                Status = 1,
                ResponsePhrase = "Blob container does not exist"
            };

        }

        public async Task<int> CountBlobsAsync(string blobContainerName)
        {
            List<string> items = new();

            await foreach (var blobItem in _blobServiceClient.GetBlobContainerClient(blobContainerName).GetBlobsAsync()) items.Add(blobItem.Name);

            return items.Count;
        }

        public async Task<int> CountBlobsContainersAsync()
        {
            List<string> blobContainers = new();

            await foreach (var blobContainer in _blobServiceClient.GetBlobContainersAsync()) blobContainers.Add(blobContainer.Name);

            return blobContainers.Count;
        }        
    }
}
