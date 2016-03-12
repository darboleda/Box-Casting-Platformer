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

    public float Duration;
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

        float normalized = currentTime / Duration;
        int loopCount = Mathf.FloorToInt(normalized);
        float loopTime = normalized - loopCount;

        if (Wrap == WrapBehavior.Hold && loopCount > 0)
        {
            SetPosition(1);
        }
        else if (Wrap == WrapBehavior.Reset && loopCount > 0)
        {
            SetPosition(0);
        }
        else if (Wrap == WrapBehavior.PingPong)
        {
            SetPosition(Mathf.PingPong(normalized, 1));
        }
        else
        {
            SetPosition(loopTime);
        }

        transform.Rotate(Vector3.forward * RotationSpeed * Time.deltaTime, Space.Self);
    }

    private void SetPosition(float t)
    {
        if (Center == null) return;

        Vector3 position = Center.position;
        float x = Radius * Mathf.Cos((Offset + t) * Mathf.PI * 2);
        float y = Radius * Mathf.Sin((Offset + t) * Mathf.PI * 2);

        transform.position = (new Vector3(position.x + x, position.y + y, position.z));
    }
}
