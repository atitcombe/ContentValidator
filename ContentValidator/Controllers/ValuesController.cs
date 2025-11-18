using ContentValidator.Core;
using ContentValidator.Models;
using ContentValidator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContentValidator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController(IImageService ImageService, ILogger<ImageController> logger) : ControllerBase
    {
       

        // GET api/<ValudatorController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostInputDto>> GetById(String id)
        {
            var image = await ImageService.getImage(id);
            logger.LogInformation("the gotten image from controller is: " + image);
            if (image == null)
            {
                return NotFound();
            }
            return image;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] PostInputDto value)
        {
            logger.LogInformation("Post request made at {DT}",
           DateTime.UtcNow.ToLongTimeString());

            if (!value.ImageUpload.HasValidFileExtension() || !value.ImageUpload.IsInFileSizeMinimimum())
            {
                return StatusCode(400);
            }
            try
            {
                var response = await ImageService.addImage(value);
                return CreatedAtAction(nameof(GetById), new { value.id }, response);
            }
            catch (Exception e)
            {
                logger.LogError("Error occured here + " + e);
                return StatusCode(500);
                

            }
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public IActionResult Put(String id, [FromBody] PostInputDto value)
        {
            if (id != value.id)
            {
                return BadRequest();
            }
            var existingImage = ImageService.getImage(id);
            if (existingImage != null)
            {
                return NotFound();
            }
            return NoContent();
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(String id)
        {
            if (ImageService.getImage(id) != null)
            {
                ImageService.deleteImage(id);
                return NoContent();
            }
            return NotFound();
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/error")]
        public IActionResult HandleError() =>
            Problem();
        }


}
