namespace Bison.Web.Services;

using Microsoft.AspNetCore.Http;

//this interface is for the LocalPictureStorage
public interface IPictureStorage
{
    bool IsValid(IFormFile file);

    Task<string> SaveAsync(IFormFile file);
}
