using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using MonthlyClaimSystem.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace MonthlyClaimSystem.Tests
{
    [TestFixture]
    public class FileHelpersTests
    {
        [Test]
        public void IsAllowedFile_AllowsPdf_ReturnsTrue()
        {
            // Arrange - create an in-memory PDF file
            var content = new MemoryStream(new byte[] { 1, 2, 3 });
            var formFile = new FormFile(content, 0, content.Length, "file", "test.pdf")
            {
                ContentType = "application/pdf",
                Headers = new HeaderDictionary()
            };

            // Act
            var allowed = FileHelpers.IsAllowedFile(formFile);

            // Assert (FluentAssertions; avoids xUnit Assert)
            allowed.Should().BeTrue();
        }

        [Test]
        public async Task SaveFileAsync_WritesFile_ReturnsSavedPath()
        {
            // Arrange - in-memory file and a temp directory
            var bytes = new byte[] { 4, 5, 6 };
            var content = new MemoryStream(bytes);
            var formFile = new FormFile(content, 0, content.Length, "file", "mydoc.pdf")
            {
                ContentType = "application/pdf",
                Headers = new HeaderDictionary()
            };

            var tempDir = Path.Combine(Path.GetTempPath(), "MonthlyClaimSystemTests");
            Directory.CreateDirectory(tempDir);

            try
            {
                // Act
                var saved = await FileHelpers.SaveFileAsync(formFile, tempDir);

                // Assert - file exists and returned path points inside tempDir
                File.Exists(saved).Should().BeTrue();
                Path.GetDirectoryName(saved).Should().BeEquivalentTo(Path.GetFullPath(tempDir));
            }
            finally
            {
                // Cleanup
                foreach (var f in Directory.EnumerateFiles(tempDir))
                {
                    try { File.Delete(f); } catch { }
                }
                try { Directory.Delete(tempDir); } catch { }
            }
        }
    }
}