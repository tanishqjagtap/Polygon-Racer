using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public Transform startPoint;

    void Start()
    {
        // Get saved car index from Garage
        int index = PlayerPrefs.GetInt("SelectedCarIndex", 0);

        GameObject car = Instantiate(carPrefabs[index], startPoint.position, startPoint.rotation);
        car.tag = "Player";

        // Connect camera automatically
        CarCameraFollowNFS cam = FindObjectOfType<CarCameraFollowNFS>();
        if (cam != null)
            cam.target = car.transform;
    }
}
