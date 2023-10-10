using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnLocalClicked()
    {
        SceneManager.LoadScene("LocalGame");
    }

    public void OnOnlineClicked()
    {
        SceneManager.LoadScene("Lobby");
    }
}
