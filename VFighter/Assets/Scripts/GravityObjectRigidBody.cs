using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VelocityType
{
    Gravity,
    Movement,
    Dash,
    OtherPhysics
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class GravityObjectRigidBody : MonoBehaviour {
    private static int _idCnt = 0;

    [SerializeField]
    private int _id;
    [SerializeField]
    private float _gravityScale = 1f;
    [SerializeField]
    private Vector2 _gravityDirection = Vector2.down;
    [SerializeField]
    private Vector2 _maxComponentSpeed = new Vector2(10f, 10f);
    [SerializeField]
    private bool _stopObjectOnCollide = true;
    [SerializeField]
    private float _dashDecay = 1f;
    [SerializeField]
    private float _drag = 1f;

    public float Bounciness = 0f;

    public bool CanBeSelected = true;
    public bool KillsPlayer = true;

    public PlayerController Owner;

    private Dictionary<VelocityType, Vector2> _velocities = new Dictionary<VelocityType, Vector2>();

    private Rigidbody2D _rB;

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

    public Vector2 MaxComponentSpeed
    {
        get{return _maxComponentSpeed;}
        set{_maxComponentSpeed = value;}
    }

    private void Awake()
    {
        _id = _idCnt++;
        _rB = GetComponent<Rigidbody2D>();
    }

    void Start () {
        _rB.gravityScale = 0;
	}
	
	void FixedUpdate () {
        DoGravity();
        DashDecay();
        DoDrag();
        ProcessVelocity();
	}

    private void ProcessVelocity()
    {
        _rB.velocity = Vector2.zero;

        foreach (var velocity in _velocities)
        {
            _rB.velocity += velocity.Value;
        }
    }

    private void DoDrag()
    {
        Vector2 vel = GetVelocity(VelocityType.OtherPhysics);
        vel *= 1 - _drag;
        UpdateVelocity(VelocityType.OtherPhysics, vel);
    }

    private void DashDecay()
    {
        Vector2 dashVel = GetVelocity(VelocityType.Dash);
        var deltaPerSecond = dashVel * _dashDecay;
        var deltaPerTick = deltaPerSecond * Time.fixedDeltaTime;
        UpdateVelocity(VelocityType.Dash, dashVel - deltaPerTick);
    }

    private void DoGravity()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0;
        AddLinearAcceleration(VelocityType.Gravity, GravityDirection * GravityScale * 9.81f);
    }

    public Vector2 GetVelocity(VelocityType id)
    {
        if (!_velocities.ContainsKey(id))
        {
            _velocities.Add(id, Vector2.zero);
        }

        return _velocities[id];
    }

    public void UpdateVelocity(VelocityType id, Vector2 vel)
    {
        if(!_velocities.ContainsKey(id))
        {
            _velocities.Add(id, vel);
        }

        _velocities[id] = vel;
    }

    public void AddVelocity(VelocityType id, Vector2 velocityVector)
    {
        var velocityDelta = velocityVector;
        var tempVel = GetVelocity(id) + velocityDelta;
        var xVel = Mathf.Clamp(tempVel.x, -MaxComponentSpeed.x, MaxComponentSpeed.x);
        var yVel = Mathf.Clamp(tempVel.y, -MaxComponentSpeed.y, MaxComponentSpeed.y);
        _velocities[id] = new Vector2(xVel, yVel);
    }

    public void AddLinearAcceleration(VelocityType id, Vector2 AccelerationVector)
    {
        AddVelocity(id, AccelerationVector * Time.fixedDeltaTime);
    }

    public void ChangeGravityDirection(Vector2 dir)
    {
        if (dir != GravityDirection)
        {
            UpdateVelocity(VelocityType.Gravity, Vector2.zero);
            GravityDirection = dir;
        }
    }

    public void ChangeGravityScale(float newGravityScale)
    {
        GravityScale = newGravityScale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<GravityObjectRigidBody>())
        {
            var vel = _rB.velocity;
            var normal = Vector2.zero;
            foreach (var contact in collision.contacts)
            {
                normal += contact.normal;
                break;
            }

            vel = Vector2.Reflect(vel, normal.normalized);
            Debug.Log(name +" " + vel * collision.gameObject.GetComponent<GravityObjectRigidBody>().Bounciness);
            var thisMass = _rB.mass;
            var otherMass = collision.rigidbody.mass;
            Debug.Log(thisMass + " " + otherMass);
            Debug.Log(1 - (thisMass / (thisMass + otherMass)));
            var reflectionCoef = collision.gameObject.GetComponent<GravityObjectRigidBody>().Bounciness * (1 - (thisMass / (thisMass + otherMass)));
            var reflectionVec = vel * reflectionCoef;
            UpdateVelocity(VelocityType.OtherPhysics, reflectionVec);
        }

        if (_stopObjectOnCollide)
        {
            ChangeGravityDirection(Vector2.zero);
        }
    }
}
