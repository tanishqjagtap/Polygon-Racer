using UnityEngine;
using UnityEngine.SceneManagement;

public class GarageCarSelector : MonoBehaviour
{
    [Header("Spawn")]
    public Transform carPoint;

    [Header("Car Prefabs (Drag here)")]
    public GameObject[] carPrefabs;

    [Header("Rotation")]
    public float rotateSpeed = 25f;

    [Header("Auto Fit Settings")]
    public float targetSize = 3.0f;
    public float groundOffset = 0.05f;

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
            // rotate smoothly around itself
            currentCar.transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, Space.Self);
        }
    }

    void SpawnCar()
    {
        if (carPrefabs == null || carPrefabs.Length == 0 || carPoint == null) return;

        if (currentCar != null)
            Destroy(currentCar);

        currentCar = Instantiate(carPrefabs[index], carPoint);
        currentCar.transform.localPosition = Vector3.zero;
        currentCar.transform.localScale = Vector3.one;

        // ✅ Apply rotation fix per car (if exists)
        CarGarageFix fix = currentCar.GetComponent<CarGarageFix>();
        if (fix != null)
            currentCar.transform.localRotation = Quaternion.Euler(fix.rotationOffset);
        else
            currentCar.transform.localRotation = Quaternion.identity;

        // ✅ Auto scale + place properly
        AutoFitCar(currentCar);
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

    public void NextCar()
    {
        index = (index + 1) % carPrefabs.Length;
        SpawnCar();
    }

    public void PrevCar()
    {
        index--;
        if (index < 0) index = carPrefabs.Length - 1;
        SpawnCar();
    }
    public void SelectCar()
    {
        PlayerPrefs.SetInt("SelectedCarIndex", index);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Intro"); // <-- put your Intro scene name here
    }
}
