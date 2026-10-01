using System;
using System.Collections.Generic;
using Bison.Razor.Models;

namespace Bison.DAL
{
    public interface IPostRepository : IDisposable
    {
        IEnumerable<Post> GetPost();

        Post GetPost(int id);

        //implements methods using CRUD

        //create
        void InsertPost(Post post);

        // read??

        
        //update
        void UpdatePost(Post post);

        //delete
        void DeletePost(int post);

        void Save();
    }

}
