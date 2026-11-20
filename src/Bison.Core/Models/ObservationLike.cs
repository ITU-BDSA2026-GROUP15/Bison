

namespace Bison.Core.Models;



//et obervationLike bliver koblet til en person(med et userId) og en Observation.

public class ObservationLike 
{
     // Fremmednøgle til den observation, der er liket.
    public int ObservationId{get;set;}

    // Navigationsegenskab, som EF Core kan bruge til at hente observationen.
    public Observation Observation{get;set;}=null!;

    // ID på den bruger, der gav liket.
    public string UserId {get;set;}=string.Empty;
}