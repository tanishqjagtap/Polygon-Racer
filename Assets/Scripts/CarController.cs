using UnityEngine;

public class CarController : MonoBehaviour
{
    public float acceleration = 50f;
    public float reverseAcceleration = 30f;
    public float turnSpeed = 80f;
    public float maxSpeed = 25f;

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

        // ---------- TURNING ----------
        if (currentSpeed > 0.5f)   // only turn while moving
        {
            if (Input.GetKey(KeyCode.A))
                transform.Rotate(0f, -turnSpeed * Time.deltaTime, 0f);

            if (Input.GetKey(KeyCode.D))
                transform.Rotate(0f, turnSpeed * Time.deltaTime, 0f);
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
