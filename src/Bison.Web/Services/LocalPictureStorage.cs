namespace Bison.Web.Services;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

//saves the picture in wwwroot/uploads folder
public class LocalPictureStorage : IPictureStorage
{
    private const long MaxSize = 5 * 1024 * 1024; //this is the size of 5 mb
    private static readonly string[] AllowedTypes = {"image/jpeg", "image/png", "image/webp", "image/heic"};

    //gives us the path to wwwroot/uploads later, can differ if started on azure as i understand it
    private readonly IWebHostEnvironment _env;

    public LocalPictureStorage(IWebHostEnvironment env)
    {
        _env = env;
    }

    //checks if the form is valid, both in file size and type, true if allowed
    public bool IsValid(IFormFile file)
    {
        return file.Length > 0
               && file.Length <= MaxSize
               && AllowedTypes.Contains(file.ContentType);
    }

    //saves and return as url for instance /uploads/abc.jpg
    public async Task<string> SaveAsync(IFormFile file)
    {
        //makes the path to wwwroot/uploads, and creates the folder, if it does not exist
        var folder = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(folder);

        //makes a unique name and combines it with the path from above
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var path = Path.Combine(folder, fileName);

        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream);

        return "/uploads/" + fileName;
    }
}
