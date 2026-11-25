using Bison.Core.ModelsDTO;
using Bison.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

using Bison.Core.Models;

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
    // isLiked er den ønskede tilstand efter klikket:
    // true betyder, at liket skal være gemt, og false betyder, at det skal fjernes.
    public async Task<bool> SetLikeAsync(int observationId, string userId, bool isLiked)
{
    // Tjek, at ID'et peger på en eksisterende observation.
    // AnyAsync tjekker, om observationen findes, uden at hente hele objektet.
    var observationExists = await _context.Observations
        .AnyAsync(observation => observation.PostId == observationId);

    if (!observationExists)
    {
         // Giv den kaldende kode besked om, at observationen ikke findes.
        return false;
    }
    // Slå liket op med den sammensatte primærnøgle.
    // Rækkefølgen skal matche BisonDBContext: ObservationId først, UserId bagefter.
    // Find brugerens eget like ved hjælp af den sammensatte nøgle.
    var existingLike = await _context.ObservationLikes
        .FindAsync(observationId, userId);

    if (isLiked && existingLike is null)
    {
        // Brugeren vil like observationen og har endnu ikke liket den.
        _context.ObservationLikes.Add(new ObservationLike
        {
            ObservationId = observationId,
            UserId = userId
        });
    }
    // Brugeren vil fjerne sit like. Markér det eksisterende like til sletning.
    else if (!isLiked && existingLike is not null)
    {
        _context.ObservationLikes.Remove(existingLike);
    }
    // Hvis liket allerede har den ønskede tilstand, er der intet at ændre.
    // Ellers skriver SaveChangesAsync tilføjelsen eller sletningen til databasen.
    await _context.SaveChangesAsync();
    // Observationen findes, og liket har nu den ønskede tilstand.
    return true;
    }
}