using UnityEngine;

public class CarController : MonoBehaviour
{
    public float forwardForce = 3f;
    public float turnSpeed = 20f;

    Rigidbody rb;
    1
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * forwardForce);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0f, -turnSpeed * Time.deltaTime, 0f);
        }
        if(Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0f, turnSpeed * Time.deltaTime, 0f);
        }
    }
}
