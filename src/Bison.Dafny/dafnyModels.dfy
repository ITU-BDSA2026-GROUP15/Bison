
module Models {
    class {:extern "Bison.Razor.Models.DafnyTaxon"} Taxon {
        function {:extern} {:axiom} isSubTaxon(ancestor: Taxon): bool
    }

    class {:extern "Bison.Razor.Models.DafnyObservation"} Observation {
        function {:extern} {:axiom} getTaxon(): Taxon
    }
}