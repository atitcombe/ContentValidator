using ContentValidator.Models;
using Microsoft.Azure.Cosmos;

namespace ContentValidator.Repository
{
    public interface IRepository
    {
        Task<PostInputDto?> GetImage(string v);
        Task<ItemResponse<Post>> PostImage(PostInputDto image);
    }
}