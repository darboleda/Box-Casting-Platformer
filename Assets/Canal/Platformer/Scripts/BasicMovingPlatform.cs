using UnityEngine;
using System.Collections;

public class BasicMovingPlatform : MonoBehaviour
{

    public enum WrapBehavior
    {
        Hold,
        Reset,
        PingPong,
        Loop
    }

    public Rigidbody Rigidbody;

    public float OrbitsPerSecond;
    public float Radius;
    public float Offset;
    public Transform Center;
    public WrapBehavior Wrap;

    public float RotationSpeed;

    private float startTime;

    public void OnEnable()
    {
        startTime = Time.time;
    }

    private Vector3 previousPosition;
    public Vector3 Delta
    {
        get { return transform.position - previousPosition; }
    }

    private Quaternion previousRotation;
    public Quaternion RotationDelta
    {
        get { return Quaternion.Inverse(previousRotation) * transform.rotation; }
    }

    public void FixedUpdate()
    {
        previousPosition = transform.position;
        previousRotation = transform.rotation;
        float currentTime = Time.time - startTime;

        float normalized = currentTime * OrbitsPerSecond;
        int loopCount = Mathf.FloorToInt(normalized);
        float loopTime = normalized - loopCount;

        if (Wrap == WrapBehavior.Hold && loopCount > 0)
        {
            Rigidbody.velocity = ((Vector3)EvaluatePosition(1) - transform.position) / Time.deltaTime;
        }
        else if (Wrap == WrapBehavior.Reset && loopCount > 0)
        {
            Rigidbody.velocity = ((Vector3)EvaluatePosition(0) - transform.position) / Time.deltaTime;
        }
        else if (Wrap == WrapBehavior.PingPong)
        {
            Rigidbody.velocity = ((Vector3)EvaluatePosition(Mathf.PingPong(normalized, 1)) - transform.position) / Time.deltaTime;
        }
        else
        {
            Rigidbody.velocity = ((Vector3)EvaluatePosition(loopTime) - transform.position) / Time.deltaTime;
        }

        Rigidbody.angularVelocity = Vector3.forward * Mathf.Deg2Rad * RotationSpeed;
    }

    private Vector2 EvaluatePosition(float t)
    {
        if (Center == null) return transform.position;

        Vector3 position = Center.position;
        float x = Radius * Mathf.Cos((Offset + t) * Mathf.PI * 2);
        float y = Radius * Mathf.Sin((Offset + t) * Mathf.PI * 2);

        return new Vector3(position.x + x, position.y + y, position.z);
    }
}
