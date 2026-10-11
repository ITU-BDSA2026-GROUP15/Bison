
function FilterBy ( root: Taxon, obs: seq<Observation>): seq<Observation>
ensures forall o :: o in FilterBy (root, obs) ==> oin obs && o.getTaxon().isSubTaxon(root)
ensures forall o :: o in obs && o.getTaxon().isSubTaxon(root) ==> o in FilterBy(root, obs)
ensures |FilterBy(root, obs) | <= |obs|

{
if|obs| == 0 then []
else
(if obs[0].getTaxon().isSubTaxon(root) then [
