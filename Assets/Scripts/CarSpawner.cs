using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public Transform startPoint;
    public Rigidbody carRb;

    void Start()
    {
        if (startPoint == null) return;

        // place car
        transform.position = startPoint.position;
        transform.rotation = startPoint.rotation;

        // stop any old velocity
        if (carRb != null)
        {
            carRb.linearVelocity = Vector3.zero;
            carRb.angularVelocity = Vector3.zero;
        }
    }
}
