using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // SINGLETON //
    private Singleton singleton;
    [SerializeField] private GameObject singletonInstance;

    void Awake()
    {
        // SINGLETON //
        singleton = singletonInstance.GetComponent<Singleton>();
    }


    Vector3 playerCoords;
    float distance = 0;
    float velocity = 0;
    public float pow;
    public float div;
    public float sum;
    public float mul;

    void FixedUpdate()
    {
        void PlayerFollow()
        {
            playerCoords = new Vector3(singleton.playerCoords.x, singleton.playerCoords.y, -10.0f);
            
            distance = Vector3.Distance(transform.position, playerCoords);
            velocity = (Mathf.Pow(distance, pow) / div) + sum;
            velocity = velocity * Time.fixedDeltaTime * mul;

            transform.position = Vector3.MoveTowards(transform.position, playerCoords, velocity);
        }

        PlayerFollow();
    }
}