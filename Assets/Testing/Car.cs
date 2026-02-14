using UnityEngine;

public class Car : MonoBehaviour
{
    [Header("Wheels")]
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider rearLeft;
    public WheelCollider rearRight;

    [Header("Wheel Meshes")]
    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    [Header("Engine")]
    public float maxMotorTorque = 1500f;
    public float maxSteeringAngle = 30f;
    public float brakeForce = 3000f;

    [Header("Transmission")]
    public float[] gearRatios = { 3.2f, 2.1f, 1.5f, 1.2f, 1.0f, 0.8f };
    public float finalDrive = 3.5f;
    public float maxRPM = 8000f;
    public float idleRPM = 800f;

    [Header("Center Of Mass (OPTIONAL BUT GOOD)")]
    public Transform centreOfMass;

    private int currentGear = 0;
    private float engineRPM;
    private Rigidbody rb;

    float throttleInput;
    float steerInput;
    bool brakeInput;

    // =========================
    // INIT
    // =========================
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // ✅ MUCH more stable values
        rb.mass = 1300f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0.05f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // ✅ Proper center of mass
        if (centreOfMass != null)
            rb.centerOfMass = centreOfMass.localPosition;
        else
            rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    // =========================
    // INPUT (frame based)
    // =========================
    void Update()
    {
        throttleInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
        brakeInput = Input.GetKey(KeyCode.Space);
    }

    // =========================
    // PHYSICS (VERY IMPORTANT)
    // =========================
    void FixedUpdate()
    {
        HandleMotor(throttleInput);
        HandleSteering(steerInput);
        HandleBraking(brakeInput);
        UpdateRPM();
        AutoShift();
    }

    // =========================
    // VISUAL UPDATE
    // =========================
    void LateUpdate()
    {
        UpdateWheelMeshes();
    }

    // =========================
    // MOTOR
    // =========================
    void HandleMotor(float throttle)
    {
        float torque = throttle * maxMotorTorque;

        rearLeft.motorTorque = torque;
        rearRight.motorTorque = torque;
    }

    // =========================
    // STEERING
    // =========================
    void HandleSteering(float steer)
    {
        float steeringAngle = steer * maxSteeringAngle;

        frontLeft.steerAngle = steeringAngle;
        frontRight.steerAngle = steeringAngle;
    }

    // =========================
    // BRAKES
    // =========================
    void HandleBraking(bool braking)
    {
        float force = braking ? brakeForce : 0f;

        frontLeft.brakeTorque = force;
        frontRight.brakeTorque = force;
        rearLeft.brakeTorque = force;
        rearRight.brakeTorque = force;
    }

    // =========================
    // RPM + GEARS
    // =========================
    void UpdateRPM()
    {
        float wheelRPM = (rearLeft.rpm + rearRight.rpm) / 2f;
        engineRPM = Mathf.Abs(wheelRPM) * gearRatios[currentGear] * finalDrive;
        engineRPM = Mathf.Clamp(engineRPM, idleRPM, maxRPM);
    }

    void AutoShift()
    {
        if (engineRPM > maxRPM * 0.9f && currentGear < gearRatios.Length - 1)
            currentGear++;

        if (engineRPM < maxRPM * 0.3f && currentGear > 0)
            currentGear--;
    }

    // =========================
    // WHEEL VISUALS
    // =========================
    void UpdateWheelMeshes()
    {
        UpdateWheel(frontLeft, frontLeftMesh);
        UpdateWheel(frontRight, frontRightMesh);
        UpdateWheel(rearLeft, rearLeftMesh);
        UpdateWheel(rearRight, rearRightMesh);
    }

    void UpdateWheel(WheelCollider col, Transform mesh)
    {
        if (mesh == null) return;

        Vector3 pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);
        mesh.position = pos;
        mesh.rotation = rot;
    }
}
