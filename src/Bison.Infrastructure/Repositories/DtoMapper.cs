using Bison.Core.Models;
using Bison.Core.ModelsDTO;

namespace Bison.Infrastructure.Repositories;

public static class DtoMapper
{
    public static ObservationDto ToObservationDto(Observation o) => new ObservationDto
    {
        Id = o.PostId,
        Text = o.Text,
        TimeStamp = o.TimeStamp.ToString("yyyy-MM-dd HH:mm"),
        AuthorName = o.Author.Name,
        DanishVernacularName = o.Taxon?.VernacularName ?? "",
        PictureUrl = o.PictureUrl
    };

    public static CommentDto ToCommentDto (Comment c) => new CommentDto
        {
            Id = c.PostId,
            Text = c.Text,
            TimeStamp = c.TimeStamp.ToString("yyyy-MM-dd"),
            AuthorName = c.Author.Name,

        };

    public static ProposalDto ToProposalDto (Proposal p) => new ProposalDto
        {
            Id = p.PostId,
            Text = p.Text,
            TimeStamp = p.TimeStamp.ToString("yyyy-MM-dd"),
            AuthorName = p.Author.Name,
            DanishVernacularName = p.Taxon?.VernacularName ?? "",
        };


}