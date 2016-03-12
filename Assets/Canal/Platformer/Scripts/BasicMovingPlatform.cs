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

    private float startTime;

    public void Awake()
    {
        //UpdateOrderController.Controller.RegisterFixedUpdate(this, _FixedUpdate);
    }

    public void OnEnable()
    {
        startTime = Time.time;
    }

    private Vector3 previousPosition;
    public Vector3 Delta
    {
        get { return transform.position - previousPosition; }
    }

    public void FixedUpdate()
    {
        previousPosition = transform.position;
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
    }

    private void SetPosition(float t)
    {
        Vector3 position = Center.position;
        float x = Radius * Mathf.Cos((Offset + t) * Mathf.PI * 2);
        float y = Radius * Mathf.Sin((Offset + t) * Mathf.PI * 2);

        transform.position = (new Vector3(position.x + x, position.y + y, position.z));
    }
}
