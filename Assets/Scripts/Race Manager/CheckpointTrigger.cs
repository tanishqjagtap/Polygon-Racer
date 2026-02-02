using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RaceManager.Instance.SetCheckpoint(transform);
            Debug.Log("Checkpoint Reached!");
        }
    }
}
