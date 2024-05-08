using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = System.Numerics.Vector3;

public class Movement : MonoBehaviour
{

    public Rigidbody2D PlayerRigidbody2D;

    public float walkThrust;
    float walkForce;
    public float maxVelocity;

    public float jumpImpulse;
    public float raycastDistance;
    public float jumpYVelocityError;
    public LayerMask rayLayer;
    int inAir = 0;

    public Collider2D PlayerCollider;
    Collider2D PlatformCollider;
    bool falling = false;

    void Start()
    {
        
    }

    void Update()
    {
        //Debug.Log(PlayerRigidbody2D.velocity.x);
        walkForce = walkThrust * Time.deltaTime;

        RaycastHit2D JumpRay = Physics2D.Raycast(transform.position, -transform.up, raycastDistance, rayLayer);
        Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - raycastDistance), Color.red);
        //Debug.Log(JumpRay.collider);

        void ForceMovement()
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.localEulerAngles = new UnityEngine.Vector3(0, 180, 0);
                walkForce = walkForce * (1 - (-PlayerRigidbody2D.velocity.x / maxVelocity));
                PlayerRigidbody2D.AddForce(transform.right * walkForce, ForceMode2D.Force);
            }
                
            if (Input.GetKey(KeyCode.D))
            {
                transform.localEulerAngles = new UnityEngine.Vector3(0, 0, 0);
                walkForce = walkForce * (1 - (PlayerRigidbody2D.velocity.x / maxVelocity));
                PlayerRigidbody2D.AddForce(transform.right * walkForce, ForceMode2D.Force);
            }

            if (Input.GetKey(KeyCode.Space))
            {
                if (JumpRay.collider != null)
                {
                    if (PlayerRigidbody2D.velocity.y >= 0-jumpYVelocityError && PlayerRigidbody2D.velocity.y <= 0+jumpYVelocityError)
                    {
                        if (inAir == 0)
                        {
                            PlayerRigidbody2D.AddForce(transform.up * jumpImpulse, ForceMode2D.Impulse);
                            inAir = 1;

                            if (falling == true)
                            {
                                Physics2D.IgnoreCollision(PlayerCollider, PlatformCollider, false);
                                falling = false;
                            }
                        }
                    }
                }
                else
                {
                    if(inAir == 1)
                    {
                        inAir = 0;
                    }
                }
            }

            if (Input.GetKey(KeyCode.S))
            {
                if (JumpRay.collider != null)
                {
                    if (falling == false)
                    {
                        PlatformCollider = JumpRay.collider;
                        Physics2D.IgnoreCollision(PlayerCollider, PlatformCollider, true);
                        falling = true;
                    }
                }
            }
        }



        ForceMovement();
    }
}