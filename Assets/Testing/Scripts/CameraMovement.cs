using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // SINGLETON //
    private Singleton _singleton;
    [SerializeField] private GameObject singletonInstance;

    void Awake()
    {
        // SINGLETON //
        _singleton = singletonInstance.GetComponent<Singleton>();
    }


    Vector3 playerCoords;
    float _distance = 0;
    float _velocity = 0;
    public float pow;
    public float div;
    public float sum;
    public float mul;

    void FixedUpdate()
    {
        void PlayerFollow()
        {
            playerCoords = new Vector3(_singleton.playerCoords.x, _singleton.playerCoords.y, -10.0f);
            
            _distance = Vector3.Distance(transform.position, playerCoords);
            _velocity = (Mathf.Pow(_distance, pow) / div) + sum;
            _velocity = _velocity * Time.fixedDeltaTime * mul;

            transform.position = Vector3.MoveTowards(transform.position, playerCoords, _velocity);
        }

        PlayerFollow();
    }
}