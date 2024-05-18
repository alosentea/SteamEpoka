using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    // SINGLETON //
    private Singleton singleton;
    [SerializeField] private GameObject singletonInstance;

    void Awake()
    {
        // SINGLETON //
        singleton = singletonInstance.GetComponent<Singleton>();
    }


    // Player Rigidbody2D Component //
    public Rigidbody2D playerRigidbody2D;
    
    // Player Animator Component //
    public Animator animator;

    // "Walk" variables //
    public float walkThrust;
    float walkForce;
    public float maxVelocity;
    float stopImpulse;
    int isItWalking;
    bool isItWalkingLeft;
    bool isItWalkingRight;
    
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

    void Update()
    {
        singleton.playerCoords = playerRigidbody2D.position;
        
        RaycastHit2D JumpRay = Physics2D.Raycast(transform.position, -transform.up, raycastDistance, rayLayer);
        // Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - raycastDistance), Color.red);
        
        
        
        void HorizontalMovement()
        {
            walkForce = walkThrust * Time.deltaTime;

            if (isItWalking == 0 )
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_Dash") == false && animator.GetBool("Jumping") == false)
                {
                    stopImpulse = -playerRigidbody2D.velocity.x * playerRigidbody2D.mass;
                    
                    if (stopImpulse < 0)
                    {
                        playerRigidbody2D.AddForce(transform.right * stopImpulse, ForceMode2D.Impulse);
                    }
                    if (stopImpulse > 0)
                    {
                        playerRigidbody2D.AddForce(-transform.right * stopImpulse, ForceMode2D.Impulse);
                    }
                }
                animator.SetBool("Walking", false);
                isItWalking = 2;
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_Dash") == true)
            {
                animator.SetBool("Dashing", false);
            }
            
            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S)) // "Left walk" keys
            {
                transform.localEulerAngles = new UnityEngine.Vector3(0, 180, 0);
                walkForce = walkForce * (1 - (-playerRigidbody2D.velocity.x / maxVelocity));
                playerRigidbody2D.AddForce(transform.right * walkForce, ForceMode2D.Force);
                isItWalkingLeft = true;
                animator.SetBool("Walking", true);
            }
            else
            {
                isItWalkingLeft = false;
            }
            
            walkForce = walkThrust * Time.deltaTime;
                
            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.S)) // "Right walk" keys
            {
                transform.localEulerAngles = new UnityEngine.Vector3(0, 0, 0);
                walkForce = walkForce * (1 - (playerRigidbody2D.velocity.x / maxVelocity));
                playerRigidbody2D.AddForce(transform.right * walkForce, ForceMode2D.Force);
                isItWalkingRight = true;
                animator.SetBool("Walking", true);
            }
            else
            {
                isItWalkingRight = false;
            }
            
            if(isItWalkingLeft == true || isItWalkingRight == true)
            {
                isItWalking = 1;
            }
            else
            {
                isItWalking = 0;
            }
            
            timeSinceDash += Time.deltaTime;

            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.S)) // "Left dash" keys
            {
                if (timeSinceDash >= dashCooldown)
                {
                    transform.localEulerAngles = new UnityEngine.Vector3(0, 180, 0);
                    playerRigidbody2D.AddForce(transform.right * dashImpulse, ForceMode2D.Impulse);
                    timeSinceDash = 0;
                    animator.SetBool("Dashing", true);
                }
            }

            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.S)) // "Right dash" keys
            {
                if (timeSinceDash >= dashCooldown)
                {
                    transform.localEulerAngles = new UnityEngine.Vector3(0, 0, 0);
                    playerRigidbody2D.AddForce(transform.right * dashImpulse, ForceMode2D.Impulse);
                    timeSinceDash = 0;
                    animator.SetBool("Dashing", true);
                }
            }
        }
        void VerticalMovement()
        {
            if (JumpRay.collider != null)
            {
                if (playerRigidbody2D.velocity.y >= 0-jumpYVelocityError && playerRigidbody2D.velocity.y <= 0+jumpYVelocityError)
                {
                    if (inAir == false)
                    {
                        if (Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.S)) // "Jump" keys
                        {
                            // Vertical impulse //
                            playerRigidbody2D.AddForce(transform.up * jumpImpulse, ForceMode2D.Impulse);
                            inAir = true;
                            animator.SetBool("Jumping", true);
                            animator.SetBool("Landed", false);
                        }
                        else
                        {
                            animator.SetBool("Landed", true);
                            animator.SetBool("Jumping", false);
                            animator.SetBool("Cayendo", false);
                        }
                        
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
            
            if (Input.GetKey(KeyCode.S) && (animator.GetBool("Cayendo") == false)) // "Squat" key
            {
                animator.SetBool("Agachado", true);
            }
            else
            {
                if (animator.GetBool("Cayendo") == false)
                {
                    animator.SetBool("Agachado", false);
                }
            }

            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.Space) && (animator.GetBool("Cayendo") == false)) // "Fall through platform" keys
            {
                if (JumpRay.collider != null)
                {
                    if (falling == false)
                    {
                        // Platform collider deactivation //
                        platformCollider2D = JumpRay.collider;
                        Physics2D.IgnoreCollision(playerCollider, platformCollider2D, true);
                        falling = true;
                        animator.SetBool("Agachado", true);
                        animator.SetBool("Cayendo", true);
                    }
                }
            }
        }
        
        HorizontalMovement();
        VerticalMovement();
    }
}