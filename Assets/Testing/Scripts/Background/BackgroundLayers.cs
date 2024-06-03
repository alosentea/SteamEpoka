using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BackgroundLayers : MonoBehaviour
{
    public Rigidbody2D layerRigidbody2D;
    public Rigidbody2D playerRigidbody2D;
    public GameObject cameraGameObject;
    public float layerVelocity;
    public float layerYOffset;

    void FixedUpdate()
    {
        layerRigidbody2D.velocity = new Vector2(playerRigidbody2D.velocity.x * layerVelocity * 0.5f, 0);
        transform.position = new Vector3(transform.position.x, cameraGameObject.transform.position.y + layerYOffset, transform.position.z);
        
        if (cameraGameObject.transform.position.x - transform.position.x >= 16)
        {
            var Clone = Instantiate(gameObject, new Vector3(transform.position.x + 31.96f, transform.position.y, transform.position.z), Quaternion.identity);
            Clone.name = gameObject.name;
            Destroy(gameObject);
        }
        if (cameraGameObject.transform.position.x - transform.position.x <= -16)
        {
            var Clone = Instantiate(gameObject, new Vector3(transform.position.x - 31.96f, transform.position.y, transform.position.z), Quaternion.identity);
            Clone.name = gameObject.name;
            Destroy(gameObject);
        }
    }
}
