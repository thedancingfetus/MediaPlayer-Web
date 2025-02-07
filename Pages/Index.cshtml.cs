using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

public class IndexModel : PageModel
{
    private readonly IWebHostEnvironment _environment;

    public List<string> VideoFiles { get; set; }

    public IndexModel(IWebHostEnvironment environment)
    {
        _environment = environment;
        VideoFiles = new List<string>();
    }

    public void OnGet()
    {
        var mediaPath = Path.Combine(_environment.WebRootPath, "media");
        VideoFiles = Directory.GetFiles(mediaPath).Select(Path.GetFileName).ToList();
        foreach(var file in VideoFiles)
        {
            Console.WriteLine("Video: " + file);
        }
    }

    public JsonResult OnGetJson()
    {
        var mediaPath = Path.Combine(_environment.WebRootPath, "media");
        return new JsonResult(JsonConvert.SerializeObject(Directory.GetFiles(mediaPath).Select(Path.GetFileName).ToList()));
    }
}
