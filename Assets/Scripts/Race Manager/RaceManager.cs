using UnityEngine;
using System.Collections;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    [Header("Race State")]
    public bool passedCheckpoint = false;
    private bool raceFinished = false;

    private Vector3 lastCheckpointPos;
    private Quaternion lastCheckpointRot;

    [Header("Countdown Images")]
    public GameObject threeImage;
    public GameObject twoImage;
    public GameObject oneImage;
    public GameObject goImage;

    [Header("Countdown Audio")]
    public AudioSource audioSource;
    public AudioClip tickSound;
    public AudioClip goSound;

    private CarMove playerCar;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Find player car and lock movement
        playerCar = FindObjectOfType<CarMove>();
        if (playerCar != null)
            playerCar.enabled = false;

        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        // 3
        threeImage.SetActive(true);
        audioSource.PlayOneShot(tickSound);
        yield return new WaitForSeconds(1f);
        threeImage.SetActive(false);

        // 2
        twoImage.SetActive(true);
        audioSource.PlayOneShot(tickSound);
        yield return new WaitForSeconds(1f);
        twoImage.SetActive(false);

        // 1
        oneImage.SetActive(true);
        audioSource.PlayOneShot(tickSound);
        yield return new WaitForSeconds(1f);
        oneImage.SetActive(false);

        // GO
        goImage.SetActive(true);
        audioSource.PlayOneShot(goSound);

        // Unlock car
        if (playerCar != null)
            playerCar.enabled = true;

        yield return new WaitForSeconds(1f);
        goImage.SetActive(false);
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        lastCheckpointPos = checkpoint.position;
        lastCheckpointRot = checkpoint.rotation;
        passedCheckpoint = true;
    }

    public void ResetToCheckpoint(GameObject car)
    {
        if (!passedCheckpoint || raceFinished) return;

        Rigidbody rb = car.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        car.transform.position = lastCheckpointPos + Vector3.up * 0.5f;
        car.transform.rotation = lastCheckpointRot;
    }

    public void FinishRace(GameObject car)
    {
        if (raceFinished) return;
        raceFinished = true;

        Debug.Log("RACE FINISHED!");

        var controller = car.GetComponent<CarMove>();
        if (controller != null)
            controller.enabled = false;

        Rigidbody rb = car.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
