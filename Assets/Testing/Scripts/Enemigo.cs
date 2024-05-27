using UnityEngine;

public class Enemigo : MonoBehaviour
{

    // SINGLETON //
    private Singleton _singleton;
    [SerializeField] private GameObject singletonInstance;

    // Enemy Components //
    public Rigidbody2D enemyRigidbody2D;

    // "Sonar" method variables //
    RaycastHit2D _sonarRaycast;
    private float _sonarAngle;
    private float _sonarCos;
    private float _sonarSin;
    public float sonarVelocity;
    public float sonarDistance;
    public LayerMask sonarLayers;
    public bool _walkLeft;
    public bool _walkRight;

    // "Walk" method variables //
    public float walkThrust;
    private float _walkForce;
    public float maxVelocity;
    private int _isWalking;
    private bool _isWalkingLeft;
    private bool _isWalkingRight;





    void Awake()
    {
        _singleton = singletonInstance.GetComponent<Singleton>();
    }





    void Update()
    {
        Sonar();
        Walk();
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
            _sonarAngle -= 360;
        }

        if(_sonarRaycast.collider != null)
        {
            Debug.Log(_sonarAngle);
            if(_sonarAngle > 135 && _sonarAngle < 225)
            {
                _walkLeft = true;
                _walkRight = false;
            }
            if (_sonarAngle < 45 || _sonarAngle > 315)
            {
                _walkLeft = false;
                _walkRight = true;
            }
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
            //enemyAnimator.SetBool("Walking", true);
        }
        else
        {
            //enemyAnimator.SetBool("Walking", false);
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
}
