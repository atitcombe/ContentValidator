using Microsoft.Azure.Cosmos;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using ContentValidator.Models;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.AspNetCore.Http.HttpResults;
using Azure;
using Microsoft.Extensions.Logging;

namespace ContentValidator.Repository
{

    public class ContentRepository: IRepository
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
            



            cosmosClient = new CosmosClient(Configuration["ConnectionStrings:DatabaseName"], credential);
            database = cosmosClient.GetDatabase("images");

        }

        public async Task<Image?> GetImage(String id)
        {
            _logger.LogInformation("Get  request made in Repo now at {DT}",
          DateTime.UtcNow.ToLongTimeString());

            var secret = await secretClient.GetSecretAsync("cdb-name");            
            var cosmosClient = new CosmosClient(secret.Value.Value, credential);
            var database = cosmosClient.GetDatabase("images");

            Container container = database.GetContainer("pics");

            using FeedIterator<Image> imageIterator = container.GetItemLinqQueryable<Image>().
                Where(item => id == item.id).ToFeedIterator();

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

        public async Task<ItemResponse<Image>> PostImage(Image image)
        {
            Container container = database.GetContainer("pics");
            _logger.LogInformation("Post request made in Repo now at {DT}",
           DateTime.UtcNow.ToLongTimeString());

            if (image == null)
            {
                throw new Exception();
            }
            var response= await container.CreateItemAsync(image);
            _logger.LogInformation("response to post is " + response.Resource);
            
            return response;
        }


    }
}
