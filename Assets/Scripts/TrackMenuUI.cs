using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackMenuUI : MonoBehaviour
{
    public void LoadSunnyTrack()
    {
        SceneManager.LoadScene("Track_Sunny");
    }
    public void LoadIntroUI()
    {
        SceneManager.LoadScene("Intro");
    }
}
    