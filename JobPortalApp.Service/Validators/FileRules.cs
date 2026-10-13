using Microsoft.AspNetCore.Http;

namespace JobPortalApp.Service.Validators;

public static class FileRules
{
    public const long MaxImageSizeInBytes = 2 * 1024 * 1024;
    public const long MaxPdfSizeInBytes = 5 * 1024 * 1024;

    private static readonly string[] ImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public static bool IsImage(IFormFile? file) =>
        file is not null && ImageExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant());

    public static bool IsPdf(IFormFile? file) =>
        file is not null && Path.GetExtension(file.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase);
}
