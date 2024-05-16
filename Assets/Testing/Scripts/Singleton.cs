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

    public Vector2 playerCoords;
}
