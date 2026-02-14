using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountdownManager : MonoBehaviour
{
    public Image threeImage;
    public Image twoImage;
    public Image oneImage;
    public Image goImage;

    private MonoBehaviour playerCar; // TEMP generic reference

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
        // Wait until any car controller is found (temporary)
        while (playerCar == null)
        {
            playerCar = FindObjectOfType<MonoBehaviour>();
            yield return null;
        }

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

        yield return new WaitForSeconds(1f);
        goImage.gameObject.SetActive(false);
    }
}
