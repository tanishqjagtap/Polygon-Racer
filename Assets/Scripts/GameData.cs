using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData I;
    public int selectedCar = 0;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);
    }
}
