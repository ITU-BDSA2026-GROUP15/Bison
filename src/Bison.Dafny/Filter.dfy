include "dafnyModels.dfy" //import statement 

module Filtering {
    import opened Models //when imported we don't have to write Models.Taxon

    //looks for a root-taxon and creates a sequence of observations that fit that
    function FilterBy ( root: Taxon, obs: seq<Observation>): seq<Observation>
        reads root, obs, set o | o in obs :: o

    //verfification steps, ensuring every relevant observation is included
    ensures forall o :: o in FilterBy (root, obs) ==> o in obs && o.getTaxon().isSubTaxon(root)
    ensures forall o :: o in obs && o.getTaxon().isSubTaxon(root) ==> o in FilterBy(root, obs)
    ensures |FilterBy(root, obs) | <= |obs|

    {
    if|obs| == 0 then [] //Best case: if the input sequence is empty, the result is the empty sequence []
    
    //if the element is verified, add to sequence, otherwise add to empty sequence
    //sequence contains one element, and the sequences are then added together
    else
    (if obs[0].getTaxon().isSubTaxon(root) then [obs[0]] else []) +
    FilterBy(root, obs[1..])

    }
}