using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroUI : MonoBehaviour
{
    public void OpenGarage()
    {
        SceneManager.LoadScene("Garage");
    }

    public void Play()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
