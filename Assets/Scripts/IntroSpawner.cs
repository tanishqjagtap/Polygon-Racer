using UnityEngine;

public class IntroCarSpawner : MonoBehaviour
{
    public Transform carPoint;
    public GameObject[] carPrefabs;
    public float rotateSpeed = 25f;

    [Header("Auto Fit Settings")]
    public float targetSize = 3.0f;
    public float groundOffset = 0.05f;

    GameObject currentCar;

    void Start()
    {
        int index = PlayerPrefs.GetInt("SelectedCarIndex", 0);
        index = Mathf.Clamp(index, 0, carPrefabs.Length - 1);

        currentCar = Instantiate(carPrefabs[index], carPoint);
        currentCar.transform.localPosition = Vector3.zero;
        currentCar.transform.localRotation = Quaternion.identity;
        currentCar.transform.localScale = Vector3.one;

        AutoFitCar(currentCar);
    }

    void Update()
    {
        if (currentCar != null)
            currentCar.transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, Space.Self);
    }

    void AutoFitCar(GameObject car)
    {
        Renderer[] renderers = car.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        float biggestSide = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float scaleFactor = targetSize / biggestSide;

        car.transform.localScale *= scaleFactor;

        // recalc bounds after scale
        renderers = car.GetComponentsInChildren<Renderer>();
        bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        // lift up to sit on podium
        float bottomY = bounds.min.y;
        float deltaY = carPoint.position.y - bottomY + groundOffset;
        car.transform.position += new Vector3(0f, deltaY, 0f);

        // center it on podium
        Vector3 centerOffset = bounds.center - carPoint.position;
        car.transform.position -= new Vector3(centerOffset.x, 0f, centerOffset.z);
    }
}

