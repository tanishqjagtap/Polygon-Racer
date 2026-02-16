using UnityEngine;

// 🔥 ULTRA REALISTIC CAR CONTROLLER (UPDATED — FIXED reverse for real)
// Controls:
//   W / Up → forward
//   S / Down → brake then reverse
//   R → reset car

[RequireComponent(typeof(Rigidbody))]
public class car : MonoBehaviour
{
    [Header("=== ENGINE SPECS ===")]
    public float engineCC = 3000f;
    public float maxRPM = 7500f;
    public float idleRPM = 800f;
    public float maxTorque = 500f;
    public AnimationCurve torqueCurve;

    [Header("=== TRANSMISSION ===")]
    public float[] gearRatios = { 3.8f, 2.4f, 1.7f, 1.25f, 1.0f, 0.82f };
    public float finalDriveRatio = 3.73f;
    public float drivetrainEfficiency = 0.85f;
    public float shiftUpRPM = 7200f;
    public float shiftDownRPM = 2500f;

    [Header("=== SPEED & DRAG ===")]
    public float aerodynamicDrag = 0.32f;
    public float frontalArea = 2.2f;
    public float rollingResistance = 8f;

    [Header("=== WHEELS ===")]
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;

    public Transform meshFL;
    public Transform meshFR;
    public Transform meshRL;
    public Transform meshRR;

    [Header("=== VISUAL FIXES ===")]
    public bool flipRearWheels = true;

    [Header("=== RESET SETTINGS ===")]
    public float resetLift = 1.2f;

    [Header("=== STEERING ===")]
    public float maxSteerAngle = 30f;

    [Header("=== BRAKES ===")]
    public float brakeForce = 4000f;
    public float handbrakeForce = 8000f;

    [Header("=== RUNTIME (READ ONLY) ===")]
    public float currentRPM;
    public int currentGear = 1;
    public float speedKmh;

    private Rigidbody rb;
    private float throttleInput; // can be negative
    private float brakeInput;
    private float steerInput;
    private float handbrakeInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = transform.Find("centre of mass") != null
            ? transform.Find("centre of mass").localPosition
            : new Vector3(0, -0.45f, 0);

        if (torqueCurve.length == 0)
        {
            torqueCurve = new AnimationCurve(
                new Keyframe(0f, 0.7f),
                new Keyframe(0.25f, 1f),
                new Keyframe(0.6f, 0.95f),
                new Keyframe(1f, 0.75f)
            );
        }
    }

    void Update()
    {
        float vertical = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
        handbrakeInput = Input.GetKey(KeyCode.Space) ? 1f : 0f;

        speedKmh = rb.linearVelocity.magnitude * 3.6f;

        // 🔥 MUCH MORE RELIABLE BRAKE/REVERSE LOGIC
        if (vertical > 0.05f)
        {
            // forward
            throttleInput = vertical;
            brakeInput = 0f;
        }
        else if (vertical < -0.05f)
        {
            // check if car is moving forward in local space
            float localZ = transform.InverseTransformDirection(rb.linearVelocity).z;

            if (localZ > 1f)
            {
                // still rolling forward → brake first
                throttleInput = 0f;
                brakeInput = 1f;
            }
            else
            {
                // now allow reverse
                throttleInput = vertical; // negative value
                brakeInput = 0f;
            }
        }
        else
        {
            throttleInput = 0f;
            brakeInput = 0f;
        }

        if (Input.GetKeyDown(KeyCode.R))
            ResetCar();

        UpdateWheelMeshes();
    }

    void FixedUpdate()
    {
        CalculateEngineRPM();
        HandleAutomaticGears();
        ApplyMotor();
        ApplySteering();
        ApplyBrakes();
        ApplyDrag();
    }

    // ================= ENGINE =================

    void CalculateEngineRPM()
    {
        float wheelRPM = (wheelRL.rpm + wheelRR.rpm) * 0.5f;
        float gearRatio = gearRatios[Mathf.Clamp(currentGear - 1, 0, gearRatios.Length - 1)];

        currentRPM = Mathf.Abs(wheelRPM * gearRatio * finalDriveRatio);
        currentRPM = Mathf.Clamp(currentRPM, idleRPM, maxRPM);
    }

    float GetEngineTorque()
    {
        float normalizedRPM = currentRPM / maxRPM;
        float torqueFactor = torqueCurve.Evaluate(normalizedRPM);
        return maxTorque * torqueFactor * throttleInput; // negative allowed
    }

    // ================= GEARS =================

    void HandleAutomaticGears()
    {
        if (currentGear < gearRatios.Length && currentRPM > shiftUpRPM)
            currentGear++;
        else if (currentGear > 1 && currentRPM < shiftDownRPM)
            currentGear--;
    }

    // ================= MOTOR =================

    void ApplyMotor()
    {
        float engineTorque = GetEngineTorque();
        float gearRatio = gearRatios[Mathf.Clamp(currentGear - 1, 0, gearRatios.Length - 1)];

        float wheelTorque = engineTorque * gearRatio * finalDriveRatio * drivetrainEfficiency;
        wheelTorque = Mathf.Clamp(wheelTorque, -6000f, 8000f);

        wheelRL.motorTorque = wheelTorque;
        wheelRR.motorTorque = wheelTorque;
    }

    // ================= STEERING =================

    void ApplySteering()
    {
        float steer = steerInput * maxSteerAngle;
        wheelFL.steerAngle = steer;
        wheelFR.steerAngle = steer;
    }

    // ================= BRAKES =================

    void ApplyBrakes()
    {
        float brake = brakeInput * brakeForce;
        float handbrake = handbrakeInput * handbrakeForce;

        wheelFL.brakeTorque = brake;
        wheelFR.brakeTorque = brake;
        wheelRL.brakeTorque = brake + handbrake;
        wheelRR.brakeTorque = brake + handbrake;
    }

    // ================= DRAG =================

    void ApplyDrag()
    {
        if (rb.linearVelocity.magnitude < 0.1f) return;

        float airDensity = 1.225f;
        float dragForce = 0.5f * airDensity * aerodynamicDrag * frontalArea * rb.linearVelocity.sqrMagnitude;

        Vector3 drag = -rb.linearVelocity.normalized * dragForce;
        rb.AddForce(drag);

        Vector3 rolling = -rb.linearVelocity.normalized * rollingResistance;
        rb.AddForce(rolling);
    }

    // ================= RESET =================

    void ResetCar()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 euler = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, euler.y, 0f);
        transform.position += Vector3.up * resetLift;
    }

    // ================= VISUALS =================

    void UpdateWheelMeshes()
    {
        UpdateWheel(wheelFL, meshFL, false);
        UpdateWheel(wheelFR, meshFR, false);
        UpdateWheel(wheelRL, meshRL, true);
        UpdateWheel(wheelRR, meshRR, true);
    }

    void UpdateWheel(WheelCollider col, Transform mesh, bool isRear)
    {
        if (col == null || mesh == null) return;

        Vector3 pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);

        if (isRear && flipRearWheels)
            rot *= Quaternion.Euler(0f, 180f, 0f);

        mesh.position = pos;
        mesh.rotation = rot;
    }
}
