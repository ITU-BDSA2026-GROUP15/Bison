namespace Bison.Razor.Models;

// C#-implementeringen af {:extern}-klassen Taxon fra src/Bison.Dafny/dafnyModels.dfy.
public class DafnyTaxon
{
    // Den "rigtige" EF-taxon, som denne Dafny-taxon repræsenterer.
    public Taxon Taxon { get; }

    public DafnyTaxon(Taxon taxon)
    {
        // Dafny-typer kan aldrig være null, så vi fejler tidligt (i konstruktøren), hvis der er en fejl.
        Taxon = taxon ?? throw new ArgumentNullException(nameof(taxon));
    }

    public bool isSubTaxon(DafnyTaxon ancestor)
    {
        Taxon? current = this.Taxon; 

        while (current != null)                    
        {
           if (current.TaxonId == ancestor.Taxon.TaxonId) return true;
            
            current = current.Parent;             
        }
        return false;
    }
}