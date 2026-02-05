using UnityEngine;

public class Real : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;

    [Header("Wheel Meshes")]
    public Transform frontLeftMesh;
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    [Header("Car Settings")]
    public float motorPower = 1800f;
    public float brakePower = 3500f;
    public float maxSteerAngle = 32f;
    public float handbrakeDriftFactor = 0.5f;

    [Header("Grip Settings")]
    public float rearStiffness = 1.2f;
    public float driftStiffness = 0.6f;

    private float horizontalInput;
    private float verticalInput;
    private bool isGrounded;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SetupSuspension();
    }

    void Update()
    {
        GetInput();
        UpdateWheelMeshes();
    }

    void FixedUpdate()
    {
        CheckGrounded();
        ApplyMotor();
        ApplySteering();
        ApplyBrakes();
        ApplyDriftPhysics();
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal"); // A/D
        verticalInput = Input.GetAxis("Vertical");     // W/S
    }

    void ApplyMotor()
    {
        if (!isGrounded)
        {
            rearLeftCollider.motorTorque = 0;
            rearRightCollider.motorTorque = 0;
            return;
        }

        float torque = motorPower * verticalInput;

        // Rear Wheel Drive only
        rearLeftCollider.motorTorque = torque;
        rearRightCollider.motorTorque = torque;
    }

    void ApplySteering()
    {
        if (!isGrounded) return;

        float steerAngle = maxSteerAngle * horizontalInput;

        // Front wheels steer
        frontLeftCollider.steerAngle = steerAngle;
        frontRightCollider.steerAngle = steerAngle;
    }

    void ApplyBrakes()
    {
        bool braking = verticalInput < 0;

        float brakeTorque = braking ? brakePower : 0;

        frontLeftCollider.brakeTorque = brakeTorque;
        frontRightCollider.brakeTorque = brakeTorque;
        rearLeftCollider.brakeTorque = brakeTorque;
        rearRightCollider.brakeTorque = brakeTorque;
    }

    void ApplyDriftPhysics()
    {
        WheelFrictionCurve sidewaysFriction = rearLeftCollider.sidewaysFriction;

        if (Mathf.Abs(horizontalInput) > 0.5f && Mathf.Abs(verticalInput) > 0.1f)
        {
            sidewaysFriction.stiffness = driftStiffness; // easier to slide
        }
        else
        {
            sidewaysFriction.stiffness = rearStiffness; // normal grip
        }

        rearLeftCollider.sidewaysFriction = sidewaysFriction;
        rearRightCollider.sidewaysFriction = sidewaysFriction;
    }

    void CheckGrounded()
    {
        isGrounded = frontLeftCollider.isGrounded || frontRightCollider.isGrounded ||
                     rearLeftCollider.isGrounded || rearRightCollider.isGrounded;
    }

    void UpdateWheelMeshes()
    {
        UpdateSingleWheel(frontLeftCollider, frontLeftMesh);
        UpdateSingleWheel(frontRightCollider, frontRightMesh);
        UpdateSingleWheel(rearLeftCollider, rearLeftMesh);
        UpdateSingleWheel(rearRightCollider, rearRightMesh);
    }

    void UpdateSingleWheel(WheelCollider collider, Transform mesh)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        mesh.position = pos;
        mesh.rotation = rot;
    }

    void SetupSuspension()
    {
        JointSpring spring = new JointSpring();
        spring.spring = 35000;
        spring.damper = 4500;
        spring.targetPosition = 0.5f;

        frontLeftCollider.suspensionSpring = spring;
        frontRightCollider.suspensionSpring = spring;
        rearLeftCollider.suspensionSpring = spring;
        rearRightCollider.suspensionSpring = spring;
    }
}
