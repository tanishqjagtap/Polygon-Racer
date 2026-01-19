using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class CarControllerWORKING : MonoBehaviour
{
    public float acceleration = 2000f;
    public float maxSpeed = 35f;
    public float turnSpeed = 80f;
    public float grip = 0.9f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundDistance = 0.6f;
    public float groundRadius = 0.35f;

    [Header("Downforce")]
    public float downforce = 50f;

    Rigidbody rb;
    Collider col;

    Vector3 groundNormal = Vector3.up;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        rb.centerOfMass = new Vector3(0f, -0.5f, 0f);
    }

    void FixedUpdate()
    {
        // INPUT
        float throttle = 0f;
        if (Input.GetKey(KeyCode.W)) throttle = 1f;
        if (Input.GetKey(KeyCode.S)) throttle = -1f;

        float steer = 0f;
        if (Input.GetKey(KeyCode.A)) steer = -1f;
        if (Input.GetKey(KeyCode.D)) steer = 1f;

        float speed = rb.linearVelocity.magnitude;

        bool grounded = IsGrounded();

        // ✅ Align car to slope
        if (grounded)
        {
            Quaternion slopeRot = Quaternion.FromToRotation(transform.up, groundNormal) * rb.rotation;
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, slopeRot, 8f * Time.fixedDeltaTime));
        }

        // ✅ Engine on slope
        if (grounded && speed < maxSpeed)
        {
            Vector3 forwardOnSlope = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized;
            rb.AddForce(forwardOnSlope * throttle * acceleration * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }

        // ✅ Turning (still around Y but stable now)
        if (grounded && speed > 0.5f)
        {
            float turn = steer * turnSpeed * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));
        }

        // ✅ Grip (fix sideways slip but do not kill slope movement)
        if (grounded)
        {
            Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
            localVel.x *= grip;
            rb.linearVelocity = transform.TransformDirection(localVel);
        }

        // ✅ Downforce
        if (grounded)
        {
            rb.AddForce(-groundNormal * downforce * speed, ForceMode.Force);
        }
    }

    bool IsGrounded()
    {
        Vector3 origin = col.bounds.center;

        if (Physics.SphereCast(origin, groundRadius, -transform.up, out RaycastHit hit, groundDistance, groundLayer))
        {
            groundNormal = hit.normal;
            return true;
        }

        groundNormal = Vector3.up;
        return false;
    }
}
