using UnityEngine;

public class GarageCarSelector : MonoBehaviour
{
    [Header("Spawn")]
    public Transform carPoint;

    [Header("Car Prefabs (Drag here)")]
    public GameObject[] carPrefabs;

    [Header("Rotation")]
    public float rotateSpeed = 25f;

    private int index = 0;
    private GameObject currentCar;

    void Start()
    {
        SpawnCar();
    }

    void Update()
    {
        if (currentCar != null)
        {
            currentCar.transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, Space.World);
        }
    }

    void SpawnCar()
    {
        if (carPrefabs == null || carPrefabs.Length == 0 || carPoint == null) return;

        if (currentCar != null)
            Destroy(currentCar);

        currentCar = Instantiate(carPrefabs[index], carPoint.position, carPoint.rotation);
    }

    public void NextCar()
    {
        index++;
        if (index >= carPrefabs.Length) index = 0;
        SpawnCar();
    }

    public void PrevCar()
    {
        index--;
        if (index < 0) index = carPrefabs.Length - 1;
        SpawnCar();
    }
}
