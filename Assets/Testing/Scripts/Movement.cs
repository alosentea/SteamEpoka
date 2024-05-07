using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = System.Numerics.Vector3;

public class Movement : MonoBehaviour
{

    public Rigidbody2D PlayerRigidbody2D;

    public float walkThrust;
    float walkForce;
    public float jumpForce;
    
    public float maxVelocity;

    void Start()
    {
        
    }

    void Update()
    {
        walkForce = walkThrust * Time.deltaTime;

        void ForceMovement()
        {
            if (PlayerRigidbody2D.velocity.x <= maxVelocity)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    transform.localEulerAngles = new UnityEngine.Vector3(0, 180, 0);
                    PlayerRigidbody2D.AddForce(transform.right * walkForce, ForceMode2D.Force);
                }
                
                if (Input.GetKey(KeyCode.D))
                {
                    transform.localEulerAngles = new UnityEngine.Vector3(0, 0, 0);
                    PlayerRigidbody2D.AddForce(transform.right * walkForce, ForceMode2D.Force);
                }
            }
            else
            {
                PlayerRigidbody2D.AddForce(transform.right * -(PlayerRigidbody2D.mass*(PlayerRigidbody2D.velocity.x-maxVelocity)), ForceMode2D.Force);
                Debug.Log(PlayerRigidbody2D.velocity.x);
            }
            
            if (PlayerRigidbody2D.velocity.x >= -maxVelocity)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    transform.localEulerAngles = new UnityEngine.Vector3(0, 180, 0);
                    PlayerRigidbody2D.AddForce(transform.right * walkForce, ForceMode2D.Force);
                }
                
                if (Input.GetKey(KeyCode.D))
                {
                    transform.localEulerAngles = new UnityEngine.Vector3(0, 0, 0);
                    PlayerRigidbody2D.AddForce(transform.right * walkForce, ForceMode2D.Force);
                }
            }
            else
            {
                PlayerRigidbody2D.AddForce(transform.right * (PlayerRigidbody2D.mass*(PlayerRigidbody2D.velocity.x-(-maxVelocity))), ForceMode2D.Force);
                Debug.Log(PlayerRigidbody2D.velocity.x);
            }

            //if (PlayerIsNotInTheAir)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    PlayerRigidbody2D.AddForce(transform.up * jumpForce, ForceMode2D.Force);
                }
            }
        }



        ForceMovement();
    }
}