using UnityEngine;

public class Menu : MonoBehaviour
{
    public void Play()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
