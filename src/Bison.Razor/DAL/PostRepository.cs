using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Bison.Razor.Models;

//communicates with the database and returns psot objects determned by the generic type <T>

namespace Bison.Razor.DAL
{
    public class PostRepository : IPostRepository
    {
        //database context is defined as a variabel
        
        private const int PageSize = 32;
        private BisonDBContext _context;
        

        public PostRepository(BisonDBContext context)
        {
            _context = context;
        }



        //create
        public void InsertPost(Post post)
        {
            _context.Posts.Add(post);
        }

        //read
        public IEnumerable<T> GetPosts<T>(int page) where T : Post
        {
            page = Math.Max(1, page);
            int offset = (page - 1) * PageSize;

            return _context.Posts.OfType<T>()
                .Include(p => p.Author)
                .OrderByDescending(p => p.TimeStamp)
                .Skip(offset)
                .Take(PageSize)
                .ToList();
        }

        public Post GetPost(int id)
        {
            return _context.Posts.Find(id);
        }

        public IEnumerable<T> GetPostsByAuthor<T>(string author, int page) where T : Post
        {
            page = Math.Max(1, page);
            int offset = (page - 1) * PageSize;

            return _context.Posts.OfType<T>()
                .Include(p => p.Author)
                .Where(p => p.Author.Name == author)
                .OrderByDescending(p => p.TimeStamp)
                .Skip(offset)
                .Take(PageSize)
                .ToList();
        }


        //update
        public void UpdatePost(Post post)
        {
            _context.Entry(post).State = EntityState.Modified;
        }

        public void DeletePost(int id)
        {
            Post post = _context.Posts.Find(id);
            if ( post != null)
            {
                _context.Posts.Remove(post);
            }

        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose (bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        //Implementation of GetObservationDetails, getComments, GetProposals
        // from interface methods.
        public Observation? GetObservationDetails(int id)
        {
            return _context.Posts.OfType<Observation>()
                .Include(o => o.Author)
                .Include(o => o.Taxon)
                .FirstOrDefault(o => o.PostId == id);
        }

        public IEnumerable<Comment> GetComments(int observationId)
        {
            return _context.Posts.OfType<Comment>()
                .Include (c => c.Author)
                .Where (c => c.ObservationId == observationId)
                .ToList();

        }

        public IEnumerable<Proposal> GetProposals(int observationId)
        {
            return _context.Posts.OfType<Proposal>()
                .Include(p => p.Author)
                .Include(p => p.Taxon)
                .Where(p => p.ObservationId == observationId)
                .ToList();

        }

    }
}
