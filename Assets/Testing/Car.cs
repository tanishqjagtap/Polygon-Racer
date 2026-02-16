using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class Car : MonoBehaviour
{
    public bool motorCycleControl = false;
    public float motorcycleTiltDamping = 2f;
    public float motorcycleYawDamping = 1f;
    public float restoreStrength = 1f;
    public float restoreStrengthY = 1f;
    public float steerAssistTarget = 0.75f;
    public float coefFrictionMultiplier = 1.0f;
    public Vector3 centerOfDownforce = new Vector3(0, 0, 0);

    [Header("Aerodynamics")]
    public float dragCoefficient = 0.278f;
    public float frontalArea = 1.88f;
    public float airDensity = 1.225f;
    public float lowSpeedDragCoefficient = 0.37f;
    public float rollingResistanceCoeff = 0.015f;
    public GameObject adaptiveBrakingWing;
    public float brakingWingAngle = 60f;
    public float brakingWingSpeed = 8f;
    [HideInInspector] public float currentWingAngle = 0f;

    public Engine e;
    public GameObject skidMarkPrefab;
    public float smoothTurn = 0.03f;
    float coefStaticFriction = 0.95f;
    float coefKineticFriction = 0.35f;
    public GameObject wheelPrefab;
    public WheelProperties[] wheels;
    public float wheelGripX = 8f;
    public float wheelGripZ = 42f;
    public float suspensionForce = 90f;
    public float dampAmount = 2.5f;
    public float suspensionForceClamp = 200f;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public bool forwards = true;

    // Assists
    public bool steeringAssist = true;
    public bool throttleAssist = true;
    public bool brakeAssist = true;
    [HideInInspector] public Vector2 userInput = Vector2.zero;
    public float downforce = 0.16f;
    [HideInInspector] public float isBraking = 0f;

    public Vector3 COMOffset = new Vector3(0, -0.2f, 0);
    public float Inertia = 1.2f;
    public float carSpeedFactor = 0.03f;

    float handbrakeInput = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();

        foreach (var w in wheels)
        {
            w.wheelObject = Instantiate(wheelPrefab, transform);
            w.wheelObject.transform.localPosition = w.localPosition;
            w.wheelObject.transform.eulerAngles = transform.eulerAngles;
            w.wheelObject.transform.localScale = 2f * new Vector3(w.size, w.size, w.size);
            w.wheelCircumference = 2f * Mathf.PI * w.size;
        }

        rb.centerOfMass += COMOffset;
        rb.inertiaTensor *= Inertia;

        e.SetRPM(0f);
    }

    void Update()
    {
        // ✅ SIMPLE INPUT (old Input Manager)
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        float steerInput = Input.GetAxis("Horizontal");
        float throttleInput = Input.GetAxis("Vertical");
        handbrakeInput = Input.GetKey(KeyCode.Space) ? 1f : 0f;

        userInput.x = Mathf.Lerp(
            userInput.x,
            (moveInput.x + steerInput) / (1 + rb.velocity.magnitude * carSpeedFactor),
            50f * Time.deltaTime
        );

        userInput.y = Mathf.Lerp(
            userInput.y,
            moveInput.y + throttleInput,
            50f * Time.deltaTime
        );

        isBraking = userInput.y < 0 && forwards ? Mathf.Abs(userInput.y) : 0f;

        // Reset car
        if (Input.GetKeyDown(KeyCode.R))
        {
            float yrotation = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0, yrotation, 0);
            transform.position += Vector3.up * 2f;
            rb.velocity = transform.forward * 5f;
            rb.angularVelocity = Vector3.zero;
        }

        // Manual gear keys still work
        if (Input.GetKeyDown(KeyCode.E)) e.UpGear(this);
        else if (Input.GetKeyDown(KeyCode.D)) e.DownGear(this);

        e.checkGearSwitching(this, throttleInput);
    }

    void FixedUpdate()
    {
        ApplyAerodynamicDrag();

        float averageWheelAngularVelocity = 0f;
        foreach (var w in wheels)
        {
            averageWheelAngularVelocity += w.angularVelocity;
        }

        averageWheelAngularVelocity /= wheels.Length;
        e.SetRPM(averageWheelAngularVelocity);
    }

    private void ApplyAerodynamicDrag()
    {
        Vector3 velocity = rb.velocity;
        float speed = velocity.magnitude;

        float currentDragCoeff = dragCoefficient;
        float dragMagnitude = 0.5f * airDensity * speed * speed * currentDragCoeff * frontalArea;

        Vector3 dragForce = -velocity.normalized * dragMagnitude;
        rb.AddForce(dragForce / 200f, ForceMode.Force);
    }
}
