using Bison.Core.ModelsDTO;
using Bison.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
//using Bison.Application.Interfaces; does not work for the implementation for the async


namespace Bison.Web.Pages;

[Authorize]
public class FavouriteModel : PageModel
{
     private readonly IObservationService _service;

     public List<ObservationDto> Observations {get; set;} = new();

     public FavouriteModel (IObservationService service)
    {
        _service =service;
    }

    public async Task<IActionResult> OnGet()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId != null)
        {
            return Challenge();
        }

        Observations = _service.GetFavouriteObservations(currentUserId);

        await _service.AddLikeStatusesAsync(Observations, currentUserId);
        return Page();

    }

    public async Task<IActionResult> OnPostLikeAsync(int observationId, bool isLiked)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(userId != null)
        {
            return Challenge();
        }

        var success = await _service.SetLikeAsync(
            observationId, userId, isLiked
        );

        if (!success)
        {
            return NotFound();
        }
        return RedirectToPage();
    }
}