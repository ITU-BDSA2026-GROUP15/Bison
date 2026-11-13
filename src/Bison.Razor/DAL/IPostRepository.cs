using System;
using System.Collections.Generic;
using Bison.Razor.Models;
using Bison.Razor.ModelsDTO;

namespace Bison.Razor.DAL
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

     
        Observation? GetObservationEntity(int id);
        Taxon? GetTaxon(int id);
        Author? GetAuthorByName (string name);
        IReadOnlyList<Taxon>GetTaxons();



       // Finder den forfatter, der svarer til brugerens login-navn.
        Author? GetAuthorByName(string name);

         //update
        void UpdatePost(Post post);

        //delete
        void DeletePost(int post);

        void Save();

        
       IEnumerable<T> GetPosts<T>(int page) where T : Post;
       //read -> retuans a single post
        Post GetPost(int id);
       List<Observation> getAllObservations();

    }

}
