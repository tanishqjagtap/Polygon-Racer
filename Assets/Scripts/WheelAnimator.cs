using UnityEngine;

public class WheelAnimator : MonoBehaviour
{
    [Header("References")]
    public Rigidbody carRb;

    public Transform wheelFL;
    public Transform wheelFR;
    public Transform wheelRL;
    public Transform wheelRR;

    [Header("Wheel Settings")]
    public float wheelRadius = 0.35f; // adjust if rotation looks too fast/slow

    void Update()
    {
        if (carRb == null) return;

        // speed in m/s
        float speed = carRb.linearVelocity.magnitude;

        // rotation speed (degrees per second)
        // ? = v/r  (radians/sec) => convert to degrees/sec => * Mathf.Rad2Deg
        float rotationSpeed = (speed / wheelRadius) * Mathf.Rad2Deg;

        RotateWheel(wheelFL, rotationSpeed);
        RotateWheel(wheelFR, rotationSpeed);
        RotateWheel(wheelRL, rotationSpeed);
        RotateWheel(wheelRR, rotationSpeed);
    }

    void RotateWheel(Transform wheel, float rotationSpeed)
    {
        if (wheel == null) return;

        // rotate around local X axis (if wrong axis, we will change)
        wheel.Rotate(Vector3.right, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
