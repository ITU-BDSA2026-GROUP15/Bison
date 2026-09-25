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
    public List<ObservationViewModel> Observations { get; set; }

    public PublicModel(IObservationService service)
    {
        _service = service;
    }

    public ActionResult OnGet()
    {
        Observations = _service.GetObservations();
        return Page();
    }
}
