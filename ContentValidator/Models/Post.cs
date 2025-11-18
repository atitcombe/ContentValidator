namespace ContentValidator.Models
{
    public sealed class Post
    {
        public required string id { get; set; }
        public required string TextContent { get; set; }
        public string? BlobReferenceLink { get; set; }
    }
}
