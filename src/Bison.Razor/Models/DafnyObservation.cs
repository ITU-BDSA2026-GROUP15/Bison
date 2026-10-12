namespace Bison.Razor.Models;

// C#-implementeringen af {:extern}-klassen Observation fra src/Bison.Dafny/dafnyModels.dfy.
public class DafnyObservation
{
    // Den originale observation.
    public Observation Observation { get; }

    public DafnyObservation(Observation observation)
    {
        // Dafny-typer kan aldrig være null, så vi fejler tidligt (i konstruktøren), hvis der er en fejl.
        Observation = observation ?? throw new ArgumentNullException(nameof(observation));
    }

    // Kaldes af den genererede FilterBy-kode. Kaster en fejl, hvis Taxon ikke er indlæst fra databasen
    // (fx med .Include(o => o.Taxon)), i stedet for at returnere null - null er ikke en gyldig Dafny-værdi.
    public DafnyTaxon getTaxon()
    {
        return new DafnyTaxon(Observation.Taxon);
    }
}