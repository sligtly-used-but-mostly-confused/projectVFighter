using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class GravityObjectRigidBody : MonoBehaviour {
    private static int _idCnt = 0;

    [SerializeField]
    private int _id;
    [SerializeField]
    private float _gravityScale = 1f;
    [SerializeField]
    private Vector2 _gravityDirection = Vector2.down;
    [SerializeField]
    private float _maxComponentSpeed = 10;
    [SerializeField]
    private bool _stopObjectOnCollide = true;
    public PlayerController Owner;

    public float GravityScale
    {
        get { return _gravityScale; }
        private set { _gravityScale = value; }
    }

    public Vector2 GravityDirection
    {
        get { return _gravityDirection; }
        private set { _gravityDirection = value; }
    }

    public int Id
    {
        get { return _id; }
        private set { _id = value; }
    }

    public float MaxComponentSpeed
    {
        get{return _maxComponentSpeed;}
        set{_maxComponentSpeed = value;}
    }

    private void Awake()
    {
        _id = _idCnt++;
    }

    void Start () {
        GetComponent<Rigidbody2D>().gravityScale = 0;
	}
	
	void FixedUpdate () {
        DoGravity();
	}

    private void DoGravity()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0;
        AddLinearAcceleration(GravityDirection * GravityScale * 9.81f);
    }

    public void AddVelocity(Vector2 velocityVector)
    {
        var prevVel = GetComponent<Rigidbody2D>().velocity;
        var velocityDelta = velocityVector;
        var tempVel = prevVel + velocityDelta;
        var xVel = Mathf.Clamp(tempVel.x, -MaxComponentSpeed, MaxComponentSpeed);
        var yVel = Mathf.Clamp(tempVel.y, -MaxComponentSpeed, MaxComponentSpeed);
        GetComponent<Rigidbody2D>().velocity = new Vector2(xVel, yVel);
    }

    public void AddLinearAcceleration(Vector2 AccelerationVector)
    {
        AddVelocity(AccelerationVector * Time.fixedDeltaTime);
    }

    public void ChangeGravityDirection(Vector2 dir)
    {
        //Debug.Log(name + " " + dir);
        if (dir != GravityDirection)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GravityDirection = dir;
        }
    }

    public void ChangeGravityScale(float newGravityScale)
    {
        GravityScale = newGravityScale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(_stopObjectOnCollide)
        {
            Debug.Log("hit somthing, so reset to a cardinal direction");
            ChangeGravityDirection(Vector2.zero);
        }
    }
}
