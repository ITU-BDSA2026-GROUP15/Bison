using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Bison.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Web.Pages;

// Kræver login, også hvis nogen sender en POST direkte til siden.
[Authorize]
public class CreateCommentModel : PageModel
{
    private readonly IObservationService _service;

    public CreateCommentModel(IObservationService service)
    {
        _service = service;
    }

    // Razor Pages læser værdierne fra formularen.
    [BindProperty]
    [Required(ErrorMessage = "Write your comment.")]
    public string Text { get; set; } = "";

    [BindProperty]
    [Range(1, int.MaxValue, ErrorMessage = "Choose an observation.")]
    public int ObservationId { get; set; }


    public IActionResult OnPost()
    {
        // Hent forfatteren fra den indloggede bruger.
        var authorName = User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(authorName))
            return Forbid();

        if (!ModelState.IsValid)
            return ShowForm();

        var email = User.FindFirstValue(ClaimTypes.Email) ?? "";

        if (!_service.TryCreateComment(authorName, email, Text, ObservationId))
        {
            ModelState.AddModelError(
                string.Empty, "Enter a comment and choose a valid observation to comment on.");

            return ShowForm();
        }

        // Vis brugerens oversigt efter gemning.
        return RedirectToPage("/ObservationDetails", new { id = ObservationId });
    }

    private PageResult ShowForm()
    {
        // Hent listen igen, så brugeren kan rette formularen.
        //Comments = _service.GetComments(ObservationId);
        return Page();
    }
}
