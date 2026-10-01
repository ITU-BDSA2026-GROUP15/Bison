using System;
using System.Collections.Generic;
using Bison.Razor.Models;

namespace Bison.DAL.Interface
{
    public interface IPostRepository : IDisposable
    {

        //implements methods using CRUD

        //create
        void InsertPost(Post post);

        //read -> returns all posts
        IEnumerable<Post> GetPosts();

        //read -> retuans a single post
        Post GetPost(int id);
        
        //update
        void UpdatePost(Post post);

        //delete
        void DeletePost(int post);

        void Save();
    }

}
