using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Boxcaster : Raycaster
{
    public float Length = 1f;
    public float Skin = 0.1f;
    public float DeltaSkinMultiplier = 0;
    public Vector3 Dimensions;
    public LayerMask Mask;

    [Range(-180, 180)]
    public float MinAngle;

    [Range(-180, 180)]
    public float MaxAngle;

    public bool DebugDraw = false;
    public Color DebugColor = Color.white;

    public override IEnumerable<RaycastHit> Cast(Vector3 delta, Vector3 direction, float scale = 1f)
    {
        direction.Normalize();
        float skin = Skin + Vector3.Project(delta, direction).magnitude * DeltaSkinMultiplier;
        Vector3 origin = transform.position + delta - (direction * skin);

        return Physics.BoxCastAll(origin, Dimensions * 0.5f, direction, transform.rotation, (skin + Length) * scale, Mask.value).Where(x =>
        {
            if (x.distance == 0) return false;

            float angle = Vector3.Angle(Vector3.up, x.normal);
            float sign = Mathf.Sign(Vector3.Dot(Vector3.back, Vector3.Cross(Vector3.up, x.normal)));
            float signed = angle * sign;

            return signed == Mathf.Clamp(signed, MinAngle, MaxAngle);
        });
    }
}
