using Bison.Core.ModelsDTO;
using Bison.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Bison.Infrastructure.Services;

// Henter de oplysninger, forsiden skal bruge til at vise likes.
public class ObservationLikeService
{
    private readonly BisonDBContext _context;

    // Når servicen er registreret i Program.cs, får den databasekonteksten
    // via dependency injection.
    public ObservationLikeService(BisonDBContext context)
    {
        _context = context;
    }

    // observationIds er ID'erne på observationerne på den aktuelle side.
    // currentUserId er null, hvis besøgende ikke er logget ind.
    public async Task<Dictionary<int, ObservationLikeDto>> GetLikeStatusesAsync(
        int[] observationIds, string? currentUserId)
    {
        return await _context.ObservationLikes
            // Hent kun likes for de observationer, der vises på siden.
            .Where(like => observationIds.Contains(like.ObservationId))

            // Saml alle likes for samme observation i én gruppe.
            .GroupBy(like => like.ObservationId)

            // Lav én DTO pr. gruppe. group.Key er observationens ID.
            .Select(group => new ObservationLikeDto
            {
                ObservationId = group.Key,

                // Tæl alle likes på denne observation.
                LikeCount = group.Count(),

                // Tjek, om mindst ét af likene tilhører den aktuelle bruger.
                // For en gæst er resultatet false.
                IsLikedByCurrentUser = group.Any(like => like.UserId == currentUserId)
            })

            // Kør forespørgslen, og brug observationens ID som nøgle i opslaget.
            .ToDictionaryAsync(dto => dto.ObservationId);
    }
}