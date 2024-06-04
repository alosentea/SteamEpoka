using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Singleton : MonoBehaviour
{
    
    public static int singletonInstancesNumber;

    
    
    
    
    void Awake()
    {
        singletonInstancesNumber++;

        if(singletonInstancesNumber > 1)
        {
            singletonInstancesNumber--;
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        
        
        
        playerDamage = 0;
        enemyHealth = new int[entityNumber];

        playerO2 = 1000;

        enemyTrigger = new bool[entityNumber];
        playerTrigger = new bool[entityNumber];
        
        facingEnemy = new bool[entityNumber];
        facingPlayer = new bool[entityNumber];
    }





    public Canvas gameplayMenu;
    private bool _isInstantiated;
    
    private void Update()
    {
        if (_auxO2Time >= 1)
        {
            playerO2 -= playerDamage;
            _auxO2Time = 0;
        }

        _auxO2Time += Time.deltaTime;

        if (Input.GetKey(KeyCode.Escape) && !_isInstantiated && SceneManager.GetActiveScene().name == "TestScene_001")
        {
            Time.timeScale = 0;
            var canvas = Instantiate(gameplayMenu);
            canvas.name = "Gameplay Menu";
            _isInstantiated = true;
        }
    }


    
    
    
    public int entityNumber;

    public int playerDamage;
    public int[] enemyHealth;

    public float playerO2;
    private float _auxO2Time;
    
    public bool[] enemyTrigger;
    public bool[] playerTrigger;
    
    public bool[] facingEnemy;
    public bool[] facingPlayer;
}
