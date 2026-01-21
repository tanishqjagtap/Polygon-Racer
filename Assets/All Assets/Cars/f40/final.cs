using UnityEngine;

public class final : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 12f;
    public float turnSpeed = 120f;

    [Header("Drag / Smooth Stop")]
    public float drag = 4f;

    [Header("Turn Animation (Tilt)")]
    public float maxTilt = 5f;        // how much it leans while turning
    public float tiltSmooth = 8f;      // how smooth the tilt is

    private float currentSpeed = 0f;

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Vertical");     // W/S
        float turnInput = Input.GetAxisRaw("Horizontal");   // A/D

        // --- Move speed control ---
        if (moveInput != 0)
            currentSpeed = moveInput * moveSpeed;
        else
            currentSpeed = Mathf.Lerp(currentSpeed, 0, drag * Time.deltaTime);

        // --- Move forward ---
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        // --- Turn only when moving ---
        if (Mathf.Abs(currentSpeed) > 0.1f)
            transform.Rotate(Vector3.up * turnInput * turnSpeed * Time.deltaTime);

        // --- Tilt animation while turning ---
        float targetTilt = -turnInput * maxTilt; // negative makes it lean correctly
        Quaternion targetRot = Quaternion.Euler(0, transform.eulerAngles.y, targetTilt);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, tiltSmooth * Time.deltaTime);
    }
}
