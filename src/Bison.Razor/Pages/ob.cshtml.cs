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

public class OBModel : PageModel
{
    private readonly IObservationService _service;
    public List<ObservationViewModel> Observations { get; set; }

    public OBModel(IObservationService service)
    {
        _service = service;
    }

//Razor-siden til at læse sidetallet fra URL’en og sende det videre.
    //[FromQuery] læser page fra URL’en,
    public ActionResult OnGet([FromQuery] int page=1){
        Observations = _service.GetObservations(page);
        return Page();
    }
}
