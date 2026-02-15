using UnityEngine;

public class car : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider rearLeft;
    public WheelCollider rearRight;

    [Header("Wheel Meshes")]
    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    [Header("Center Of Mass")]
    public Transform centerOfMass;

    [Header("Engine")]
    [Range(500, 5000)] public float maxMotorTorque = 1800f;
    [Range(10, 45)] public float maxSteeringAngle = 30f;
    [Range(1000, 8000)] public float brakeForce = 4000f;
    [Range(2000, 10000)] public float handbrakeForce = 7000f;
    [Range(50, 300)] public float maxSpeedKmh = 180f;

    [Header("Grip / Drift")]
    [Range(0.5f, 3f)] public float forwardGrip = 1.5f;
    [Range(0.5f, 3f)] public float sidewaysGrip = 1.2f;
    [Range(0.1f, 1f)] public float driftGripMultiplier = 0.4f;

    [Header("Reset")]
    public KeyCode resetKey = KeyCode.R;
    public float resetHeight = 1.5f;

    private Rigidbody rb;
    private readonly Quaternion rearFlip = Quaternion.Euler(0f, 180f, 0f);

    private bool isDrifting;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (centerOfMass != null)
            rb.centerOfMass = centerOfMass.localPosition;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        ApplyNormalFriction();
    }

    void FixedUpdate()
    {
        float throttle = Input.GetAxis("Vertical");
        float steer = Input.GetAxis("Horizontal");

        bool handbrake = Input.GetKey(KeyCode.Space);
        bool brakeKey = Input.GetKey(KeyCode.S);

        LimitTopSpeed();
        HandleMotor(throttle, brakeKey);
        HandleSteering(steer);
        HandleBraking(brakeKey);
        HandleHandbrake(handbrake, steer);

        UpdateWheelMeshes();
    }

    void Update()
    {
        if (Input.GetKeyDown(resetKey))
            ResetCar();
    }

    // ================= MOTOR =================
    void HandleMotor(float throttle, bool brakeKey)
    {
        if (brakeKey) // pressing S should not accelerate backwards hard
        {
            rearLeft.motorTorque = 0;
            rearRight.motorTorque = 0;
            return;
        }

        float torque = throttle * maxMotorTorque;
        rearLeft.motorTorque = torque;
        rearRight.motorTorque = torque;
    }

    // ================= STEERING =================
    void HandleSteering(float steer)
    {
        float angle = steer * maxSteeringAngle;
        frontLeft.steerAngle = angle;
        frontRight.steerAngle = angle;
    }

    // ================= NORMAL BRAKE (S) =================
    void HandleBraking(bool braking)
    {
        float force = braking ? brakeForce : 0f;

        frontLeft.brakeTorque = force;
        frontRight.brakeTorque = force;
    }

    // ================= HANDBRAKE DRIFT =================
    void HandleHandbrake(bool handbrake, float steer)
    {
        if (handbrake)
        {
            isDrifting = Mathf.Abs(steer) > 0.1f;

            // 🔥 lock rear wheels
            rearLeft.brakeTorque = handbrakeForce;
            rearRight.brakeTorque = handbrakeForce;

            // 🔥 reduce sideways grip for drift
            SetRearGrip(sidewaysGrip * driftGripMultiplier);
        }
        else
        {
            rearLeft.brakeTorque = 0f;
            rearRight.brakeTorque = 0f;

            ApplyNormalFriction();
            isDrifting = false;
        }
    }

    // ================= SPEED LIMIT =================
    void LimitTopSpeed()
    {
        float speed = rb.linearVelocity.magnitude * 3.6f;

        if (speed > maxSpeedKmh)
            rb.linearVelocity = rb.linearVelocity.normalized * (maxSpeedKmh / 3.6f);
    }

    // ================= RESET =================
    void ResetCar()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position += Vector3.up * resetHeight;
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    // ================= FRICTION =================
    void ApplyNormalFriction()
    {
        SetupWheelFriction(frontLeft, sidewaysGrip);
        SetupWheelFriction(frontRight, sidewaysGrip);
        SetupWheelFriction(rearLeft, sidewaysGrip);
        SetupWheelFriction(rearRight, sidewaysGrip);
    }

    void SetRearGrip(float grip)
    {
        SetupWheelFriction(rearLeft, grip);
        SetupWheelFriction(rearRight, grip);
    }

    void SetupWheelFriction(WheelCollider wc, float sideGrip)
    {
        WheelFrictionCurve fwd = wc.forwardFriction;
        fwd.stiffness = forwardGrip;
        wc.forwardFriction = fwd;

        WheelFrictionCurve side = wc.sidewaysFriction;
        side.stiffness = sideGrip;
        wc.sidewaysFriction = side;
    }

    // ================= VISUAL SYNC =================
    void UpdateWheelMeshes()
    {
        UpdateSingleWheel(frontLeft, frontLeftMesh, false);
        UpdateSingleWheel(frontRight, frontRightMesh, false);
        UpdateSingleWheel(rearLeft, rearLeftMesh, true);
        UpdateSingleWheel(rearRight, rearRightMesh, true);
    }

    void UpdateSingleWheel(WheelCollider col, Transform mesh, bool flip)
    {
        if (col == null || mesh == null) return;

        Vector3 pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);

        mesh.position = pos;
        mesh.rotation = flip ? rot * rearFlip : rot;
    }
}
