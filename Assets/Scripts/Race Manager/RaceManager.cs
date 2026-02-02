using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    public bool passedCheckpoint = false;
    private bool raceFinished = false;

    private Vector3 lastCheckpointPos;
    private Quaternion lastCheckpointRot;

    private void Awake()
    {
        Instance = this;
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        lastCheckpointPos = checkpoint.position;
        lastCheckpointRot = checkpoint.rotation;
        passedCheckpoint = true;
    }

    public void ResetToCheckpoint(GameObject car)
    {
        if (!passedCheckpoint || raceFinished) return;

        Rigidbody rb = car.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        car.transform.position = lastCheckpointPos + Vector3.up * 0.5f;
        car.transform.rotation = lastCheckpointRot;
    }

    public void FinishRace(GameObject car)
    {
        if (raceFinished) return;
        raceFinished = true;

        Debug.Log("RACE FINISHED!");

        var controller = car.GetComponent<CarMove>();
        if (controller != null)
            controller.enabled = false;

        Rigidbody rb = car.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
