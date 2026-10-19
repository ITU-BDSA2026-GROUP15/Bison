    using Bison.Razor.DAL;
    using Bison.Razor.Models;

    public interface IFilterService
    
        public List<ObservationViewModel> GetObservationsFromTaxon (Taxon root, int page = 1);

    }

    public class FilterService : IFilterService
    {
        // DBFacade håndterer adgangen til SQLite-databasen.
        private readonly IPostRepository _repo;

        // Modtager DBFacade gennem dependency injection.
        public FilterService(IPostRepository repo)
        {
            _repo = repo;
        }
    
    public List<ObservationViewModel> GetObservationsFromTaxon(Taxon root, int page = 1)
    {
        //wrapping an observation to a DafnyObservation
        var dafnyObs = _repo.GetPosts<Observation>(page).Select(obs => new DafnyObservation(obs)).ToArray();

        //converting to a dafny-sequence, which is the parameter for the wrapper class
        var input = Dafny.Sequence<DafnyObservation>.FromArray(dafnyObs);

        var dafnyRoot = new DafnyTaxon(root);
        
        //calling the wrapper, and verify the filtering
        var result = DafnyWrapper.Wrap.Filter(dafnyRoot, input);

        //unwrapping from dafnyObservation to Observation and convert to ViewModel
        return result.Elements.Select(daf => ObservationService.ToViewModel(daf.Observation)).ToList();
    }
}