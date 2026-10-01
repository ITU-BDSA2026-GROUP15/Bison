using System;
using System.Collections.Generic;
using System.Linq;
using Models;

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
            context.Post.Add(post);
        }

        //read
        public IEnumerable<Post> GetPosts()
        {
            return context.Post.ToList();
        }

        public Post GetPost(int id)
        {
            return context.Posts.Find(id);
        }

        //update
        public void UpdatePost(Post post)
        {
            context.Entry(post).State = EntityState.Modified;
        }

        public void DeletePost(int id)
        {
            Post post = context.Posts.Find(id);
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

    }
}