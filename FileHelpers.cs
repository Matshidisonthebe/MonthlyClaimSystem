using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace MonthlyClaimSystem.Helpers
{
    public static class FileHelpers
    {
        public static readonly long MaxBytes = 5 * 1024 * 1024; // 5 MB
        private static readonly HashSet<string> permittedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf",
            ".docx",
            ".xlsx"
        };

        public static bool IsAllowedFile(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName);
            return !string.IsNullOrEmpty(ext) && permittedExtensions.Contains(ext);
        }

        public static async Task<string> SaveFileAsync(IFormFile file, string uploadsFolder)
        {
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var safeFileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(uploadsFolder, safeFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fullPath;
        }
    }
}