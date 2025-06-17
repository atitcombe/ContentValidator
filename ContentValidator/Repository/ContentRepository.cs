using Microsoft.Azure.Cosmos;
using Azure.Identity;
using ContentValidator.Models;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.AspNetCore.Http.HttpResults;
using Azure;

namespace ContentValidator.Repository
{

    public class ContentRepository: IRepository
    {
        private readonly CosmosClient cosmosClient;
        private readonly Database database;
        private readonly IConfiguration Configuration;
        private readonly ILogger _logger;
        DefaultAzureCredential credential = new DefaultAzureCredential();

        public ContentRepository(IConfiguration configuration, ILogger<ContentRepository> logger)
        {
            Configuration = configuration;
            _logger = logger;
            
            cosmosClient = new CosmosClient(Configuration["ConnectionStrings:DatabaseName"], credential);
            database = cosmosClient.GetDatabase("images");

        }

        public async Task<Image?> GetImage(String id)
        {
            Container container = database.GetContainer("pics");
            _logger.LogInformation("container is " + container.ReadContainerAsync());

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
            if (image == null)
            {
                throw new Exception();
            }
            var response= await container.CreateItemAsync(image);
            _logger.LogInformation("response to post is " + response);
            
            return response;
        }


    }
}
