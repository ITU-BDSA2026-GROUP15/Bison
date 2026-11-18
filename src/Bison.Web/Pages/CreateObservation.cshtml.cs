using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Bison.Core.Models;
using Bison.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Web.Pages;

// Kræver login, også hvis nogen sender en POST direkte til siden.
[Authorize]
public class CreateObservationModel : PageModel
{
    private readonly IObservationService _service;
    private readonly IPictureStorage _pictureStorage;

    public CreateObservationModel(IObservationService service,  IPictureStorage pictureStorage)
    {
        _service = service;
        _pictureStorage = pictureStorage;
    }

    // Razor Pages læser værdierne fra formularen.
    [BindProperty]
    [Required(ErrorMessage = "Write what you observed.")]
    public string Text { get; set; } = "";

    [BindProperty]
    [Range(1, int.MaxValue, ErrorMessage = "Choose a taxon.")]
    public int TaxonId { get; set; }

    [BindProperty]
    public IFormFile? Picture { get; set; }

    public IReadOnlyList<Taxon> Taxons { get; private set; } =
        Array.Empty<Taxon>();

    public void OnGet()
    {
        Taxons = _service.GetTaxons();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Hent forfatteren fra den indloggede bruger.
        var authorName = User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(authorName))
            return Forbid();

        if (!ModelState.IsValid)
            return ShowForm();

        var email = User.FindFirstValue(ClaimTypes.Email) ?? "";

        string? pictureUrl = null;

        if (Picture != null)
        {
            if (!_pictureStorage.IsValid(Picture))
            {
                ModelState.AddModelError(nameof(Picture), "The picture must be JPEG, PNG, WebP or HEIC and at most 5 MB");
                return ShowForm();
            }

            pictureUrl = await _pictureStorage.SaveAsync(Picture);
        }

        if (!_service.TryCreateObservation(authorName, email, Text, TaxonId, pictureUrl))
        {
            ModelState.AddModelError(
                string.Empty, "Enter an observation and choose a valid taxon.");

            return ShowForm();
        }

        // Vis brugerens oversigt efter gemning.
        return RedirectToPage("/UserTimeline", new { author = authorName });
    }

    private PageResult ShowForm()
    {
        // Hent listen igen, så brugeren kan rette formularen.
        Taxons = _service.GetTaxons();
        return Page();
    }
}
