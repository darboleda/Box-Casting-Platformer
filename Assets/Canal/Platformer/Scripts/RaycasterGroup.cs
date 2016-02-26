using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RaycasterGroup : Raycaster
{
    public Raycaster[] SubCasters;

    public override IEnumerable<RaycastHit> Cast(Vector3 delta, Vector3 direction, float scale = 1f)
    {
        return SubCasters.SelectMany(x => x.Cast(delta, direction, scale));
    }
}
