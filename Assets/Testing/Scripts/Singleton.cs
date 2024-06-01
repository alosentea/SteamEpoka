using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
    
    public int entityNumber = 10;
    private static readonly int EntityNumber = 10;

    public int playerHealth;
    public int[] enemyHealth = new int[EntityNumber];
    
    public bool[] enemyTrigger = new bool[EntityNumber];
    public bool[] playerTrigger = new bool[EntityNumber];
    
    public bool[] facingEnemy = new bool[EntityNumber];
    public bool[] facingPlayer = new bool[EntityNumber];
}
