using ContentValidator.Models;
using ContentValidator.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;

namespace ContentValidator.Services;

public class ImageService(IRepository contentRepository) : IImageService
{
    static int nextId = 2;
    
    static List<Image> Images {  get; }

    public static List<Image> GetAll() => Images;

    public async Task<Image?> getImage(string id)
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
        Image? image = await getImage(id);
        if (image != null)
        {
            Images?.Remove(image);
        } else
        {
            return;
        }
        
    }

    public async Task<ItemResponse<Image>> addImage(Image image)
    {
        if (image == null)
        {
            throw new Exception();
        }
        Images?.Add(image);        
        return await contentRepository.PostImage(image);

    }


}
