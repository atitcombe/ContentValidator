using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ContentValidator.Models;

public class PostInputDto
{
    public required string id { get; set; }

    [MinLength(4)]
    public required string TextContent { get; set; }

    public string? Type { get; set; }

    [BindRequired]
    public required IFormFile ImageUpload { get; init; }

    public byte[] ByteEncoding { get; set; } = new byte[4];
}
