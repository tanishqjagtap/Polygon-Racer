using UnityEngine;

public class FinalCarController : MonoBehaviour
{
    [Header("Refs")]
    public Transform carVisual;

    [Header("Engine")]
    public float acceleration = 6500f;
    public float reverseAcceleration = 4500f;
    public float maxSpeed = 30f;

    [Header("Steering")]
    public float turnSpeed = 140f;
    public float turnSpeedHigh = 60f;

    [Header("Grip")]
    [Range(0.6f, 1f)]
    public float grip = 0.88f;

    [Header("Downforce")]
    public float downforce = 80f;

    [Header("Visual Tilt")]
    public float rollAngle = 12f;
    public float pitchAngle = 4f;
    public float tiltSmooth = 10f;

    Rigidbody rb;
    Quaternion visualStartRot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // stability
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.centerOfMass = new Vector3(0f, -0.6f, 0f);

        if (carVisual != null)
            visualStartRot = carVisual.localRotation;
    }

    void FixedUpdate()
    {
        float speed = rb.linearVelocity.magnitude;

        // INPUT
        float throttle = 0f;
        if (Input.GetKey(KeyCode.W)) throttle = 1f;
        if (Input.GetKey(KeyCode.S)) throttle = -1f;

        float steer = 0f;
        if (Input.GetKey(KeyCode.A)) steer = -1f;
        if (Input.GetKey(KeyCode.D)) steer = 1f;

        // gaming reverse steering
        if (throttle < 0f) steer *= -1f;

        // ENGINE
        if (throttle > 0f && speed < maxSpeed)
            rb.AddForce(transform.forward * acceleration, ForceMode.Force);

        if (throttle < 0f && speed < maxSpeed)
            rb.AddForce(-transform.forward * reverseAcceleration, ForceMode.Force);

        // DOWNFORCE (helps ramps + no shaking)
        rb.AddForce(-transform.up * downforce, ForceMode.Force);

        // TURN (speed based)
        if (speed > 0.5f)
        {
            float steerStrength = Mathf.Lerp(turnSpeed, turnSpeedHigh, speed / maxSpeed);
            float turn = steer * steerStrength * Time.fixedDeltaTime;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));
        }

        // GRIP (kills drifting)
        Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
        localVel.x *= grip;
        rb.linearVelocity = transform.TransformDirection(localVel);

        // VISUAL TILT ONLY
        if (carVisual != null)
        {
            float roll = -steer * rollAngle;
            float pitch = -throttle * pitchAngle;

            Quaternion targetRot = visualStartRot * Quaternion.Euler(pitch, 0f, roll);

            carVisual.localRotation = Quaternion.Lerp(
                carVisual.localRotation,
                targetRot,
                tiltSmooth * Time.fixedDeltaTime
            );
        }
    }
}
