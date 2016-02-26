using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerPhysics : MonoBehaviour {

    [System.Serializable]
    private struct FloorInfo
    {
        public GameObject Floor;
        public Vector3 FloorVector;
        public Vector3 PositionRelative;
    }

    public float WalkSpeed = 5f;
    public float WalkAcceleration = 1f;
    public float WalkFriction = 5f;

    public float JumpHeight = 3f;

    public float Gravity = 9.8f;

    public Raycaster[] FloorRays;

    private Vector3 velocity;

    public float targetXvel;
    public bool enableInput = true;
    public bool enableInfiniteJumps = false;
    public bool breakOnDrop = false;
    private bool onGround;

    private FloorInfo floor;

    public void Update()
    {
        if (enableInput)
        {
            targetXvel = Input.GetAxisRaw("Horizontal") * WalkSpeed;
            if (Input.GetAxisRaw("Vertical") < 0 && onGround)
            {
                targetXvel = 0;
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (Input.GetAxisRaw("Vertical") < 0 && onGround)
            {
                transform.Translate(Vector3.down * 0.1f);
                velocity.y = -3;
                onGround = false;
                if (breakOnDrop) Debug.Break();
            }
            else if (onGround || enableInfiniteJumps)
            {
                Jump(JumpHeight);
            }
        }
        if (Input.GetButtonUp("Jump") && velocity.y > 0)
        {
            velocity.y *= 0.25f;
            if (breakOnDrop) Debug.Break();
        }

    }

    public void LateUpdate()
    {
        // Move along with the floor
        if (onGround && floor.Floor != null)
        {
            Vector3 correctedDelta = (floor.Floor.transform.position + floor.PositionRelative) - transform.position;

            FloorInfo? info;
            correctedDelta = DetectFloor(correctedDelta, out info);
            floor = info ?? default(FloorInfo);
            if(floor.Floor != null)
            {
                floor.PositionRelative = (transform.position + correctedDelta) - floor.Floor.transform.position;
            }
            transform.Translate(correctedDelta);
        }
    }

    public void FixedUpdate()
    {
        velocity += transform.TransformVector(Vector3.down * Gravity) * Time.deltaTime;
        velocity.x = CalculateWalkSpeed(velocity.x, targetXvel);

        Vector3 uncorrectedDelta = velocity * Time.deltaTime;
        Vector3 correctedDelta = uncorrectedDelta;
        
        FloorInfo? info;
        correctedDelta = DetectFloor(correctedDelta, out info);

        onGround = false;
        if (info != null)
        {
            velocity.y = 0;
            onGround = true;
        }
        floor = info ?? default(FloorInfo);
        if (floor.Floor != null)
        {
            floor.PositionRelative = (transform.position + correctedDelta) - floor.Floor.transform.position;
        }

        foreach (Raycaster caster in FloorRays)
        {
            Debug.DrawLine(caster.Position, caster.Position + correctedDelta, Color.green, 0, false);
        }

        transform.Translate(correctedDelta);
    }

    private void Jump(float height)
    {
        velocity.y = Mathf.Sqrt(2 * height * Gravity);
    }

    private Vector3 DetectFloor(Vector3 delta, out FloorInfo? info)
    {
        info = null;

        float newDelta = float.NegativeInfinity;

        foreach (var caster in FloorRays)
        {
            FloorInfo? detected = null;
            float d = DetectFloor(delta, caster, out detected);

            if (detected != null && d > newDelta)
            {
                newDelta = d;
                info = detected;
            }
        }

        if (!float.IsInfinity(newDelta))
        {
            delta.y = newDelta;
        }
        return delta;
    }

    private float DetectFloor(Vector3 delta, Raycaster caster, out FloorInfo? info)
    {
        info = null;
        Vector3 currentPosition = caster.Position;
        Vector3 expectedPosition = currentPosition + delta;

        float bestDistance = float.NegativeInfinity;

        IEnumerable<RaycastHit> otherHits = caster.Cast(Vector3.zero, Vector3.down);
        foreach (RaycastHit hit in caster.Cast(delta, Vector3.down))
        {
            float distance = hit.point.y - expectedPosition.y;

            if (distance > bestDistance
                && (TestFloorHitForWalking(delta, caster.Position, hit, distance)
                    || TestFloorHitForLanding(delta, caster.Position, hit, otherHits)))
            {
                bestDistance = distance;
                Vector3 v = Vector3.Cross(Vector3.back, hit.normal);

                info = new FloorInfo()
                {
                    Floor = hit.collider.gameObject,
                    FloorVector = v
                };
            }
            else
            {
                Debug.DrawLine(expectedPosition, hit.point, Color.cyan, 0, false);
            }
        }
        return delta.y + bestDistance;
    }

    private bool TestFloorHitForLanding(Vector3 delta, Vector3 currentPosition, RaycastHit hit, IEnumerable<RaycastHit> currentPositionHits)
    {
        Vector3 expectedPosition = currentPosition + delta;
        Vector3 cross = Vector3.Cross(Vector3.forward, hit.normal);

        BasicMovingPlatform moving = hit.collider.gameObject.GetComponentInParent<BasicMovingPlatform>();
        Vector3 platformDelta = (moving != null ? moving.Delta : Vector3.zero);

        return hit.point.y > expectedPosition.y                                                                               // We expected to be below the point that we hit
               && (hit.point.y <= currentPosition.y                                                                           // Our current position is above the hit point after the x is applied
                   || Vector3.Dot(Vector3.Cross(currentPosition - (hit.point - platformDelta), cross),                                                           // Or we're starting from a position on the same side of the slope as its normal
                                  Vector3.Cross(hit.normal, cross)) >= -0.005f
                   || currentPositionHits.Where(x => x.collider == hit.collider && x.point.y <= currentPosition.y).Any());    // Or our current position is above the hit point before the x is applied
    }

    private bool TestFloorHitForWalking(Vector3 delta, Vector3 currentPosition, RaycastHit hit, float distance)
    {
        Vector3 cross = Vector3.Cross(Vector3.forward, hit.normal);

        BasicMovingPlatform moving = hit.collider.gameObject.GetComponentInParent<BasicMovingPlatform>();
        Vector3 platformDelta = (moving != null ? moving.Delta : Vector3.zero);

        return onGround
               && delta.y <= 0                                                                                                                 // We're moving downward
               && (distance <= 0                                                                                                               // We're either going down a slope
//                   || 90 - Vector3.Angle(hit.point - currentPosition, Vector3.up) < 30                                                         // Attempting to move at a shallow angle
                   || Vector3.Dot(Vector3.Cross(currentPosition - (hit.point - platformDelta), cross),                                                           // Or we're starting from a position on the same side of the slope as its normal
                                  Vector3.Cross(hit.normal, cross)) >= -0.005f
                   || Vector3.Dot(Vector3.Cross(currentPosition + floor.FloorVector * delta.x * (1 / floor.FloorVector.x) - hit.point, cross), // Or our expected position as modified by the floor's angle is on the same side of the slope as its normal
                                  Vector3.Cross(hit.normal, cross)) >= -0.005f);
    }

    private float CalculateWalkSpeed(float currentSpeed, float targetSpeed)
    {
        float speedDelta = targetSpeed - currentSpeed;
        if (Mathf.Abs(speedDelta) < 0.01f)
        {
            return targetSpeed;
        }

        float deltaSign = Mathf.Sign(speedDelta);
        float acceleration = deltaSign * (targetSpeed == 0 ? WalkFriction :
                                          (currentSpeed == 0 || Mathf.Sign(targetSpeed) == Mathf.Sign(currentSpeed)) ? WalkAcceleration :
                                          WalkAcceleration + WalkFriction);

        currentSpeed += Time.deltaTime * acceleration;
        if (deltaSign != Mathf.Sign(targetSpeed - currentSpeed)) return targetSpeed;
        return currentSpeed;
    }
}
