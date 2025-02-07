using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class ManageMediaModel : PageModel
{
    private readonly IWebHostEnvironment _environment;

    public ManageMediaModel(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public List<string> MediaFiles { get; set; }

    public void OnGet()
    {
        var mediaPath = Path.Combine(_environment.WebRootPath, "media");
        MediaFiles = Directory.GetFiles(mediaPath).Select(Path.GetFileName).ToList();
    }

    public async Task<IActionResult> OnPostAsync(IFormFile mediaFile)
    {
        if (mediaFile != null)
        {
            var filePath = Path.Combine(_environment.WebRootPath, "media", mediaFile.FileName);

            // Ensure the media directory exists
            Directory.CreateDirectory(Path.Combine(_environment.WebRootPath, "media"));

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await mediaFile.CopyToAsync(stream);
                }

                // Optionally transcode the uploaded file
                //TranscodeVideo(filePath);

                return RedirectToPage(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception and provide feedback to the user
                Console.WriteLine($"File upload failed: {ex.Message}");
                return RedirectToPage(new { success = false, error = ex.Message });
            }
        }
        return RedirectToPage(new { success = false });
    }


    public IActionResult OnPostDelete(string fileName)
    {
        if (!string.IsNullOrEmpty(fileName))
        {
            var filePath = Path.Combine(_environment.WebRootPath, "media", fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        return RedirectToPage();
    }

    private void TranscodeVideo(string inputFilePath)
    {
        var mediaPath = Path.Combine(_environment.WebRootPath, "media");
        var outputMp4 = Path.Combine(mediaPath, Path.GetFileNameWithoutExtension(inputFilePath) + ".mp4");

        var ffmpegPath = "./ffmpeg.exe"; // Update with the correct path to FFmpeg executable

        var commands = new List<string>
        {
            $"-i \"{inputFilePath}\" \"{outputMp4}\""
        };

        foreach (var command in commands)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ffmpegPath,
                    Arguments = command,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            process.WaitForExit();
        }
    }
}
