using UnityEngine;

public class FinishLineTrigger : MonoBehaviour
{
    private bool finished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (finished) return;
        if (!other.CompareTag("Player")) return;

        // 🚫 Do nothing if checkpoint not passed yet
        if (!RaceManager.Instance.passedCheckpoint)
        {
            Debug.Log("Finish line crossed too early!");
            return;
        }

        finished = true;
        Debug.Log("FINISH LINE CROSSED!");

        // Disable player control only
        var controller = other.GetComponent<CarMove>();
        if (controller != null)
            controller.enabled = false;
    }
}
