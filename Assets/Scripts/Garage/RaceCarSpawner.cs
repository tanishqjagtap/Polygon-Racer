using UnityEngine;

public class RaceCarSpawner : MonoBehaviour
{
    [Header("Assign All Car Prefabs In Order")]
    public GameObject[] carPrefabs;

    void Start()
    {
        // find spawn point
        GameObject spawn = GameObject.Find("PlayerSpawn");

        if (spawn == null)
        {
            Debug.LogError("❌ PlayerSpawn not found in scene!");
            return;
        }

        int index = CarSelection.selectedCarIndex;

        if (index < 0 || index >= carPrefabs.Length)
            index = 0;

        Instantiate(
            carPrefabs[index],
            spawn.transform.position,
            spawn.transform.rotation
        );
    }
}