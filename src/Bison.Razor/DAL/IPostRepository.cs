using System;
using System.Collections.Generic;
using Bison.Razor.Models;

namespace Bison.Razor.DAL
{
    public interface IPostRepository : IDisposable
    {

        //implements methods using CRUD

        //create
        void InsertPost(Post post);

        //read -> returns all posts
        IEnumerable<T> GetPosts<T>() where T : Post;

        //read -> retuans a single post
        Post GetPost(int id);
        
        //update
        void UpdatePost(Post post);

        //delete
        void DeletePost(int post);

        void Save();
    }

}
