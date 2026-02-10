using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountdownManager : MonoBehaviour
{
    public Image threeImage;
    public Image twoImage;
    public Image oneImage;
    public Image goImage;

    private CarMove playerCar; // we will find it automatically

    void Start()
    {
        threeImage.gameObject.SetActive(false);
        twoImage.gameObject.SetActive(false);
        oneImage.gameObject.SetActive(false);
        goImage.gameObject.SetActive(false);

        StartCoroutine(FindCarAndStart());
    }

    IEnumerator FindCarAndStart()
    {
        // Wait until the car is spawned
        while (playerCar == null)
        {
            playerCar = FindObjectOfType<CarMove>();
            yield return null;
        }

        playerCar.enabled = false; // disable control during countdown

        yield return StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        threeImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        threeImage.gameObject.SetActive(false);

        twoImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        twoImage.gameObject.SetActive(false);

        oneImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        oneImage.gameObject.SetActive(false);

        goImage.gameObject.SetActive(true);

        playerCar.enabled = true; // ENABLE control when GO appears

        yield return new WaitForSeconds(1f);
        goImage.gameObject.SetActive(false);
    }
}
