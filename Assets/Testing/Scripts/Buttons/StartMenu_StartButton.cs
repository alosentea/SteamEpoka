using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu_StartButton : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Ciudad", LoadSceneMode.Single);
    }
}
