using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GravityObjectRigidBody : MonoBehaviour {
    private static int _idCnt = 0;

    [SerializeField]
    private int _id;
    [SerializeField]
    private float _gravityScale = 1f;
    [SerializeField]
    private Vector2 _gravityDirection = Vector2.down;
    [SerializeField]
    private float MaxComponentSpeed = 10;

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

    public void AddLinearAcceleration(Vector2 AccelerationVector)
    {
        var prevVel = GetComponent<Rigidbody2D>().velocity;
        var velocityDelta = AccelerationVector * Time.fixedDeltaTime;
        var tempVel = prevVel + velocityDelta;
        var newVel = prevVel;

        if (MaxComponentSpeed > Mathf.Abs(tempVel.x))
        {
            //Debug.Log("x " + tempVel.x);
            newVel += new Vector2(velocityDelta.x, 0);
        }

        if (MaxComponentSpeed > Mathf.Abs(tempVel.y))
        {
            //Debug.Log("y " + tempVel.y);
            newVel += new Vector2(0, velocityDelta.y);
        }

        GetComponent<Rigidbody2D>().velocity = newVel;
    }

    public void ChangeGravityDirection(Vector2 dir)
    {
        //Debug.Log(name + " " + dir);
        if (dir != GravityDirection)
        {
            //Debug.Log("inside " + name + " " + dir);
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GravityDirection = dir;
        }
    }

    public void ChangeGravityScale(float newGravityScale)
    {
        GravityScale = newGravityScale;
    }
}
