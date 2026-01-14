using UnityEngine;

public class carSpawner1 : MonoBehaviour
{
    public GameObject carPrefab;
    public Transform startPoint;

    void Start()
    {
        if (carPrefab == null || startPoint == null)
        {
            Debug.LogError("CarSpawner missing references!");
            return;
        }

        Instantiate(carPrefab, startPoint.position, startPoint.rotation);
    }
}
