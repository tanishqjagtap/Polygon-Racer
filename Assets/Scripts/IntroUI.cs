using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroUI : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("Quit pressed");
    }

    public void CarSelect()
    {
        Debug.Log("Car Select coming soon!");
    }
}
