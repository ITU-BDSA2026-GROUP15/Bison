using Bison.Core.Models;
using Bison.Core.ModelsDTO;
using Bison.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bison.Web.Pages;

public class ObservationDetailsModel : PageModel
{
    private readonly IObservationService _service;

    //der returneres kun en enkelt observation så den behøver i princippet ikke være en liste
    public ObservationDto? ObservationDetails { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
    public List<ProposalDto> Proposals { get; set; } = new();
    public int ObservationId { get; set; }
public IReadOnlyList<Taxon> Taxons { get; set; } = new List<Taxon>();

    public ObservationDetailsModel(IObservationService service)
    {
        _service = service;
    }
    //[FromQuery] læser page fra URL’en, fra starten på 1.
    public ActionResult OnGet(int? id)
    {
            if (id is null)
        {
            return RedirectToPage("/Public");
        } 

            
            ObservationDetails = _service.GetObservationDetails(id.Value);

            if (ObservationDetails is null)
        {
            return NotFound();
        }

        // GetObservationDetails already includes comments and proposals.
            Comments = ObservationDetails.Comments;
            Proposals = ObservationDetails.Proposals;
            ObservationId = id.Value;
            Taxons = _service.GetTaxons();

            return Page();
        }
    }
