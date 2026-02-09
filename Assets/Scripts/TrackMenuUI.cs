using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackMenuUI : MonoBehaviour
{
    public void LoadOval()
    {
        SceneManager.LoadScene("Oval");
    }
    public void LoadIntroUI()
    {
        SceneManager.LoadScene("Intro");
    }
    public void LoadKingsdown()
    {
        SceneManager.LoadScene("Kingsdown");
    }
    public void LoadFrance()
    {
        SceneManager.LoadScene("France");
    }


}
    