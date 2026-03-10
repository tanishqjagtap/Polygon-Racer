using UnityEngine;
using System.Collections;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    [Header("Race State")]
    public bool raceStarted = false;

    [Header("Checkpoint State")]
    private bool finishArmed = false;
    public bool raceFinished = false;

    [Header("Countdown Images")]
    public GameObject threeImage;
    public GameObject twoImage;
    public GameObject oneImage;
    public GameObject goImage;

    [Header("Countdown Audio")]
    public AudioSource audioSource;
    public AudioClip tickSound;
    public AudioClip goSound;

    [Header("Player Reference (AUTO FOUND)")]
    public Car playerCar;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(InitializeRace());
    }

    IEnumerator InitializeRace()
    {
        yield return new WaitForSeconds(0.5f);

        if (playerCar == null)
            playerCar = FindFirstObjectByType<Car>();

        if (playerCar != null)
            playerCar.canDrive = false;

        raceStarted = false;

        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        // 3
        threeImage.SetActive(true);
        if (tickSound) audioSource.PlayOneShot(tickSound);
        yield return new WaitForSeconds(1f);
        threeImage.SetActive(false);

        // 2
        twoImage.SetActive(true);
        if (tickSound) audioSource.PlayOneShot(tickSound);
        yield return new WaitForSeconds(1f);
        twoImage.SetActive(false);

        // 1
        oneImage.SetActive(true);
        if (tickSound) audioSource.PlayOneShot(tickSound);
        yield return new WaitForSeconds(1f);
        oneImage.SetActive(false);

        // GO
        goImage.SetActive(true);
        if (goSound) audioSource.PlayOneShot(goSound);

        raceStarted = true;

        if (playerCar != null)
            playerCar.canDrive = true;

        yield return new WaitForSeconds(1f);
        goImage.SetActive(false);
    }

    // called by middle checkpoint
    public void ArmFinish()
    {
        if (raceFinished) return;

        finishArmed = true;
        Debug.Log("Finish armed");
    }

    // called by finish line
    public void FinishRace()
    {
        if (raceFinished) return;
        if (!finishArmed) return;

        raceFinished = true;

        Debug.Log("RACE FINISHED!");

        StartCoroutine(SlowStopCar());
    }

    IEnumerator SlowStopCar()
    {
        if (playerCar == null) yield break;

        Rigidbody rb = playerCar.GetComponent<Rigidbody>();

        float t = 0f;
        while (rb.linearVelocity.magnitude > 0.5f && t < 4f)
        {
            rb.linearVelocity *= 0.96f;
            yield return new WaitForFixedUpdate();
            t += Time.fixedDeltaTime;
        }

        rb.linearVelocity = Vector3.zero;
        playerCar.canDrive = false;
    }
}