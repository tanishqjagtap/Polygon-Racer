using UnityEngine;

public class GarageManager : MonoBehaviour
{
    [Header("Assign Car Prefabs In Order")]
    public GameObject[] carPrefabs;

    [Header("Spawn Point")]
    public Transform carPoint;

    private GameObject currentCar;
    private int currentIndex;

    void Start()
    {
        currentIndex = CarSelection.selectedCarIndex;
        SpawnCar();
    }

    void SpawnCar()
    {
        if (currentCar != null)
            Destroy(currentCar);

        currentCar = Instantiate(
            carPrefabs[currentIndex],
            carPoint.position,
            carPoint.rotation
        );
    }

    public void NextCar()
    {
        currentIndex++;
        if (currentIndex >= carPrefabs.Length)
            currentIndex = 0;

        SpawnCar();
    }

    public void PreviousCar()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = carPrefabs.Length - 1;

        SpawnCar();
    }

    public void SelectCar()
    {
        CarSelection.selectedCarIndex = currentIndex;
        Debug.Log("Selected car index: " + currentIndex);
    }
}