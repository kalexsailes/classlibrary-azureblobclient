namespace AzureBlobClient.Model
{
    public class ResponseModel
    {
        public string BlobContainer { get; set; }
        public byte[] Blob { get; set; }
        public int Status { get; set; }
        public string ResponsePhrase { get; set; }
    }
}
