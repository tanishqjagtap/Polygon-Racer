using UnityEngine;
using TMPro;

public class SpeedometerUI : MonoBehaviour
{
    public Rigidbody carRb;
    public TextMeshProUGUI speedText;

    void Update()
    {
        if (!carRb || !speedText) return;

        // Unity speed is m/s. Convert to km/h
        float speedKmh = carRb.linearVelocity.magnitude * 3.6f;

        speedText.text = Mathf.RoundToInt(speedKmh) + " KM/H";
    }
}
