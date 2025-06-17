using System.ComponentModel.DataAnnotations;

namespace ContentValidator.Models;

public class Image
{
    public required string id { get; set; }
    public string? Name { get; set; }

    [MinLength(4)]
    public required string Description { get; set; }
    public string? Type { get; set; }
    public string? Base64Encoding { get; set; }
}
