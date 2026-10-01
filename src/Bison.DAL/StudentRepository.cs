using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Bison.Razor.Models;

namespace Bison.DAL
{
    public class PostRepository : IPostRepository, IDisposable
    {
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

    }
}