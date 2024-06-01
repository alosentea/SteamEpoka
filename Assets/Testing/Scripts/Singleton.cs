using System;
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
            //DontDestroyOnLoad(gameObject);
        }

        
        
        
        
        enemyHealth = new int[entityNumber];

        enemyTrigger = new bool[entityNumber];
        playerTrigger = new bool[entityNumber];
        
        facingEnemy = new bool[entityNumber];
        facingPlayer = new bool[entityNumber];
    }
    
    
    
    public int entityNumber;

    public int playerHealth;
    public int[] enemyHealth;
    
    public bool[] enemyTrigger;
    public bool[] playerTrigger;
    
    public bool[] facingEnemy;
    public bool[] facingPlayer;
}
