using AzureBlobClient.Model;

namespace AzureBlobClient.Interfaces
{
    public interface IBlobService
    {
        Task<ResponseModel> CreateBlobContainerAsync(string blobContainerName);

        Task<ResponseModel> CreateBlobAsync(string blobContainerName, string blobName, string path);

        ResponseModel GetBlobContainerAsync(string blobContainerName);

        Task<ResponseModel> GetBlobAsync(string blobContainerName, string blobName);

        Task<ResponseModel> DeleteContainerAsync(string blobContainerName, string blobName);

        Task<ResponseModel> DeleteBlobAsync(string blobContainerName, string blobName);

        Task<int> CountBlobsAsync(string blobContainerName);

        Task<int> CountBlobsContainersAsync();
    }
}
