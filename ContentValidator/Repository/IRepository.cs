using ContentValidator.Models;
using Microsoft.Azure.Cosmos;

namespace ContentValidator.Repository
{
    public interface IRepository
    {
        Task<Image?> GetImage(string v);
        Task<ItemResponse<Image>> PostImage(Image image);
    }
}
