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
    public void LoadMountain_Track()
    {
        SceneManager.LoadScene("Mountain_Track");
    }
    public void LoadNight_track()
    {
        SceneManager.LoadScene("Night_track");
    }


}
    