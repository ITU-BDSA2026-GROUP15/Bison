include "Taxon.dfy"

class {:extern} DafnyObservation {
    function {:extern} getTaxon(): DafnyTaxon
}