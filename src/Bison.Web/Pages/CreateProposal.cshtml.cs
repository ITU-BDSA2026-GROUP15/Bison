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
public class CreateProposalModel : PageModel
{
    private readonly IObservationService _service;

    public IReadOnlyList<Taxon> Taxons { get; set; } = new List<Taxon>();

    public CreateProposalModel(IObservationService service)
    {
        _service = service;
    }

    // Razor Pages læser værdierne fra formularen.
    [BindProperty]
    [Required(ErrorMessage = "Write your proposal.")]
    public string Text { get; set; } = "";

    [BindProperty]
    [Range(1, int.MaxValue, ErrorMessage = "Choose an observation.")]
    public int ObservationId { get; set; }

    [BindProperty]
    [Range(1, int.MaxValue, ErrorMessage = "Choose a taxon.")]
    public int taxonId { get; set; }

    public IActionResult OnPost()
    {
        // Hent forfatteren fra den indloggede bruger.
        var authorName = User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(authorName))
            return Forbid();

        if (!ModelState.IsValid)
            return ShowForm();

        var email = User.FindFirstValue(ClaimTypes.Email) ?? "";

        if (!_service.TryCreateProposal(authorName, email, Text, ObservationId, taxonId))
        {
            ModelState.AddModelError(
                string.Empty, "Enter a proposal and choose a valid taxon id.");

            return ShowForm();
        }

        // Vis brugerens oversigt efter gemning.
        return RedirectToPage("/ObservationDetails", new { id = ObservationId });
    }

    private PageResult ShowForm()
    {
        // Hent listen igen, så brugeren kan rette formularen.
        Taxons = _service.GetTaxons();
        return Page();
    }
}
