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

        // POST api/<ValuesController>
        [HttpPost]
        public async  Task<IActionResult> Post([FromBody] Image value)
        {
            if (!ModelState.IsValid)
            {
                var details = new ValidationProblemDetails(ModelState);
                logger.LogError(details.ToString());
                return BadRequest(details);


            }
            var response = await ImageService.addImage(value);
            return CreatedAtAction(nameof(GetById), new {id = value.id}, response);
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
    }
}
