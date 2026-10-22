    using Bison.Razor.DAL;
    using Bison.Razor.Models;
    using Bison.Core;

    public interface IFilterService{
        public List<ObservationViewModel> GetObservationsFromTaxon (int taxonId, int page = 1);
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

        private const int PageSize = 32;
    
    public List<ObservationViewModel> GetObservationsFromTaxon(int taxonId, int page = 1)
    {
        page = Math.Max(1, page);

        var root = _repo.GetTaxon(taxonId);
        if (root is null)
        {
            return new List<ObservationViewModel>();
        }

        //wrapping an observation to a DafnyObservation
        var dafnyObs = _repo.GetAllObservations().Select(obs => new DafnyObservation(obs)).ToArray();

        //converting to a dafny-sequence, which is the parameter for the wrapper class
        var input = Dafny.Sequence<DafnyObservation>.FromArray(dafnyObs);

        var dafnyRoot = new DafnyTaxon(root);
        
        //calling the wrapper, and verify the filtering
        var result = DafnyWrapper.Wrap.Filter(dafnyRoot, input);

        //unwrapping from dafnyObservation to Observation and convert to ViewModel
        return result.Elements
            .Skip((page-1) * PageSize)
            .Take(PageSize)
            .Select(daf => ObservationService.ToViewModel(daf.Observation))
            .ToList();
    }
}