using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using ContentValidator.Models;
using ContentValidator.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;

namespace ContentValidator.Repository
{
    public class ContentRepository : IRepository
    {
        private readonly CosmosClient cosmosClient;
        private readonly SecretClient secretClient;
        private readonly Database database;
        private readonly IConfiguration Configuration;
        private readonly ILogger _logger;
        DefaultAzureCredential credential = new DefaultAzureCredential();

        public ContentRepository(IConfiguration configuration, ILogger<ContentRepository> logger)
        {
            Configuration = configuration;
            _logger = logger;

            string keyVaultName = Configuration["ConnectionStrings:KEY_VAULT_NAME"];
            if (keyVaultName == null)
            {
                throw new Exception();
            }
            var kvUri = "https://" + keyVaultName + ".vault.azure.net";
            _logger.LogInformation("the key vault uri is " + kvUri + "\n");
            secretClient = new SecretClient(new Uri(kvUri), credential);

            cosmosClient = new CosmosClient(
                Configuration["ConnectionStrings:DatabaseName"],
                credential
            );
            database = cosmosClient.GetDatabase("images");
        }

        public async Task<PostInputDto?> GetImage(String id)
        {
            _logger.LogInformation(
                "Get  request made in Repo now at {DT}",
                DateTime.UtcNow.ToLongTimeString()
            );

            var secret = await secretClient.GetSecretAsync("cdb-name");
            var cosmosClient = new CosmosClient(secret.Value.Value, credential);
            var database = cosmosClient.GetDatabase("images");

            Container container = database.GetContainer("pics");

            using FeedIterator<PostInputDto> imageIterator = container
                .GetItemLinqQueryable<PostInputDto>()
                .Where(item => id == item.id)
                .ToFeedIterator();

            while (imageIterator.HasMoreResults)
            {
                foreach (var item in await imageIterator.ReadNextAsync())
                {
                    _logger.LogInformation("response to post is " + item);
                    return item;
                }
            }
            return null;
        }

        public async Task<ItemResponse<Post>> PostImage(PostInputDto image)
        {
            Container container = database.GetContainer("pics");
            _logger.LogInformation(
                "Post request made in Repo now at {DT}",
                DateTime.UtcNow.ToLongTimeString()
            );

            // Get service and container clients (your helper class)
            var serviceClient = BlobStorageAccountClient.GetBlobServiceClient(
                "contentvalidatorb3cb"
            );
            var containerClient = BlobStorageAccountClient.GetBlobContainerClient(
                serviceClient,
                "content-validator-items"
            );

            // Ensure the container exists before uploading
            await containerClient.CreateIfNotExistsAsync();

            string? untrustedFileName = Path.GetFileName(image.ImageUpload.FileName);

            string blobName = image.id;

            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            byte[] bytes = image.ByteEncoding;
            using var ms = new MemoryStream(bytes);

            // Upload (overwrite if exists)
            await blobClient.UploadAsync(ms, overwrite: true);
            _logger.LogInformation("Uploaded to blob storage");

            // Optionally set content type
            await blobClient.SetHttpHeadersAsync(
                new Azure.Storage.Blobs.Models.BlobHttpHeaders
                {
                    ContentType = image.Type ?? "application/octet-stream",
                }
            );

            Post uploadingImage = new()
            {
                id = image.id,
                TextContent = image.TextContent,
                BlobReferenceLink = blobClient.Uri.ToString(),
            };
            var response = await container.CreateItemAsync(uploadingImage);
            _logger.LogInformation("Item uploaded to cosmos with response {Response}", response.Resource);

            return response;
        }
    }
}
