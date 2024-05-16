using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    
    // Player Rigidbody2D Component //
    public Rigidbody2D playerRigidbody2D;

    // "Walk" variables //
    public float walkThrust;
    float walkForce;
    public float maxVelocity;
    
    // "Dash" variables //
    public float dashImpulse;
    float timeSinceDash;
    public float dashCooldown;

    // "Jump" variables //
    public float jumpImpulse;
    public float raycastDistance;
    public float jumpYVelocityError;
    public LayerMask rayLayer;
    bool inAir = false;

    // "Fall through platform" variables //
    public Collider2D playerCollider;
    Collider2D platformCollider2D;
    bool falling = false;

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
        singleton.playerCoords = playerRigidbody2D.position;
        
        RaycastHit2D JumpRay = Physics2D.Raycast(transform.position, -transform.up, raycastDistance, rayLayer);
        // Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - raycastDistance), Color.red);

        
        
        void HorizontalMovement()
        {
            walkForce = walkThrust * Time.deltaTime;
            
            if (Input.GetKey(KeyCode.A)) // "Left walk" key
            {
                transform.localEulerAngles = new UnityEngine.Vector3(0, 180, 0);
                walkForce = walkForce * (1 - (-playerRigidbody2D.velocity.x / maxVelocity));
                playerRigidbody2D.AddForce(transform.right * walkForce, ForceMode2D.Force);
            }
            
            walkForce = walkThrust * Time.deltaTime;
                
            if (Input.GetKey(KeyCode.D)) // "Right walk" key
            {
                transform.localEulerAngles = new UnityEngine.Vector3(0, 0, 0);
                walkForce = walkForce * (1 - (playerRigidbody2D.velocity.x / maxVelocity));
                playerRigidbody2D.AddForce(transform.right * walkForce, ForceMode2D.Force);
            }
            
            timeSinceDash += Time.deltaTime;

            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.LeftShift)) // "Left dash" keys
            {
                if (timeSinceDash >= dashCooldown)
                {
                    transform.localEulerAngles = new UnityEngine.Vector3(0, 180, 0);
                    playerRigidbody2D.AddForce(transform.right * dashImpulse, ForceMode2D.Impulse);
                    timeSinceDash = 0;
                }
            }

            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftShift)) // "Right dash" keys
            {
                if (timeSinceDash >= dashCooldown)
                {
                    transform.localEulerAngles = new UnityEngine.Vector3(0, 0, 0);
                    playerRigidbody2D.AddForce(transform.right * dashImpulse, ForceMode2D.Impulse);
                    timeSinceDash = 0;
                }
            }
        }
        void VerticalMovement()
        {
            if (Input.GetKey(KeyCode.Space)) // "Jump" key
            {
                if (JumpRay.collider != null)
                {
                    if (playerRigidbody2D.velocity.y >= 0-jumpYVelocityError && playerRigidbody2D.velocity.y <= 0+jumpYVelocityError)
                    {
                        if (inAir == false)
                        {
                            // Vertical impulse //
                            playerRigidbody2D.AddForce(transform.up * jumpImpulse, ForceMode2D.Impulse);
                            inAir = true;

                            if (falling == true)
                            {
                                // Platform collider reactivation //
                                Physics2D.IgnoreCollision(playerCollider, platformCollider2D, false);
                                falling = false;
                            }
                        }
                    }
                }
                else
                {
                    if(inAir == true)
                    {
                        inAir = false;
                    }
                }
            }
            
            if (Input.GetKey(KeyCode.S)) // "Fall through platform" key
            {
                if (JumpRay.collider != null)
                {
                    if (falling == false)
                    {
                        // Platform collider deactivation //
                        platformCollider2D = JumpRay.collider;
                        Physics2D.IgnoreCollision(playerCollider, platformCollider2D, true);
                        falling = true;
                    }
                }
            }
        }
        
        HorizontalMovement();
        VerticalMovement();
    }
}