using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;


[Serializable]
public struct VelocityTypePair
{
    [SerializeField]
    public VelocityType type;
    [SerializeField]
    public Vector2 Velocity;
}

public enum VelocityType
{
    Gravity,
    Movement,
    Dash,
    OtherPhysics
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class GravityObjectRigidBody : NetworkBehaviour {
    #region vars
    private static int _idCnt = 0;

    [SerializeField]
    private int _id;
    [SerializeField]
    private float _gravityScale = 1f;
    [SerializeField]
    private Vector2 _gravityDirection = Vector2.down;
    [SerializeField]
    private Vector2 _maxDefaultComponentSpeed = new Vector2(10f, 10f);
    [SerializeField]
    private List<VelocityTypePair> _maxComponentSpeeds;
    [SerializeField]
    private bool _stopObjectOnCollide = true;
    [SerializeField]
    private float _timeToDashStop = 1f;
    [SerializeField]
    private float _drag = 1f;

    public float Bounciness = 0f;

    public bool CanBeSelected = true;
    public bool KillsPlayer = true;

    public PlayerController Owner;

    protected Dictionary<VelocityType, Vector2> _maxComponentVelocities = new Dictionary<VelocityType, Vector2>();
    protected Dictionary<VelocityType, Vector2> _velocities = new Dictionary<VelocityType, Vector2>();

    private Rigidbody2D _rB;
    public bool IsSimulatedOnThisConnection = false;

    
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
    #endregion

    private void Awake()
    {
        _id = _idCnt++;
        _rB = GetComponent<Rigidbody2D>();
        _maxComponentSpeeds.ForEach(x => _maxComponentVelocities.Add(x.type, x.Velocity));
    }

    public override void OnStartServer()
    {
        Debug.Log("start " + isServer + " " + GetComponent<PlayerController>());
        if (!GetComponent<PlayerController>())
        {
            Debug.Log("end " + isServer + " " + isLocalPlayer);
            IsSimulatedOnThisConnection = isServer;
        }
    }

    void Start () {
        _rB.gravityScale = 0;
	}
	
	void FixedUpdate () {
        DoGravity();
        DoDrag();
        ProcessVelocity();
	}

    private void ProcessVelocity()
    {
        if (GetComponent<GravityObjectRigidBody>().IsSimulatedOnThisConnection)
        {
            _rB.velocity = Vector2.zero;

            foreach (var velocity in _velocities)
            {
                _rB.velocity += velocity.Value * GameManager.Instance.TimeScale;
            }
        }
    }

    private void DoDrag()
    {
        Vector2 vel = GetVelocity(VelocityType.OtherPhysics);
        vel *= 1 - _drag;
        UpdateVelocity(VelocityType.OtherPhysics, vel);
    }

    private void DoGravity()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0;
        AddVelocity(VelocityType.Gravity, GravityDirection * GravityScale * 9.81f);
    }

    public Vector2 GetVelocity(VelocityType id)
    {
        if (!_velocities.ContainsKey(id))
        {
            _velocities.Add(id, Vector2.zero);
        }

        return _velocities[id];
    }

    public virtual void UpdateVelocity(VelocityType id, Vector2 vel)
    {
        if(!_velocities.ContainsKey(id))
        {
            _velocities.Add(id, vel);
        }

        _velocities[id] = vel;
    }

    public virtual Vector2 GetMaxComponentVelocity(VelocityType type)
    {
        if(!_maxComponentVelocities.ContainsKey(type))
        {
            _maxComponentVelocities.Add(type, _maxDefaultComponentSpeed);
        }

        return _maxComponentVelocities[type];
    }

    public virtual void AddVelocity(VelocityType id, Vector2 velocityVector)
    {
        var velocityDelta = velocityVector;
        var tempVel = GetVelocity(id) + velocityDelta;
        var maxComponentSpeed = GetMaxComponentVelocity(id);
        if (tempVel.magnitude > maxComponentSpeed.magnitude)
        {
            tempVel = tempVel.normalized * maxComponentSpeed.magnitude;
        }

        _velocities[id] = tempVel;
    }

    public void AddLinearAcceleration(VelocityType id, Vector2 AccelerationVector)
    {
        AddVelocity(id, AccelerationVector * Time.fixedDeltaTime);
    }

    public void ChangeGravityDirection(Vector2 dir)
    {
        /*
        if(IsSimulatedOnThisConnection)
        {
            ChangeGravityDirectionInternal(dir);
        }
        else
        {
            //find a player object
            //FindObjectOfType<PlayerController>().PassThough(CmdBroadcastChangeGravityDirection, dir);
            //CmdBroadcastChangeGravityDirection(dir);
        }
        */
    }

    /*
    [Command]
    public void CmdBroadcastChangeGravityDirection(Vector2 dir)
    {
        Debug.Log("cmd change grav");
        if (IsSimulatedOnThisConnection)
        {
            ChangeGravityDirectionInternal(dir);
        }
        else
        {
            FindObjectOfType<PlayerController>().PassThough(RpcBroadcastChangeGravityDirection, dir);
        }
    }

    [ClientRpc]
    public void RpcBroadcastChangeGravityDirection(Vector2 dir)
    {
        Debug.Log("rpc change grav");
        if (IsSimulatedOnThisConnection)
        {
            ChangeGravityDirection(dir);
        }
    }
    */

    public void ChangeGravityDirectionInternal(Vector2 dir)
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

    public void Dash(Vector2 dashVec)
    {
        StartCoroutine(StartDash(dashVec));
    }

    private IEnumerator StartDash(Vector2 dashVec)
    {
        ClearAllVelocities();
        UpdateVelocity(VelocityType.Dash, dashVec);
        yield return new WaitForSeconds(_timeToDashStop);
        ClearAllVelocities();
    }

    private void ClearAllVelocities()
    {
        var velKeys = _velocities.Keys.ToList();
        for(int i = 0; i < velKeys.Count; i++)
        {
            UpdateVelocity(velKeys[i], Vector2.zero);
        }
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
            //Debug.Log(name +" " + vel * collision.gameObject.GetComponent<GravityObjectRigidBody>().Bounciness);
            var thisMass = _rB.mass;
            var otherMass = collision.rigidbody.mass;
            //Debug.Log(thisMass + " " + otherMass);
            //Debug.Log(1 - (thisMass / (thisMass + otherMass)));
            var reflectionCoef = collision.gameObject.GetComponent<GravityObjectRigidBody>().Bounciness * (1 - (thisMass / (thisMass + otherMass)));
            var reflectionVec = vel * reflectionCoef;
            UpdateVelocity(VelocityType.OtherPhysics, reflectionVec);
        }

        if (_stopObjectOnCollide && IsSimulatedOnThisConnection)
        {
            //ChangeGravityDirection(Vector2.zero);
            FindObjectOfType<PlayerController>().ChangeGORBGravityDirection(this, Vector2.zero);
        }
    }
}
