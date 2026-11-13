using Microsoft.AspNetCore.Http;

namespace Bison.Infrastructure.Services;

//this interface is for the LocalPictureStorage
public interface IPictureStorage
{
    bool IsValid(IFormFile file);

    Task<string> SaveAsync(IFormFile file);
}
