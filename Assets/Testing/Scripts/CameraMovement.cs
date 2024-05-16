using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Vector3 playerCoords;
    float distance = 0;
    float velocity = 0;
    public float pow = 2.0f;
    public float div = 100.0f;
    public float sum = 0.0f;
    
    // SINGLETON //
    private Singleton singleton;
    [SerializeField] private GameObject singletonInstance;

    void Awake()
    {
        // SINGLETON //
        singleton = singletonInstance.GetComponent<Singleton>();
    }

    void Update()
    {
        void ArrowFollow()
        {
            playerCoords = new Vector3(singleton.playerCoords.x, singleton.playerCoords.y, -10.0f);
            
            distance = Vector3.Distance(transform.position, playerCoords);
            velocity = (Mathf.Pow(distance, pow) / div) + sum;

            transform.position = Vector3.MoveTowards(transform.position, playerCoords, velocity);
        }

        ArrowFollow();
    }
}