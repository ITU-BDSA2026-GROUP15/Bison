
function FilterBy ( root: Taxon, obs: seq<Observation>): seq<Observation>
ensures forall o :: o in FilterBy (root, obs) ==> o in obs && o.getTaxon().isSubTaxon(root)
ensures forall o :: o in obs && o.getTaxon().isSubTaxon(root) ==> o in FilterBy(root, obs)
ensures |FilterBy(root, obs) | <= |obs|

{
if|obs| == 0 then [] //Best case: if the input sequence is empty, the result is the empty sequence []
else
(if obs[0].getTaxon().isSubTaxon(root) then [obs[0]] else []) +
FilterBy(root, obs[1..])

}
