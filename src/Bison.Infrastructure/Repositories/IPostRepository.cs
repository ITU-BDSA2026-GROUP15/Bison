using Bison.Core.Models;
using Bison.Core.ModelsDTO;

namespace Bison.Infrastructure.Repositories
{
    public interface IPostRepository : IDisposable
    {

        //implements methods using CRUD

        //create
        void InsertPost(Post post);

       
        IEnumerable<ObservationDto> GetObservations (int page);

        IEnumerable<ObservationDto> GetObservationsByAuthor (string author, int page);
        ObservationDto? GetObservationDetails(int id);
        IEnumerable<CommentDto> GetComments (int observationId);
        IEnumerable<ProposalDto> GetProposals (int observationId);

        IEnumerable <ObservationDto> GetFavouriteObservations (string userId);
        Observation? GetObservationEntity(int id);
        Taxon? GetTaxon(int id);
        // Finder den forfatter, der svarer til brugerens login-navn.
        Author? GetAuthorByName (string name);
        IReadOnlyList<Taxon>GetTaxons();





         //update
        void UpdatePost(Post post);

        //delete
        void DeletePost(int post);

        Task<Dictionary<int, (int LikeCount, bool IsLikedByCurrentUser)>> GetLikeStatusesAsync(
            int[] observationIds,
            string? currentUserId);

        Task<bool> SetLikeAsync(int observationId, string userId, bool isLiked);

        void Save();

    
       //read -> retuans a single post
        Post GetPost(int id);
       List<Observation> GetAllObservations();

    }

}
