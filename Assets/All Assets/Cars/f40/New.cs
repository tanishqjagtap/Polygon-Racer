using UnityEngine;

public class New : MonoBehaviour
{
    [Header("Refs")]
    public Transform carVisual;

    [Header("Engine")]
    public float acceleration = 55f;
    public float reverseAcceleration = 35f;
    public float maxSpeed = 28f;

    [Header("Steering")]
    public float turnSpeed = 90f;
    public float minTurnSpeed = 35f;     // turning at high speed
    public float steeringSmooth = 6f;

    [Header("Grip / Drift")]
    [Range(0.4f, 1f)]
    public float grip = 0.82f;           // lower = more drift

    [Header("Body Roll + Shock")]
    public float rollAngle = 12f;        // Z tilt while turning
    public float pitchAngle = 4f;        // X tilt accel/brake
    public float visualSmooth = 8f;

    public float shockStrength = 0.15f;  // up/down wobble
    public float shockSpeed = 8f;

    Rigidbody rb;

    float steerCurrent;
    Quaternion visualBaseRot;
    float shockTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (carVisual != null)
            visualBaseRot = carVisual.localRotation;
    }

    void FixedUpdate()
    {
        float speed = rb.linearVelocity.magnitude;

        // ----- INPUT -----
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.W)) moveInput = 1f;
        if (Input.GetKey(KeyCode.S)) moveInput = -1f;

        float steerInput = 0f;
        if (Input.GetKey(KeyCode.A)) steerInput = -1f;
        if (Input.GetKey(KeyCode.D)) steerInput = 1f;

        // gaming reverse steering
        if (moveInput < 0f) steerInput *= -1f;

        // ----- ENGINE FORCE -----
        if (moveInput > 0f && speed < maxSpeed)
            rb.AddForce(transform.forward * acceleration, ForceMode.Acceleration);

        if (moveInput < 0f && speed < maxSpeed)
            rb.AddForce(-transform.forward * reverseAcceleration, ForceMode.Acceleration);

        // ----- STEERING (speed based) -----
        float turnAtSpeed = Mathf.Lerp(turnSpeed, minTurnSpeed, speed / maxSpeed);
        float targetSteer = steerInput * turnAtSpeed;
        steerCurrent = Mathf.Lerp(steerCurrent, targetSteer, steeringSmooth * Time.fixedDeltaTime);

        if (speed > 0.5f)
        {
            Quaternion turnRot = Quaternion.Euler(0f, steerCurrent * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * turnRot);
        }

        // ----- GRIP (sideways friction) -----
        Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
        localVel.x *= grip;
        rb.linearVelocity = transform.TransformDirection(localVel);

        // ----- SHOCK TIMER (for bounce while moving) -----
        if (speed > 1f)
            shockTimer += Time.fixedDeltaTime * shockSpeed;
        else
            shockTimer = Mathf.Lerp(shockTimer, 0f, 6f * Time.fixedDeltaTime);

        // ----- VISUAL ROLL / PITCH / SHOCK -----
        if (carVisual != null)
        {
            float roll = -steerInput * rollAngle;          // tilt sideways
            float pitch = -moveInput * pitchAngle;         // tilt forward/back

            float shock = Mathf.Sin(shockTimer) * shockStrength * Mathf.Clamp01(speed / 10f);

            Quaternion targetRot = visualBaseRot * Quaternion.Euler(pitch, 0f, roll);
            carVisual.localRotation = Quaternion.Lerp(carVisual.localRotation, targetRot, visualSmooth * Time.fixedDeltaTime);

            // small bounce (shock)
            Vector3 lp = carVisual.localPosition;
            lp.y = Mathf.Lerp(lp.y, shock, visualSmooth * Time.fixedDeltaTime);
            carVisual.localPosition = lp;
        }
    }
}
