using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class DetailModel : PageModel
{
    private readonly IObservationService _service;
    public List<ObservationViewModel> ObservationDetails { get; set; }

    public DetailModel(IObservationService service)
    {
        _service = service;
    }
    //[FromQuery] læser page fra URL’en, fra starten på 1.
    public ActionResult OnGet(int? id, [FromQuery]int page = 1)
    {
        if (id == null)
        {
            //if no ID is provided, then show all
            ObservationDetails = _service.GetObservations(page);
        } else 
        {
            ObservationDetails = _service.GetObservationDetails(id, page);
        }
        return Page();
    }
}
