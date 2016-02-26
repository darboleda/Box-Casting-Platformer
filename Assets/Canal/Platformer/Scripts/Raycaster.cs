using UnityEngine;
using System.Collections.Generic;

public abstract class Raycaster : MonoBehaviour
{
    public virtual Vector3 Position { get { return transform.position; } }
    public abstract IEnumerable<RaycastHit> Cast(Vector3 delta, Vector3 direction, float scale = 1f);
}
