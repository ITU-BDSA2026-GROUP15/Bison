using Bison.Core.Models;

namespace Bison.Infrastructure.Repositories
{
    public interface IPostRepository : IDisposable
    {

        //implements methods using CRUD

        //create
        void InsertPost(Post post);

        //read -> returns all posts
       IEnumerable<T> GetPosts<T>(int page) where T : Post;

       //read -> returns posts by a certain author
        IEnumerable<T> GetPostsByAuthor<T>(string author, int page) where T : Post;
        //read -> retuans a single post
        Post GetPost(int id);

        //update
        void UpdatePost(Post post);

        //delete
        void DeletePost(int post);

        void Save();

        Observation? GetObservationDetails(int id);
        Taxon? GetTaxon(int id);
        IEnumerable<Comment> GetComments(int ObservationId);
        IEnumerable<Proposal> GetProposals(int ObservationId);
        List<Observation> GetAllObservations();

       // Finder den forfatter, der svarer til brugerens login-navn.
        Author? GetAuthorByName(string name);

        // Henter de taxoner, brugeren kan vælge i formularen.
        IReadOnlyList<Taxon> GetTaxons();

    }

}
