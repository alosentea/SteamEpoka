using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayMenu_MainMenuButton : MonoBehaviour
{
    // SINGLETON //
    private Singleton _singleton;
    [SerializeField] private GameObject singletonInstance;
    
    public void MainMenu()
    {
        _singleton = GameObject.FindWithTag("Singleton").GetComponent<Singleton>();
        
        SceneManager.LoadScene("TestScene_Menu", LoadSceneMode.Single);
        _singleton.mainMenuButtonEnabled = true;
    }
}
