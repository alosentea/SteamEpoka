using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement2 : MonoBehaviour
{
    
    // SINGLETON //
    private Singleton _singleton;
    [SerializeField] private GameObject singletonInstance;
    
    // Player Components //
    public Rigidbody2D playerRigidbody2D;
    public Animator playerAnimator;
    public Collider2D playerCollider;
    
    // "Walk" method variables //
    public float walkThrust;
    private float _walkForce;
    public float maxVelocity;
    private int _isWalking;
    private bool _isWalkingLeft;
    private bool _isWalkingRight;
    
    // "Dash" method variables //
    public float dashImpulse;
    private float _timeSinceDash;
    public float dashCooldown;
    private int _isDashing;
    private bool _isDashingLeft;
    private bool _isDashingRight;
    
    // "Landed Detector" method variables //
    RaycastHit2D _landedRaycast;
    public float landedRaycastDistance;
    public LayerMask landedRaycastLayers;
    private bool _isLanded;
    private float _timeSinceLanded;
    
    // "Jump" method variables //
    public float jumpImpulse;
    private bool _jumpKeysHolded;
    private bool _justJumped;
    private bool _isJumping;
    public float coyoteTime;
    private float _cancelFallingVelocityImpulse;
    
    // "Jump_HeightControl" method variables //
    public float jumpThrust;
    private float _jumpForce;
    public float jumpThrustTime;
    private float _timeAfterJump;
    
    // "Squatting" method variables //
    private bool _isSquatting;
    
    // "Dropping" method variables //
    RaycastHit2D _dropRaycast;
    public LayerMask dropRaycastLayers;
    Collider2D _platformCollider2D;
    private bool _justDropped;
    private bool _isDropping;
    
    // "Attack" method variables //
    public int attackADamage;
    public int attackBDamage;
    public int jumpAttackDamage;
    private int attackAux;
    private bool _attackKeysHolded;
    private bool _isAttacking;
    private bool _stoppedInAir;
    public float jumpAttackTime;
    private bool _launchedDownwards;
    public float jumpAttackDownwardsImpulse;
    
    
    
    
    
    void Awake()
    {
        _singleton = singletonInstance.GetComponent<Singleton>();
    }
    
    
    
    
    
    void Update()
    {
        AddDeltaTime();
        LandedDetector();
        
        Squatting();
        
        if (!_isSquatting)
        {
            Jump();
        }

        if (!_isJumping && !_isSquatting)
        {
            Dash();
        }
        
        if (_isDashing == 0 && !_isSquatting && !_isAttacking)
        {
            Walk();
        }
        else
        {
            _isWalkingLeft = false;
            _isWalkingRight = false;
            _isWalking = 0;
        }

        if (_isWalking == 0 && _isDashing == 0 && !_isJumping && _isLanded)
        {
            CancelHorizontalVelocity();
        }
        
        Dropping();
        
        Attack();
    }
    
    
    
    
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            if (!_singleton.playerTrigger[int.Parse(collision.gameObject.name)])
            {
                _singleton.playerTrigger[int.Parse(collision.gameObject.name)] = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            if (_singleton.playerTrigger[int.Parse(collision.gameObject.name)])
            {
                _singleton.playerTrigger[int.Parse(collision.gameObject.name)] = false;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            if ((collision.gameObject.transform.position.x >= transform.position.x && transform.right.x > 0) || (collision.gameObject.transform.position.x <= transform.position.x && transform.right.x < 0))
            {
                if (_singleton.playerTrigger[int.Parse(collision.gameObject.name)])
                {
                    if (!_singleton.facingEnemy[int.Parse(collision.gameObject.name)])
                    {
                        _singleton.facingEnemy[int.Parse(collision.gameObject.name)] = true;
                    }
                }
                else
                {
                    if (_singleton.facingEnemy[int.Parse(collision.gameObject.name)])
                    {
                        _singleton.facingEnemy[int.Parse(collision.gameObject.name)] = false;
                    }
                }
            }
            else
            {
                if (_singleton.facingEnemy[int.Parse(collision.gameObject.name)])
                {
                    _singleton.facingEnemy[int.Parse(collision.gameObject.name)] = false;
                }
            }
        }
    }
    
    
    
    
    
    private void Walk()
    {
        // [_isWalkingLeft] always correct outside function
        // [_isWalkingRight] always correct outside function
        // [_isWalking] always correct outside function
        
        
        
        if (Input.GetKey(KeyCode.A))
        {
            Walk_ApplyLeftWalkingForce();
            _isWalkingLeft = true;
        }
        else
        {
            _isWalkingLeft = false;
        }
        
        
        
        if (Input.GetKey(KeyCode.D))
        {
            Walk_ApplyRightWalkingForce();
            _isWalkingRight = true;
        }
        else
        {
            _isWalkingRight = false;
        }
        
        
        
        if (_isWalkingLeft || _isWalkingRight)
        {
            _isWalking = 1;
        }
        else
        {
            _isWalking = 0;
        }
        if (_isWalkingLeft && _isWalkingRight)
        {
            _isWalking = 0;
            _isWalkingLeft = false;
            _isWalkingRight = false;
        }
        
        
        
        if (_isWalking == 1)
        {
            playerAnimator.SetBool("Walking", true);
        }
        else
        {
            playerAnimator.SetBool("Walking", false);
        }
    }
    
    private void Walk_ApplyLeftWalkingForce()
    {
        _walkForce = walkThrust * Time.deltaTime;
        transform.localEulerAngles = new UnityEngine.Vector3(0, 180, 0);
        _walkForce = _walkForce * (1 - (-playerRigidbody2D.velocity.x / maxVelocity));
        playerRigidbody2D.AddForce(transform.right * _walkForce, ForceMode2D.Force);
    }
    private void Walk_ApplyRightWalkingForce()
    {
        _walkForce = walkThrust * Time.deltaTime;
        transform.localEulerAngles = new UnityEngine.Vector3(0, 0, 0);
        _walkForce = _walkForce * (1 - (playerRigidbody2D.velocity.x / maxVelocity));
        playerRigidbody2D.AddForce(transform.right * _walkForce, ForceMode2D.Force);
    }

    
    
    private void Dash()
    {
        // [_isDashingLeft] always correct outside function
        // [_isDashingRight] always correct outside function
        // [_isDashing] always correct outside function



        if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_Dash"))
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                if (_timeSinceDash >= dashCooldown)
                {
                    CancelHorizontalVelocity();
                    Dash_ApplyLeftDashingImpulse();
                    _timeSinceDash = 0;
                
                    _isDashingLeft = true;
                }
                else
                {
                    _isDashingLeft = false;
                }
            }
            else
            {
                _isDashingLeft = false;
            }
        }



        if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_Dash"))
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                if (_timeSinceDash >= dashCooldown)
                {
                    CancelHorizontalVelocity();
                    Dash_ApplyRightDashingImpulse();
                    _timeSinceDash = 0;
                
                    _isDashingRight = true;
                }
                else
                {
                    _isDashingRight = false;
                }
            }
            else
            {
                _isDashingRight = false;
            }
        }


        if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_Dash"))
        {
            if (_isDashingLeft || _isDashingRight)
            {
                _isDashing = 1;
            }
            else
            {
                _isDashing = 0;
            }
        }
        
        
        
        if (_isDashing == 1)
        {
            playerAnimator.SetBool("Dashing", true);
        }
        else
        {
            playerAnimator.SetBool("Dashing", false);
        }
    }
    
    private void Dash_ApplyRightDashingImpulse()
    {
        transform.localEulerAngles = new UnityEngine.Vector3(0, 0, 0);
        playerRigidbody2D.AddForce(transform.right * dashImpulse, ForceMode2D.Impulse);
    }
    private void Dash_ApplyLeftDashingImpulse()
    {
        transform.localEulerAngles = new UnityEngine.Vector3(0, 180, 0);
        playerRigidbody2D.AddForce(transform.right * dashImpulse, ForceMode2D.Impulse);
    }
    
    
    
    private void CancelHorizontalVelocity()
    {
        playerRigidbody2D.velocity = new Vector2(0.0f, playerRigidbody2D.velocity.y);
    }
    private void LandedDetector()
    {
        _landedRaycast = Physics2D.Raycast(transform.position, -transform.up, landedRaycastDistance, landedRaycastLayers);

        if (_landedRaycast.collider != null)
        {
            playerAnimator.SetBool("Landed", true);
            _isLanded = true;
            _timeSinceLanded = 0;
        }
        else
        {
            playerAnimator.SetBool("Landed", false);
            _isLanded = false;
        }
    }
    private void AddDeltaTime()
    {
        _timeSinceLanded += Time.deltaTime;
        _timeSinceDash += Time.deltaTime;
        _timeAfterJump += Time.deltaTime;
    }

    
    
    private void Jump()
    {
        if ((_isLanded || _timeSinceLanded < coyoteTime) && !_isJumping)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                if (!_jumpKeysHolded)
                {
                    _cancelFallingVelocityImpulse = -playerRigidbody2D.velocity.y * playerRigidbody2D.mass;
                    playerRigidbody2D.AddForce(transform.up * _cancelFallingVelocityImpulse, ForceMode2D.Impulse);
                    
                    playerRigidbody2D.AddForce(transform.up * jumpImpulse, ForceMode2D.Impulse);
                    _justJumped = true;
                    _timeAfterJump = 0;
                }

                _jumpKeysHolded = true;
            }
            else
            {
                _jumpKeysHolded = false;
            }
        }

        if (!_isAttacking)
        {
            Jump_HeightControl();
        }
        
        if (!_isLanded && _justJumped)
        {
            _justJumped = false;
            _isJumping = true;
        }

        if (_isLanded && _isJumping)
        {
            _isJumping = false;
        }
    }

    private void Jump_HeightControl()
    {
        if (playerRigidbody2D.velocity.y > 0 && _timeAfterJump < jumpThrustTime)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                _jumpForce = jumpThrust * Time.deltaTime;
                _jumpForce = _jumpForce * (1 - (_timeAfterJump / jumpThrustTime));
                playerRigidbody2D.AddForce(transform.up * _jumpForce, ForceMode2D.Force);
            }
        }
    }

    
    
    private void Squatting()
    {
        if (_isLanded)
        {
            if (Input.GetKey(KeyCode.S))
            {
                _isSquatting = true;
                
                playerAnimator.SetBool("Squatting", true);
            }
            else
            {
                if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_Agacharse"))
                {
                    _isSquatting = false;
                
                    playerAnimator.SetBool("Squatting", false);
                }
            }
        }
    }

    
    
    private void Dropping()
    {
        _dropRaycast = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.32f), -transform.up, 0.65f, dropRaycastLayers);
        
        if (_isLanded && !_justDropped && !_isDropping)
        {
            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.Space))
            {
                _platformCollider2D = _dropRaycast.collider;
                Physics2D.IgnoreCollision(playerCollider, _platformCollider2D, true);
                _justDropped = true;
            }
        }
        
        if (!_isLanded && _justDropped)
        {
            _justDropped = false;
            _isDropping = true;
        }

        if (_isLanded && _isDropping)
        {
            Physics2D.IgnoreCollision(playerCollider, _platformCollider2D, false);
            _isDropping = false;
        }
    }



    private void Attack()
    {
        if (Input.GetKey(KeyCode.K))
        {
            if (_attackKeysHolded == false)
            {
                _attackKeysHolded = true;
            
                if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_AtacarA") || playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_AtacarAgachadoA"))
                {
                    playerAnimator.SetBool("AttackB", true);
                    
                    attackAux = 0;
                    while (attackAux < _singleton.entityNumber)
                    {
                        if (_singleton.facingEnemy[attackAux])
                        {
                            _singleton.enemyHealth[attackAux] -= attackBDamage;
                        }
                        attackAux++;
                    }
                }
                else
                {
                    if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_Idle") || playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_Correr") || playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_Agachado"))
                    {
                        playerAnimator.SetBool("AttackA", true);
                            
                        attackAux = 0;
                        while (attackAux < _singleton.entityNumber)
                        {
                            if (_singleton.facingEnemy[attackAux])
                            {
                                _singleton.enemyHealth[attackAux] -= attackADamage;
                            }
                            attackAux++;
                        }
                    }
                    if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_TakeOff") || playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_InAir"))
                    {
                        playerAnimator.SetBool("AttackA", true);
                    }
                }
            }
        }
        else
        {
            _attackKeysHolded = false;
        }
        
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_AtacarA") || playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_AtacarAgachadoA") || playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_AtacarSaltandoA"))
        {
            playerAnimator.SetBool("AttackA", false);
            _isAttacking = true;
        }
        else
        {
            _isAttacking = false;
        }
        
        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_AtacarB") || playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_AtacarAgachadoB"))
        {
            playerAnimator.SetBool("AttackB", false);
            _isAttacking = true;
        }

        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_AtacarSaltandoA"))
        {
            if (_stoppedInAir == false)
            {
                playerRigidbody2D.velocity = Vector2.zero;
                playerRigidbody2D.gravityScale = 0;
                _stoppedInAir = true;
            }
            
            attackAux = 0;
            while (attackAux < _singleton.entityNumber)
            {
                if (_singleton.playerTrigger[attackAux])
                {
                    _singleton.enemyHealth[attackAux] -= jumpAttackDamage;
                }
                attackAux++;
            }

            if (playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= jumpAttackTime && !_launchedDownwards)
            {
                playerRigidbody2D.gravityScale = 3;
                playerRigidbody2D.AddForce(transform.up * -jumpAttackDownwardsImpulse, ForceMode2D.Impulse);
                _launchedDownwards = true;
            }
        }

        if (_isLanded && !playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAnimations_AtacarSaltandoA") && _launchedDownwards == true)
        {
            _stoppedInAir = false;
            _launchedDownwards = false;
        }
    }
}
