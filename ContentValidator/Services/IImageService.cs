namespace ContentValidator.Services
{
    using ContentValidator.Models;
    using ContentValidator.Repository;
    using Microsoft.Azure.Cosmos;

    public interface IImageService
    {
        Task<ItemResponse<Post>> addImage(PostInputDto value);
        void deleteImage(String id);
        Task<PostInputDto?> getImage(String id);
    }
}