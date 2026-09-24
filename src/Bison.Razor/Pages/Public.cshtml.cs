using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly IObservationService _service;
    public List<ObservationViewModel> Observations { get; set; }

    public PublicModel(IObservationService service)
    {
        _service = service;
    }

//Razor-siden til at læse sidetallet fra URL’en og sende det videre.
    //[FromQuery] læser page fra URL’en,
    public ActionResult OnGet([FromQuery] int page=1){
        Observations = _service.GetObservations();
        return Page();
    }
}
