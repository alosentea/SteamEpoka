using UnityEngine;

public class GameplayMenu_ContinueButton : MonoBehaviour
{
    // SINGLETON //
    private Singleton _singleton;
    [SerializeField] private GameObject singletonInstance;
    
    // CANVAS //
    public Canvas gameplayMenu;
    
    public void Continue()
    {
        _singleton = GameObject.FindWithTag("Singleton").GetComponent<Singleton>();
        
        _singleton.continueButtonEnabled = true;
        Destroy(gameplayMenu.gameObject);
    }
}
