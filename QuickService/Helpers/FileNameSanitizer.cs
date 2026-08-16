using System.IO;
using System.Text.RegularExpressions;

namespace QuickService.Helpers
{
    public static class FileNameSanitizer
    {
        public static string Sanitize(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            // Remove extension to sanitize name only
            string name = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);

            // Convert to lowercase
            name = name.ToLowerInvariant();

            // Replace spaces with underscores
            name = name.Replace(" ", "_");

            // Remove special characters (only allow a-z, 0-9, and underscore)
            name = Regex.Replace(name, @"[^a-z0-9_]", "");

            // Trim multiple underscores
            name = Regex.Replace(name, @"_+", "_");

            return name + extension.ToLowerInvariant();
        }
    }
}
