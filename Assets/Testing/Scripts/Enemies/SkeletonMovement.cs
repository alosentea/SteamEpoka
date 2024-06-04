using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SkeletonMovement : MonoBehaviour
{

    // SINGLETON //
    private Singleton _singleton;
    [SerializeField] private GameObject singletonInstance;

    // Enemy Components //
    public Rigidbody2D enemyRigidbody2D;
    public Animator enemyAnimator;
    public Collider2D enemyCollider;
    
    // Player Components //
    public Collider2D playerCollider;
    
    // "OnTrigger" methods variables //
    private bool _playerInsideTrigger;
    private bool _facingPlayerInsideTrigger;

    // "Sonar" method variables //
    RaycastHit2D _sonarRaycast;
    private float _sonarAngle;
    private float _sonarCos;
    private float _sonarSin;
    public float sonarVelocity;
    public float sonarDistance;
    public float visualField;
    private float _timeWithoutDetecting;
    public float maxTimeWithoutDetecting;
    public LayerMask sonarLayers;
    private bool _walkLeft;
    private bool _walkRight;

    // "Walk" method variables //
    public float walkThrust;
    private float _walkForce;
    public float maxVelocity;
    private int _isWalking;
    private bool _isWalkingLeft;
    private bool _isWalkingRight;
    
    // "Death" method variables //
    private bool _isDying;
    
    // "Attack" method variables //
    private bool _isAttacking;
    public float attackCooldown;
    private float _timeSinceAttack;
    public int attackDamage;
    public float hitTime;
    private bool _justAttacked;
    
    // "Damage" method variables //
    private int _lastHealth;
    private bool _isDamage;
    
    // "Life" method variables //
    private bool _isLifed;
    public int enemyHealth;




    void Awake()
    {
        _singleton = GameObject.FindWithTag("Singleton").GetComponent<Singleton>();
        
        Physics2D.IgnoreCollision(enemyCollider, playerCollider, true);
        
        _timeSinceAttack = attackCooldown;
    }





    void Update()
    {
        Life();
        
        Sonar();

        if (!_isDying)
        {
            Walk();
        }
        
        Attack();
        
        Death();
        
        Damage();
        
        AddDeltaTime();
    }


    
    
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!_singleton.enemyTrigger[int.Parse(gameObject.name)])
            {
                _singleton.enemyTrigger[int.Parse(gameObject.name)] = true;
                _playerInsideTrigger = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (_singleton.enemyTrigger[int.Parse(gameObject.name)])
            {
                _singleton.enemyTrigger[int.Parse(gameObject.name)] = false;
                _playerInsideTrigger = false;
                
                _singleton.facingPlayer[int.Parse(gameObject.name)] = false;
                _facingPlayerInsideTrigger = false;
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (_singleton.enemyTrigger[int.Parse(gameObject.name)])
            {
                if ((collision.gameObject.transform.position.x >= transform.position.x && transform.right.x > 0) || (collision.gameObject.transform.position.x <= transform.position.x && transform.right.x < 0))
                {
                    if (!_singleton.facingPlayer[int.Parse(gameObject.name)])
                    {
                        _singleton.facingPlayer[int.Parse(gameObject.name)] = true;
                        _facingPlayerInsideTrigger = true;
                    }
                }
                else
                {
                    if (_singleton.facingPlayer[int.Parse(gameObject.name)])
                    {
                        _singleton.facingPlayer[int.Parse(gameObject.name)] = false;
                        _facingPlayerInsideTrigger = false;
                    }
                }
            }
        }
    }


    
    
    
    private void Sonar()
    {
        _sonarCos = Mathf.Cos(Mathf.Deg2Rad * _sonarAngle);
        _sonarSin = Mathf.Sin(Mathf.Deg2Rad * _sonarAngle);

        _sonarRaycast = Physics2D.Raycast(transform.position, new Vector2(_sonarCos, _sonarSin), sonarDistance, sonarLayers);
        Debug.DrawRay(transform.position, new Vector2(_sonarCos*sonarDistance, _sonarSin*sonarDistance), Color.red);

        _sonarAngle += sonarVelocity * Time.deltaTime;
        
        if(_sonarAngle >= 360)
        {
            _sonarAngle = 0;
        }

        if(_sonarRaycast.collider != null)
        {
            if (_sonarAngle > (180-(visualField/2)) && _sonarAngle < (180+(visualField/2)))
            {
                _walkLeft = true;
                _walkRight = false;
                
                _timeWithoutDetecting = 0;
            }
            if (_sonarAngle < (0+(visualField/2)) || _sonarAngle > (360-(visualField/2)))
            {
                _walkLeft = false;
                _walkRight = true;
                
                _timeWithoutDetecting = 0;
            }
        }
       
        if (_timeWithoutDetecting > maxTimeWithoutDetecting || _facingPlayerInsideTrigger)
        {
            _walkLeft = false;
            _walkRight = false;
        }
    }

    
    
    private void Walk()
    {
        // [_isWalkingLeft] always correct outside function
        // [_isWalkingRight] always correct outside function
        // [_isWalking] always correct outside function



        if (_walkLeft)
        {
            Walk_ApplyLeftWalkingForce();
            _isWalkingLeft = true;
        }
        else
        {
            _isWalkingLeft = false;
        }



        if (_walkRight)
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
            enemyAnimator.SetBool("Walking", true);
        }
        else
        {
            enemyAnimator.SetBool("Walking", false);
        }
    }
    
    private void Walk_ApplyLeftWalkingForce()
    {
        _walkForce = walkThrust * Time.deltaTime;
        transform.localEulerAngles = new UnityEngine.Vector3(0, 180, 0);
        _walkForce = _walkForce * (1 - (-enemyRigidbody2D.velocity.x / maxVelocity));
        enemyRigidbody2D.AddForce(transform.right * _walkForce, ForceMode2D.Force);
    }
    private void Walk_ApplyRightWalkingForce()
    {
        _walkForce = walkThrust * Time.deltaTime;
        transform.localEulerAngles = new UnityEngine.Vector3(0, 0, 0);
        _walkForce = _walkForce * (1 - (enemyRigidbody2D.velocity.x / maxVelocity));
        enemyRigidbody2D.AddForce(transform.right * _walkForce, ForceMode2D.Force);
    }
    
    
    
    private void AddDeltaTime()
    {
        _timeWithoutDetecting += Time.deltaTime;
        _timeSinceAttack += Time.deltaTime;
    }



    private void Death()
    {
        if (_singleton.enemyHealth[int.Parse(gameObject.name)] <= 0)
        {
            enemyAnimator.SetBool("Death", true);
            _isDying = true;

            if (enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("SkeletonAnimations_Muerte"))
            {
                if (enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    _singleton.facingEnemy[int.Parse(gameObject.name)] = false;
                    Destroy(gameObject);
                }
            }
        }
    }



    private void Attack()
    {
        if (_facingPlayerInsideTrigger)
        {
            if (_timeSinceAttack > attackCooldown)
            {
                if (!_isAttacking)
                {
                    enemyAnimator.SetBool("Attack", true);
                    _isAttacking = true;
                }
            }
        }
        
        if (enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("SkeletonAnimations_Atacar"))
        {
            if (enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > hitTime)
            {
                if (_facingPlayerInsideTrigger)
                {
                    if (!_justAttacked)
                    {
                        _singleton.playerDamage += attackDamage;
                        _justAttacked = true;
                    }
                }
            }
            
            if (enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                enemyAnimator.SetBool("Attack", false);
                _isAttacking = false;
                _justAttacked = false;
                    
                _timeSinceAttack = 0;
            }
        }
    }



    private void Damage()
    {
        if (_lastHealth > _singleton.enemyHealth[int.Parse(gameObject.name)] && _lastHealth > 0)
        {
            enemyAnimator.SetBool("Damage", true);
            _isDamage = true;
        }

        if (enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("SkeletonAnimations_Daño"))
        {
            if (enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                enemyAnimator.SetBool("Damage", false);
                _isDamage = false;
            }
        }

        _lastHealth = _singleton.enemyHealth[int.Parse(gameObject.name)];
    }



    private void Life()
    {
        if (!_isLifed)
        {
            _singleton.enemyHealth[int.Parse(gameObject.name)] = enemyHealth;
            _isLifed = true;
        }
    }
}
