using Microsoft.EntityFrameworkCore;
using Bison.Razor.Models;

namespace Bison.Razor.DAL {

public class PostContext : DbContext
{
    public PostContext(DbContextOptions<PostContext> options) : base(options)
    {
    }

    public DbSet<Post> Posts { get; set; }
    public DbSet<Observation> Observations { get; set; }
    public DbSet<Proposal> Proposals { get; set; }
}
}
