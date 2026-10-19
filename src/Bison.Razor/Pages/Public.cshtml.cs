using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Bison.Razor.Pages;
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

    //public List<ObservationViewModel> FilteredObservations { get; set; }


    public PublicModel(IObservationService service, IFilterService filterService)
    {
        _service = service;
        _filterService = filterService;
    }

//Razor-siden til at læse sidetallet fra URL’en og sende det videre.
    //[FromQuery] læser page fra URL’en,
    public ActionResult OnGet([FromQuery] int page=1, [FromQuery] int? taxonId = null){
        
        Observations = taxonId.HasValue            
            ? _filterService.GetObservationsFromTaxon(taxonId.Value, page)
            : _service.GetObservations(page);
    
        
        return Page();
    }
}
