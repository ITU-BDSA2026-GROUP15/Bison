using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Bison.Razor.Models;
using Bison.Razor.ModelsDTO;

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


        public Post GetPost(int id)
        {
            return _context.Posts.Find(id);
        }

        public IEnumerable<ObservationDto> GetObservations (int page)
        {
            page =Math.Max(1, page);
            int offset = (page-1) * PageSize;

            return _context.Posts.OfType<Observation>()
                .Include(o => o.Author)
                .Include(o => o.Taxon)
                .OrderByDescending(o => o.TimeStamp)
                .ThenByDescending (o => o.PostId)
                .Skip(offset)
                .Take(PageSize)
                .AsEnumerable()
                .Select(ToObservationDto) //Mapper will be created later
                .ToList();

        }

        public IEnumerable<ObservationDto> GetObservationsByAuthor (string author, int page)
        {
            page = Math.Max(1, page);
            int offset = (page-1)* PageSize;

            return _context.Posts.OfType<Observation>()
                .Include(o => o.Author)
                .Include(o => o.Taxon)
                .Where(o => o.Author.Name == author)
                .OrderByDescending(o=> o.TimeStamp)
                .ThenByDescending(o => o.PostId)
                .Skip(offset)
                .Take(PageSize)
                .AsEnumerable()
                .Select(ToObservationDto) //For later
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
        public Observation? GetObservationEntity(int id)
        {
            return _context.Posts.OfType<Observation>()
                .Include(o => o.Author)
                .Include(o => o.Taxon)
                .FirstOrDefault(o => o.PostId == id);
        }

        public ObservationDto? GetObservationDetails (int id)
        {
            var o = GetObservationEntity(id);
            if(o is null) return null;

            var dto = ToObservationDto(o);
            dto.Comments = GetComments(id).ToList();
            dto.Proposals = GetProposals(id).ToList();

            return dto;

        }
        public IEnumerable<CommentDto> GetComments (int observationId)
        {
            return _context.Posts.OfType<Comment>()
                .Include (c => c.Author)
                .Where (c=> c.ObservationId == observationId)
                .AsEnumerable()
                .Select(ToCommentDto)
                .ToList();
        }

        public IEnumerable<ProposalDto> GetProposals (int observationId)
        {
            return _context.Posts.OfType<Proposal>()
                .Include(p=> p.Author)
                .Include(p => p.Taxon)
                .Where(p => p.ObservationId == observationId)
                .AsEnumerable()
                .Select(ToProposalDto)
                .ToList();
        }



        public Taxon? GetTaxon(int id)
        {
            return _context.Taxons.OfType<Taxon>()
                .FirstOrDefault(o => o.TaxonId == id);
        }


        public List<Observation> GetAllObservations()
        {
            //loads all taxons to reference the parent taxon
            _context.Taxons.Load();

            return _context.Posts.OfType<Observation>()
                .Include(p => p.Author)
                .Include(p => p.Taxon)
                //.ThenInclude(t => t.Parent)
                .OrderByDescending(p => p.TimeStamp)
                .ToList();
        }



        public Author? GetAuthorByName(string name)
        {
            return _context.Authors.FirstOrDefault(author => author.Name == name);
        }

        public IReadOnlyList<Taxon> GetTaxons()
        {
            // Formularen skal kun læse taxonerne, så EF behøver ikke spore dem.
            return _context.Taxons
                .AsNoTracking()
                .OrderBy(taxon => taxon.VernacularName ?? taxon.dwc_TaxonID)
                .ThenBy(taxon => taxon.TaxonId)
                .ToList();
        }


        private static ObservationDto ToObservationDto (Observation o) => new ObservationDto
        {
            Id = o.PostId,
            Text= o.Text,
            TimeStamp= o.TimeStamp.ToString("yyyy-MM-dd"),
            AuthorName = o.Author.Name,
            DanishVernacularName = o.Taxon?.VernacularName ?? "",
            PictureUrl = o.PictureUrl

        };

        private static CommentDto ToCommentDto (Comment c) => new CommentDto
        {
            Id = c.PostId,
            Text = c.Text,
            TimeStamp = c.TimeStamp.ToString("yyyy-MM-dd"),
            AuthorName = c.Author.Name,

        };

        private static ProposalDto ToProposalDto (Proposal p) => new ProposalDto
        {
            Id = p.PostId,
            Text = p.Text,
            TimeStamp = p.TimeStamp.ToString("yyyy-MM-dd"),
            AuthorName = p.Author.Name,
            DanishVernacularName = p.Taxon?.VernacularName ?? "",
        };

    }
}


