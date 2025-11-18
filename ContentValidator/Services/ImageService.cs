using ContentValidator.Core;
using ContentValidator.Models;
using ContentValidator.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;

namespace ContentValidator.Services;

public class ImageService(IRepository contentRepository) : IImageService
{
    
    static List<PostInputDto> Images {  get; } = new List<PostInputDto>();

    public static List<PostInputDto> GetAll() => Images;

    public async Task<PostInputDto?> getImage(string id)
    {
       var image = await contentRepository.GetImage(id);
        if (image == null)
        {
            return null;
        }
        return image;

    }

    public async void deleteImage(String id)
    {
        PostInputDto? image = await getImage(id);
        if (image != null)
        {
            Images?.Remove(image);
        } else
        {
            return;
        }
        
    }

    public async Task<ItemResponse<Post>> addImage(PostInputDto image)
    {
        if (image == null)
        {
            throw new ArgumentNullException();
        }
        
        image.ByteEncoding = await ProcessIFormFile(image.ImageUpload);
        return await contentRepository.PostImage(image);

    }

    private async Task<byte[]> ProcessIFormFile(IFormFile formFile)
    {
        using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);

        if (memoryStream.Length > Constants.MAX_FILE_UPLOAD)
        {
            throw new ArgumentException("File too large");
        }

        return memoryStream.ToArray();
    }


}
