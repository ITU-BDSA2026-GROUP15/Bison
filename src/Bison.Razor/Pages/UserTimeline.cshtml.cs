using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly IObservationService _service;
    public List<ObservationViewModel> Observations { get; set; }

    public UserTimelineModel(IObservationService service)
    {
        _service = service;
    }
    //[FromQuery] læser page fra URL’en, fra starten på 1.
    public ActionResult OnGet(string author, [FromQuery]int page = 1)
    {
        Observations = _service.GetObservationsFromAuthor(author);
        return Page();
    }
}
