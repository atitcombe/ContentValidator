namespace ContentValidator.Services
{
    using ContentValidator.Models;
    using ContentValidator.Repository;
    using Microsoft.Azure.Cosmos;

    public interface IImageService
    {
        Task<ItemResponse<Image>> addImage(Image value);
        void deleteImage(String id);
        Task<Image?> getImage(String id);
    }
}
