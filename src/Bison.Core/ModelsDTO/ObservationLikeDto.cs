namespace Bison.Core.ModelsDTO;

public class ObservationLikeDto
{
    // ID of the observation that the like-statussen is about.
    public int ObservationId { get; set; }

    // the sum of likes on observationen.
    public int LikeCount { get; set; }

    // is true, if the logged user has liked the observationen.
    public bool IsLikedByCurrentUser { get; set; }
}