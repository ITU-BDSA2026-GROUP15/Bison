using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Razor.Pages;

public class DetailModel : PageModel
{
    private readonly IObservationService _service;

    //der returneres kun en enkelt observation så den behøver i princippet ikke være en liste
    public List<ObservationViewModel> ObservationDetails { get; set; }
    public List<ObservationViewModel> Comments { get; set; }
    public List<ObservationViewModel> Proposals { get; set; }


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
            Comments = _service.GetComments(id.Value, page);
            Proposals = _service.GetProposals(id.Value, page);
        }
        return Page();
    }
}
