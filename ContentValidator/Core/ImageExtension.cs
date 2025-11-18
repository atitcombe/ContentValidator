namespace ContentValidator.Core
{
    public static class ImageExtension
    {
        public static bool HasValidFileExtension(this IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            return string.IsNullOrEmpty(ext) || Constants.Allowed_Extension.Contains(ext);
        }

        public static bool IsInFileSizeMinimimum(this IFormFile file)
        {
            return file.Length <= Constants.MAX_FILE_UPLOAD;
        }
    }
}
