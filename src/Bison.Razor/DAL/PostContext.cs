using Microsoft.EntityFrameworkCore;
using Bison.Razor.Models;

namespace Bison.Razor.DAL {

public class PostContext : DbContext
{
    public PostContext(DbContextOptions<PostContext> options) : base(options)
    {

    }

    public DbSet<Post> Posts { get; set; } = null;
    public DbSet<Observation> Observations { get; set; } = null;
    public DbSet<Proposal> Proposals { get; set; } = null;

    public Dbset<Comment> Comments { get; set; } = null;

    public DbSet<Author> Authors { get; set; } = null;
    public DbSet<Taxon>  Taxons { get; set; } = null;

    protected override void OnModelCreating(modelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        //Table-per-Hierachy - this is used so we won't need three seperate tables for Observations, comments and proposals
        //Instead we create one table in which we will have a "PostType" which show what kind of posts we are dealing with
        modelBuilder.Entity<post>()
            .HasDiscriminator<string>("PostType")
            .HasValue<Observation>("observation")
            .HasValue<Comment>("Comment")
            .HasValue<Proposal>("Proposal");

        
        //Author-Posts relationsship: 
        //
        modelBuilder.Entity<post>()
            .HasOne(p => p.Author) //Each post has one related thing accessed by the Author, aka one-to-one
            .WithMany(a => a.post) //Each Author has a related thing to a collection of posts, aka one-to-many
            .HasForeignKey(p => p.AuthorID); //Using author as a navigation property
        
        
        //Taxon to Taxon relationship

        modelBuilder.Entity<taxon>()
            .HasOne(t => t.Parent) //One-To-One
            .WithMany(t => t.Children) //One-to-Many
            .HasForeignKey(t => t.ParentId)
            .OnDelete(DeleteBehavior.Restrict); //Avoids cascading deletions 
        
        // Observations to Taxons
        modelBuilder.Entity<Observation>()
            .HasOne(o => o.Taxon)
            .WithMany()
            .HasForeignKey(p => p.TaxonId)
        
        
            
            
        
    }

}
