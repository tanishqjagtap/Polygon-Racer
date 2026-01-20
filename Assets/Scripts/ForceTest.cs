using UnityEngine;

public class ForceTest : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
            rb.AddForce(Vector3.forward * 50f, ForceMode.Acceleration);
    }
}
