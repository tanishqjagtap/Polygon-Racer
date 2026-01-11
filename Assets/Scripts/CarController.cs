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

    [Range(0.5f, 1f)]
    public float grip = 0.85f; // 0.85 = good. Lower = more grip

    Rigidbody rb;

    Vector3 startPos;
    Quaternion startRot;

    Quaternion visualStartRot;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        startPos = transform.position;
        startRot = transform.rotation;

        if (carVisual != null)
            visualStartRot = carVisual.localRotation;
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
            // reverse reduced so it doesn't glitch
            rb.AddForce(-transform.forward * reverseAcceleration * 0.6f, ForceMode.Acceleration);
        }

        // ---------- TURN INPUT ----------
        float steer = 0f;
        if (Input.GetKey(KeyCode.A)) steer = -1f;
        if (Input.GetKey(KeyCode.D)) steer = 1f;

        // ---------- TURNING (speed based, smooth) ----------
        if (currentSpeed > 0.5f)
        {
            float steerStrength = Mathf.Lerp(0f, turnSpeed, currentSpeed / maxSpeed);

            Quaternion turnRotation = Quaternion.Euler(0f, steer * steerStrength * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        // ---------- SIDE GRIP (stops sliding) ----------
        Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
        localVel.x *= grip;
        rb.linearVelocity = transform.TransformDirection(localVel);

        // ---------- BANKING VISUAL ONLY ----------
        if (carVisual != null)
        {
            float targetZ = -steer * bankAngle;

            Quaternion targetRot = visualStartRot * Quaternion.Euler(0f, 0f, targetZ);

            carVisual.localRotation = Quaternion.Lerp(
                carVisual.localRotation,
                targetRot,
                bankSpeed * Time.fixedDeltaTime
            );
        }
    }

    void Update()
    {
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

        if (carVisual != null)
            carVisual.localRotation = visualStartRot;
    }
}
