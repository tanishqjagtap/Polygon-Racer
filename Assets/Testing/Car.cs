using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Car : MonoBehaviour
{
    // 🔥 Controlled by RaceManager
    public bool canDrive = false;

    [Header("=== AUTO STOP (FINISH) ===")]
    public bool autoStop = false;
    public float autoBrakeForce = 2500f;

    [Header("=== ENGINE SPECS ===")]
    public float engineCC = 3000f;
    public float maxRPM = 7500f;
    public float idleRPM = 800f;
    public float maxTorque = 520f;
    public AnimationCurve torqueCurve;

    [Header("=== SHIFT STABILITY ===")]
    public float minGearHoldTime = 1.0f;
    public float rpmSmoothing = 5f;

    private float smoothedRPM;
    private float gearEnterTime;

    [Header("=== TRANSMISSION ===")]
    public float[] gearRatios = { 3.8f, 2.4f, 1.7f, 1.25f, 1.0f, 0.82f };
    public float finalDriveRatio = 3.9f;
    public float drivetrainEfficiency = 0.9f;
    public float shiftUpRPM = 7100f;
    public float shiftDownRPM = 2600f;
    public float shiftDelay = 0.35f;

    [Header("=== SPEED & DRAG ===")]
    public float aerodynamicDrag = 0.30f;
    public float frontalArea = 2.2f;
    public float rollingResistance = 6f;

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
    private float throttleInput;
    private float brakeInput;
    private float steerInput;
    private float handbrakeInput;
    private float lastShiftTime;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.centerOfMass = transform.Find("centre of mass") != null
            ? transform.Find("centre of mass").localPosition
            : new Vector3(0, -0.45f, 0);

        if (torqueCurve.length == 0)
        {
            torqueCurve = new AnimationCurve(
                new Keyframe(0f, 0.8f),
                new Keyframe(0.3f, 1.0f),
                new Keyframe(0.65f, 0.95f),
                new Keyframe(1f, 0.7f)
            );
        }
    }

    void Update()
    {
        // 🚨 LOCKED DURING COUNTDOWN OR FINISH
        if (!canDrive)
        {
            throttleInput = 0f;
            brakeInput = 0f;
            steerInput = 0f;
            handbrakeInput = 0f;

            speedKmh = rb.linearVelocity.magnitude * 3.6f;
            UpdateWheelMeshes();
            return;
        }

        float vertical = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
        handbrakeInput = Input.GetKey(KeyCode.Space) ? 1f : 0f;

        speedKmh = rb.linearVelocity.magnitude * 3.6f;

        if (vertical > 0.05f)
        {
            throttleInput = vertical;
            brakeInput = 0f;
        }
        else if (vertical < -0.05f)
        {
            float localZ = transform.InverseTransformDirection(rb.linearVelocity).z;

            if (localZ > 1f)
            {
                throttleInput = 0f;
                brakeInput = 1f;
            }
            else
            {
                throttleInput = vertical;
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

        float rawRPM = Mathf.Abs(wheelRPM * gearRatio * finalDriveRatio);
        rawRPM = Mathf.Clamp(rawRPM, idleRPM, maxRPM);

        smoothedRPM = Mathf.Lerp(smoothedRPM, rawRPM, Time.fixedDeltaTime * rpmSmoothing);
        currentRPM = smoothedRPM;
    }

    float GetEngineTorque()
    {
        float normalizedRPM = currentRPM / maxRPM;
        float torqueFactor = torqueCurve.Evaluate(normalizedRPM);
        float gearBoost = Mathf.Lerp(1.25f, 0.85f, (currentGear - 1f) / (gearRatios.Length - 1f));

        return maxTorque * torqueFactor * throttleInput * gearBoost;
    }

    // ================= GEARS =================

    void HandleAutomaticGears()
    {
        if (speedKmh < 5f)
        {
            currentGear = 1;
            gearEnterTime = Time.time;
            return;
        }

        if (Time.time < gearEnterTime + minGearHoldTime) return;
        if (Time.time < lastShiftTime + shiftDelay) return;

        bool canUpshift =
            throttleInput > 0.15f &&
            currentGear < gearRatios.Length &&
            currentRPM > shiftUpRPM &&
            speedKmh > currentGear * 15f;

        if (canUpshift)
        {
            currentGear++;
            lastShiftTime = Time.time;
            gearEnterTime = Time.time;
            return;
        }

        if (currentGear > 1 && currentRPM < shiftDownRPM)
        {
            currentGear--;
            lastShiftTime = Time.time;
            gearEnterTime = Time.time;
        }
    }

    // ================= MOTOR =================

    void ApplyMotor()
    {
        float engineTorque = GetEngineTorque();
        float gearRatio = gearRatios[Mathf.Clamp(currentGear - 1, 0, gearRatios.Length - 1)];

        float wheelTorque = engineTorque * gearRatio * finalDriveRatio * drivetrainEfficiency;
        wheelTorque = Mathf.Clamp(wheelTorque, -6500f, 9000f);

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

    // ================= BRAKES (🔥 UPGRADED) =================

    void ApplyBrakes()
    {
        float brake = brakeInput * brakeForce;
        float handbrake = handbrakeInput * handbrakeForce;

        // 🔥 smooth stop after finish
        if (autoStop)
        {
            brake = autoBrakeForce;
            throttleInput = 0f;
        }

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

        rb.AddForce(-rb.linearVelocity.normalized * dragForce);
        rb.AddForce(-rb.linearVelocity.normalized * rollingResistance);
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

        col.GetWorldPose(out Vector3 pos, out Quaternion rot);

        if (isRear && flipRearWheels)
            rot *= Quaternion.Euler(0f, 180f, 0f);

        mesh.position = pos;
        mesh.rotation = rot;
    }
}