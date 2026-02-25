using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        RaceManager.Instance.ArmFinish();
        Debug.Log("Checkpoint passed — finish armed");
    }
}