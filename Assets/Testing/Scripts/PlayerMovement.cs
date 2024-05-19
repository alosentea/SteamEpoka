using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    // SINGLETON //
    private Singleton _singleton;
    [SerializeField] private GameObject singletonInstance;
    
    // Player Rigidbody2D Component //
    public Rigidbody2D playerRigidbody2D;
    
    // Player Animator Component //
    public Animator animator;

    // "Walk" variables //
    public float walkThrust;
    private float _walkForce;
    public float maxVelocity;
    private float _stopImpulse;
    private int _isItWalking;
    private bool _isItWalkingLeft;
    private bool _isItWalkingRight;
    
    // "Dash" variables //
    public float dashImpulse;
    private float _timeSinceDash;
    public float dashCooldown;

    // "Jump" variables //
    public float jumpImpulse;
    public float jumpThrust;
    private float _jumpForce;
    private float _timeAfterJump;
    private float _timeFalling;
    private float _cancelFallingVelocityImpulse;
    public float coyoteTime;
    public float maxTimeAfterJump;
    public float raycastDistance;
    public float jumpYVelocityError;
    public LayerMask rayLayer;
    private bool _inAir = false;
    private bool _jumpKeysHolded;

    // "Fall through platform" variables //
    public Collider2D playerCollider;
    Collider2D _platformCollider2D;
    private bool _droppingFromPlatform = false;
    
    // Player state machine variables //
    private bool _stateMachineWalking;
    private bool _stateMachineDashing;
    private bool _stateMachineJumping;
    private bool _stateMachineLanded;
    private bool _stateMachineSquatting;
    private bool _stateMachineDropping;
    private bool _stateMachineFalling;

    void Awake()
    {
        _singleton = singletonInstance.GetComponent<Singleton>();
    }

    void Update()
    {
        _singleton.playerCoords = playerRigidbody2D.position;
        
        HorizontalMovement();
        VerticalMovement();
    }
    
    void HorizontalMovement()
        {
            _walkForce = walkThrust * Time.deltaTime;

            if (_isItWalking == 0 )
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_Dash") == false && _stateMachineJumping == false && _stateMachineFalling == false)
                {
                    CancelWalkingVelocity();
                }
                animator.SetBool("Walking", false);
                _stateMachineWalking = false;
                
                _isItWalking = 2;
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_Dash") == true)
            {
                animator.SetBool("Dashing", false);
                _stateMachineDashing = false;
            }
            
            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.S)) // "Left walk" keys
            {
                ApplyLeftWalkingForce();
                _isItWalkingLeft = true;
                animator.SetBool("Walking", true);
                _stateMachineWalking = true;
            }
            else
            {
                _isItWalkingLeft = false;
            }
            
            _walkForce = walkThrust * Time.deltaTime;
                
            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S)) // "Right walk" keys
            {
                ApplyRightWalkingForce();
                _isItWalkingRight = true;
                animator.SetBool("Walking", true);
                _stateMachineWalking = true;
            }
            else
            {
                _isItWalkingRight = false;
            }
            
            if(_isItWalkingLeft == true || _isItWalkingRight == true)
            {
                _isItWalking = 1;
            }
            else
            {
                _isItWalking = 0;
            }
            
            _timeSinceDash += Time.deltaTime;

            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.S)) // "Left dash" keys
            {
                if (_timeSinceDash >= dashCooldown)
                {
                    ApplyLeftDashingImpulse();
                    _timeSinceDash = 0;
                    animator.SetBool("Dashing", true);
                    _stateMachineDashing = true;
                }
            }

            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S)) // "Right dash" keys
            {
                if (_timeSinceDash >= dashCooldown)
                {
                    ApplyRightDashingImpulse();
                    _timeSinceDash = 0;
                    animator.SetBool("Dashing", true);
                    _stateMachineDashing = true;
                }
            }
        }
    
    private void ApplyRightDashingImpulse()
    {
        transform.localEulerAngles = new UnityEngine.Vector3(0, 0, 0);
        playerRigidbody2D.AddForce(transform.right * dashImpulse, ForceMode2D.Impulse);
    }
    private void ApplyLeftDashingImpulse()
    {
        transform.localEulerAngles = new UnityEngine.Vector3(0, 180, 0);
        playerRigidbody2D.AddForce(transform.right * dashImpulse, ForceMode2D.Impulse);
    }
    private void ApplyRightWalkingForce()
    {
        transform.localEulerAngles = new UnityEngine.Vector3(0, 0, 0);
        _walkForce = _walkForce * (1 - (playerRigidbody2D.velocity.x / maxVelocity));
        playerRigidbody2D.AddForce(transform.right * _walkForce, ForceMode2D.Force);
    }
    private void ApplyLeftWalkingForce()
    {
        transform.localEulerAngles = new UnityEngine.Vector3(0, 180, 0);
        _walkForce = _walkForce * (1 - (-playerRigidbody2D.velocity.x / maxVelocity));
        playerRigidbody2D.AddForce(transform.right * _walkForce, ForceMode2D.Force);
    }
    private void CancelWalkingVelocity()
    {
        _stopImpulse = -playerRigidbody2D.velocity.x * playerRigidbody2D.mass;
                    
        if (_stopImpulse < 0)
        {
            playerRigidbody2D.AddForce(transform.right * _stopImpulse, ForceMode2D.Impulse);
        }
        if (_stopImpulse > 0)
        {
            playerRigidbody2D.AddForce(-transform.right * _stopImpulse, ForceMode2D.Impulse);
        }
    }
    
    void VerticalMovement()
        {
            RaycastHit2D jumpRay = Physics2D.Raycast(transform.position, -transform.up, raycastDistance, rayLayer);
            // Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - raycastDistance), Color.red);
            
            if (jumpRay.collider != null)
            {
                if (playerRigidbody2D.velocity.y >= 0-jumpYVelocityError && playerRigidbody2D.velocity.y <= 0+jumpYVelocityError)
                {
                    if (_inAir == false)
                    {
                        if (Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.S)) // "Jump" keys
                        {
                            if (_jumpKeysHolded == false)
                            {
                                // Vertical impulse //
                                playerRigidbody2D.AddForce(transform.up * jumpImpulse, ForceMode2D.Impulse);
                                _inAir = true;
                                _timeAfterJump = 0;
                                _jumpKeysHolded = true;
                            
                                animator.SetBool("Jumping", true);
                                _stateMachineJumping = true;
                                animator.SetBool("Landed", false);
                                _stateMachineLanded = false;
                            }
                            else
                            {
                                animator.SetBool("Landed", true);
                                _stateMachineLanded = true;
                                animator.SetBool("Jumping", false);
                                _stateMachineJumping = false;
                                animator.SetBool("Dropping", false);
                                _stateMachineDropping = false;
                            }
                        }
                        else
                        {
                            animator.SetBool("Landed", true);
                            _stateMachineLanded = true;
                            animator.SetBool("Jumping", false);
                            _stateMachineJumping = false;
                            animator.SetBool("Dropping", false);
                            _stateMachineDropping = false;
                            
                            _jumpKeysHolded = false;
                        }
                        
                        if (_droppingFromPlatform == true)
                        {
                            // Platform collider reactivation //
                            Physics2D.IgnoreCollision(playerCollider, _platformCollider2D, false);
                            _droppingFromPlatform = false;
                        }
                    }
                }
                animator.SetBool("Falling", false);
                _stateMachineFalling = false;
            }
            else
            {
                if(_inAir == true)
                {
                    _inAir = false;
                }
                
                if (_stateMachineJumping == false && _stateMachineLanded == true && _stateMachineSquatting == false && _stateMachineDropping == false && _stateMachineFalling == false)
                {
                    animator.SetBool("Falling", true);
                    _stateMachineFalling = true;
                    
                    _timeFalling = 0;
                }
            }

            if (_stateMachineFalling == true)
            {
                if (_timeFalling <= coyoteTime)
                {
                    if (Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.S)) // "Jump" keys
                    {
                        if (_jumpKeysHolded == false)
                        {
                            // Cancel falling velocity //
                            _cancelFallingVelocityImpulse = -playerRigidbody2D.velocity.y * playerRigidbody2D.mass;
                            playerRigidbody2D.AddForce(transform.up * _cancelFallingVelocityImpulse, ForceMode2D.Impulse);
                            
                            // Vertical impulse //
                            playerRigidbody2D.AddForce(transform.up * jumpImpulse, ForceMode2D.Impulse);
                            _inAir = true;
                            _timeAfterJump = 0;
                            _jumpKeysHolded = true;
                            
                            animator.SetBool("Jumping", true);
                            _stateMachineJumping = true;
                            animator.SetBool("Landed", false);
                            _stateMachineLanded = false;
                        }
                    }
                }
            }

            _timeFalling += Time.deltaTime;

            if (_stateMachineJumping == true)
            {
                if (Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.S)) // "Jump" keys
                {
                    if (_timeAfterJump < maxTimeAfterJump)
                    {
                        _jumpForce = jumpThrust * Time.deltaTime;
                        _jumpForce = _jumpForce * (1 - (_timeAfterJump / maxTimeAfterJump));
                    
                        // Vertical force //
                        playerRigidbody2D.AddForce(transform.up * _jumpForce, ForceMode2D.Force);
                    }
                }
                
                _timeAfterJump += Time.deltaTime;
            }
            
            if (Input.GetKey(KeyCode.S) && _stateMachineDropping == false) // "Squat" key
            {
                animator.SetBool("Squatting", true);
                _stateMachineSquatting = true;
            }
            else
            {
                if (_stateMachineDropping == false)
                {
                    animator.SetBool("Squatting", false);
                    _stateMachineSquatting = false;
                }
            }

            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.Space) && _stateMachineDropping == false) // "Fall through platform" keys
            {
                if (jumpRay.collider != null)
                {
                    if (_droppingFromPlatform == false)
                    {
                        // Platform collider deactivation //
                        _platformCollider2D = jumpRay.collider;
                        Physics2D.IgnoreCollision(playerCollider, _platformCollider2D, true);
                        _droppingFromPlatform = true;
                        animator.SetBool("Squatting", true);
                        _stateMachineSquatting = true;
                        animator.SetBool("Dropping", true);
                        _stateMachineDropping = false;
                    }
                }
            }
        }
}