using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class RaceManager : MonoBehaviour
{
    public static RaceManager I;

    [Header("UI")]
    public TMP_Text countdownText;
    public TMP_Text timerText;
    public GameObject finishPanel;
    public TMP_Text finishTimeText;

    [Header("Race Settings")]
    public float countdownTime = 3f;

    private bool raceStarted = false;
    private bool raceFinished = false;
    private float raceTimer = 0f;

    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        if (finishPanel != null)
            finishPanel.SetActive(false);

        StartCoroutine(StartCountdown());
    }

    private void Update()
    {
        if (!raceStarted || raceFinished) return;

        raceTimer += Time.deltaTime;
        UpdateTimerUI(raceTimer);
    }

    IEnumerator StartCountdown()
    {
        raceStarted = false;
        raceFinished = false;

        // Lock car control
        SetPlayerControl(false);

        float t = countdownTime;

        while (t > 0)
        {
            if (countdownText != null)
                countdownText.text = Mathf.CeilToInt(t).ToString();

            yield return new WaitForSeconds(1f);
            t -= 1f;
        }

        if (countdownText != null)
            countdownText.text = "GO!";

        raceStarted = true;
        raceTimer = 0f;

        // Unlock car control
        SetPlayerControl(true);

        yield return new WaitForSeconds(1f);

        if (countdownText != null)
            countdownText.text = "";
    }

    void UpdateTimerUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int ms = Mathf.FloorToInt((time * 100f) % 100f);

        if (timerText != null)
            timerText.text = $"{minutes:00}:{seconds:00}:{ms:00}";
    }

    public void FinishRace()
    {
        if (raceFinished) return;

        raceFinished = true;
        SetPlayerControl(false);

        if (finishPanel != null)
            finishPanel.SetActive(true);

        if (finishTimeText != null)
            finishTimeText.text = "TIME: " + timerText.text;
    }

    void SetPlayerControl(bool enable)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // your car controller script (change name if needed)
        var car = player.GetComponent<CarMove>();
        if (car != null) car.enabled = enable;

        // stop motion when disabled (prevents rolling)
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (!enable && rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // UI Buttons
    public void RestartRace()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
