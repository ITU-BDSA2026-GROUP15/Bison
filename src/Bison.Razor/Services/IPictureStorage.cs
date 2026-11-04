namespace Bison.Razor.Services;

using Microsoft.AspNetCore.Http;

//this interface decides that it should check the photo and saves it
public interface IPictureStorage
{
    //true if allowed (not too big)
    bool IsValid(IFormFile file);

    //saves and return as url for instance /uploads/abc.jpg
    Task<string> SaveAsync(IFormFile file);
}
