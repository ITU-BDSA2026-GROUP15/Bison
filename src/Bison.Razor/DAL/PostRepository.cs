using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Bison.Razor.Models;

namespace Bison.Razor.DAL
{
    public class PostRepository : IPostRepository
    {
        //database context is defined as a variabel 
        private PostContext _context;

        public PostRepository(PostContext context)
        {
            _context = context;
        }

        //create
        public void InsertPost(Post post)
        {
            _context.Posts.Add(post);
        }

        //read
        public IEnumerable<T> GetPosts<T>() where T : Post
        {
            return _context.Posts.OfType<T>().ToList();
        }

        public Post GetPost(int id)
        {
            return _context.Posts.Find(id);
        }

        public IEnumerable<T> GetPostsByAuthor<T>(string authorName) where T : Post
        {
            //refaktoriseres senere
            return _context.Posts
                .OfType<T>()
                .Where(p => p.Author.Name == authorName)
                .ToList();
        }

        public List<Observation> GetObservationsFromAuthor(string author, int page = 1)
        {
            return _repo.GetPostsByAuthor<Observation>(author, page, pageSize).ToList();
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

    }
}