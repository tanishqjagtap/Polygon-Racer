using UnityEngine;

public class CarController : MonoBehaviour
{
    public Transform carVisual;

    public float acceleration = 50f;
    public float reverseAcceleration = 30f;
    public float turnSpeed = 80f;
    public float maxSpeed = 25f;

    public float bankAngle = 15f;
    public float bankSpeed = 5f;

    Rigidbody rb;

    Vector3 startPos;
    Quaternion startRot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // save starting position + rotation
        startPos = transform.position;
        startRot = transform.rotation;
    }

    void FixedUpdate()
    {
        float currentSpeed = rb.linearVelocity.magnitude;

        // ---------- FORWARD ----------
        if (Input.GetKey(KeyCode.W))
        {
            if (currentSpeed < maxSpeed)
                rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration);
        }

        // ---------- REVERSE ----------
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-transform.forward * reverseAcceleration, ForceMode.Acceleration);
        }

        // ---------- TURN INPUT ----------
        float steer = 0f;
        if (Input.GetKey(KeyCode.A)) steer = -1f;
        if (Input.GetKey(KeyCode.D)) steer = 1f;

        // ---------- TURNING (Rigidbody Rotation ✅) ----------
        if (currentSpeed > 0.5f) // only turn while moving
        {
            Quaternion turnRotation = Quaternion.Euler(0f, steer * turnSpeed * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        // ---------- BANKING VISUAL ONLY ----------
        if (carVisual != null)
        {
            float targetZ = -steer * bankAngle;

            // ✅ banking should ONLY tilt Z axis
            Quaternion targetRot = Quaternion.Euler(0f, 0f, targetZ);

            carVisual.localRotation = Quaternion.Lerp(
                carVisual.localRotation,
                targetRot,
                bankSpeed * Time.deltaTime
            );
        }
    }

    void Update()
    {
        // press R = reset car
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartCar();
        }
    }

    void RestartCar()
    {
        transform.position = startPos;
        transform.rotation = startRot;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
