using UnityEngine;

public class ArcadeCarControllerSmooth : MonoBehaviour
{
    [Header("Refs")]
    public Transform carVisual;

    [Header("Movement")]
    public float moveSpeed = 10f;
    public float reverseSpeed = 10f;
    public float accel = 8f;

    [Header("Turning")]
    public float turnSpeed = 140f;
    public float steerSmooth = 8f;

    [Header("Visual Animation")]
    public float bankAngle = 12f;
    public float pitchAngle = 4f;
    public float animSmooth = 10f;

    float currentSpeed;
    float steerCurrent;

    Quaternion visualStartRot;

    void Start()
    {
        if (carVisual != null)
            visualStartRot = carVisual.localRotation;
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RaceManager.Instance.ResetToCheckpoint(gameObject);
        }

        // INPUT
        float throttle = 0f;
        if (Input.GetKey(KeyCode.W)) throttle = 1f;
        if (Input.GetKey(KeyCode.S)) throttle = -1f;

        float steer = 0f;
        if (Input.GetKey(KeyCode.A)) steer = -1f;
        if (Input.GetKey(KeyCode.D)) steer = 1f;

        // gaming reverse steering
        if (throttle < 0f) steer *= -1f;

        // SPEED SMOOTH
        float targetSpeed = 0f;
        if (throttle > 0f) targetSpeed = moveSpeed;
        if (throttle < 0f) targetSpeed = -reverseSpeed;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);

        // STEER SMOOTH
        steerCurrent = Mathf.Lerp(steerCurrent, steer, steerSmooth * Time.fixedDeltaTime);

        // MOVE + ROTATE
        transform.position += transform.forward * currentSpeed * Time.fixedDeltaTime;

        if (Mathf.Abs(currentSpeed) > 0.2f)
            transform.Rotate(0f, steerCurrent * turnSpeed * Time.fixedDeltaTime, 0f);

        // VISUAL ANIMATION
        if (carVisual != null)
        {
            float bank = -steerCurrent * bankAngle;
            float pitch = -throttle * pitchAngle;

            Quaternion targetRot = visualStartRot * Quaternion.Euler(pitch, 0f, bank);
            carVisual.localRotation = Quaternion.Lerp(carVisual.localRotation, targetRot, animSmooth * Time.fixedDeltaTime);
        }
    }
}
