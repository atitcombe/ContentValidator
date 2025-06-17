using ContentValidator.Models;
using ContentValidator.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContentValidator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController(IImageService ImageService, ILogger<ImageController> logger) : ControllerBase
    {
       

        // GET api/<ValudatorController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Image>> GetById(String id)
        {
            var image = await ImageService.getImage(id);
            logger.LogInformation("the gotten image from controller is: " + image);
            if (image == null)
            {
                return NotFound();
            }
            return image;
        }

        // Updated POST method to fix CS0019 error
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Image value)
        {
            logger.LogInformation("Post request made at {DT}",
           DateTime.UtcNow.ToLongTimeString());
            try
            {
                if (!ModelState.IsValid)
                {
                    var details = new ValidationProblemDetails(ModelState);
                    logger.LogError(details.ToString());
                    return BadRequest(details);
                }

                var response = await ImageService.addImage(value);
                if ((int)response.StatusCode >= 400)
                {
                    logger.LogError(response.ToString());
                }

                return CreatedAtAction(nameof(GetById), new { id = value.id }, response);
            }
            catch (Exception e)
            {
                logger.LogError("Error occured here + " + e);
                throw new Exception(e.Message);
                

            }
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public IActionResult Put(String id, [FromBody] Image value)
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
