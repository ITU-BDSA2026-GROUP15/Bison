using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Bison.Web.Pages;
/*
the class Public has
- a IObservationService_service readonly - meaning the interface of the observation for/from the server.
- a list of the Observationsviewmodel, both set and get.
- the PublicModel(IObservationService service) takes a Service

all of this is made for the pagemodel, how it should look
*/

public class PublicModel : PageModel
{
    private readonly IObservationService _service;

    private readonly IFilterService _filterService;
    public List<ObservationViewModel> Observations { get; set; } = new();

    public int CurrentPage {get; set;} = 1;
    public int? CurrentTaxonId {get; set;}

    // Taxonerne, som brugeren kan vælge i formularen.
    public IReadOnlyList<Bison.Web.Models.Taxon> Taxons { get; private set; } =
        Array.Empty<Bison.Web.Models.Taxon>();

    public PublicModel(IObservationService service, IFilterService filterService)
    {
        _service = service;
        _filterService = filterService;
    }

    //Razor-siden til at læse sidetallet fra URL’en og sende det videre.
    //[FromQuery] læser page fra URL’en,
    public ActionResult OnGet([FromQuery] int page=1, [FromQuery] int? taxonId = null){
        CurrentPage = Math.Max(1, page);
        CurrentTaxonId = taxonId;

        Observations = taxonId.HasValue
            ? _filterService.GetObservationsFromTaxon(taxonId.Value, page)
            : _service.GetObservations(page);

        // Hent taxonerne til formularens dropdown, hvis brugeren er logget ind.
        // Besøgende får ikke vist formularen, så for dem forbliver listen tom.
        if (User.Identity?.IsAuthenticated == true)
        {
            Taxons = _service.GetTaxons();
        }


        return Page();
    }
}
