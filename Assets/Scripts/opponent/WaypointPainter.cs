using UnityEngine;

// Attach this to any GameObject, then click your road in Scene view to drop waypoints
public class WaypointPainter : MonoBehaviour
{
    [Header("Drop waypoints by pressing G in Scene view while hovering your road")]
    public GameObject waypointPrefab;   // assign a simple sphere prefab
    public Transform waypointParent;    // empty GameObject to hold all waypoints

    private void OnDrawGizmos()
    {
        if (waypointParent == null) return;

        // Draw lines between all waypoints so you can see the path
        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypointParent.childCount; i++)
        {
            Transform a = waypointParent.GetChild(i);
            Transform b = waypointParent.GetChild((i + 1) % waypointParent.childCount);
            Gizmos.DrawLine(a.position, b.position);
            Gizmos.DrawSphere(a.position, 0.5f);
        }
    }
}