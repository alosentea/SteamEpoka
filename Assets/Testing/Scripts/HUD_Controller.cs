using UnityEngine;

public class HUD_Controller : MonoBehaviour
{
    // SINGLETON //
    private Singleton _singleton;
    [SerializeField] private GameObject singletonInstance;
    
    // HUD ANIMATOR //
    public Animator hud;
    
    void Awake()
    {
        _singleton = GameObject.FindWithTag("Singleton").GetComponent<Singleton>();
    }

    private void Update()
    {
        hud.SetFloat("Percentage", _singleton.playerO2*100/_singleton.maxO2);
    }
}
