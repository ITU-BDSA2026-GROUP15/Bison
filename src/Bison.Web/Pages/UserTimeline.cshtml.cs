using Bison.Core.Models;
using Bison.Core.ModelsDTO;
using Bison.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly IObservationService _service;

    public List<ObservationDto> Observations { get; set; } = new();

    // Angiver, om den indloggede bruger ser sin egen oversigt.
    public bool IsOwnTimeline { get; private set; }

    // Indeholder valgmulighederne til observationsformularen.
    public IReadOnlyList<Taxon> Taxons { get; private set; } =
        Array.Empty<Taxon>();

    public UserTimelineModel(IObservationService service)
    {
        _service = service;
    }
    //[FromQuery] læser page fra URL’en, fra starten på 1.
    public IActionResult OnGet(string author, [FromQuery]int page = 1)
    {
        Observations = _service.GetObservationsFromAuthor(author, Math.Max(1,page));

        // Sammenlign login-navnet med forfatteren i URL'en.
        IsOwnTimeline = User.Identity?.IsAuthenticated == true &&
            string.Equals(User.Identity.Name, author, StringComparison.Ordinal);

        // Hent kun taxoner, når formularen skal vises på brugerens egen oversigt.
        if (IsOwnTimeline)
        {
            Taxons = _service.GetTaxons();
        }



        return Page();
    }
}
