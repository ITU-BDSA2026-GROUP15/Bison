using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Bison.Razor.Models;

namespace Bison.DAL
{
    //IDisposable -> disposes the database context
    public class PostRepository : IPostRepository, IDisposable
    {
        //database context is defined as a variabel 
        private PostContext context;

        public PostRepository(PostContext context)
        {
            this.context = context;
        }

        //create
        public void InsertPost(Post post)
        {
            context.Students.Add(student);
        }

        //read
        public IEnumerable<Post> GetPosts()
        {
            return context.Students.ToList();
        }

        public Post GetPosts(int id)
        {
            return context.Posts.Find(id);
        }

        //update
        public void UpdteStudent(Post post)
        {
            context.Entry(post).State = EntityState.Modified;
        }

        public void DeletePost(int id)
        {
            Post post = contect.Posts.Find(id);
            context.Posts.Remove(post);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose (bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    public class PostRepository : IPostRepository
{
    // DBFacade håndterer adgangen til SQLite-databasen.
    private readonly DBFacade _db;

    // Modtager DBFacade gennem dependency injection.
    public ObservationService(DBFacade redbpo)
    {
        _db = db;
    }

    public List<ObservationViewModel> GetObservations(int page = 1)
    {
        // Sender sidetallet videre til databasekoden.
        return _db.GetObservations(page);
    }

    public List<ObservationViewModel> GetObservationsFromAuthor(
        string author, int page = 1)
    {
        // Sender både forfatter og sidetal videre til databasekoden.
        return _db.GetObservationsFromAuthor(author, page);
    }

    public List<ObservationViewModel> GetObservationDetails (
        int id, int page = 1)
    {
        return _db.GetObservationDetails(id, page);
    }

     public List<ObservationViewModel> GetProposals (
        int id, int page = 1)
    {
        return _db.GetProposals(id, page);
    }

     public List<ObservationViewModel> GetComments (
        int id, int page = 1)
    {
        return _db.GetComments(id, page);
    }
}

    }
}