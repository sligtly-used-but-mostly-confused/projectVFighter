using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(GravityObjectRigidBody))]

public abstract class PlayerController : MonoBehaviour {

    public List<GravityObjectRigidBody> AttachedObjects;

    [SerializeField]
    protected float RechargeTime = 1f;
    [SerializeField]
    protected float MoveSpeed = 1f;
    [SerializeField]
    protected float ShootSpeed = 1f;
    [SerializeField]
    protected float JumpForce = 10f;
    [SerializeField]
    protected float ImpulseToKill = 10f;

    [SerializeField]
    protected GameObject Projectile;
    [SerializeField]
    protected GameObject AimingReticle;

    private readonly Vector2[] _compass = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };

    protected DataService _dataService;

    protected virtual void Awake()
    {
        _dataService = new DataService("ActionLogs.db");
        //_dataService.CreateDB();
    }

    private bool coolingDown;

    public void Move(Vector2 dir)
    {
        GetComponent<GravityObjectRigidBody>().AddLinearAcceleration(dir * MoveSpeed);
    }

    public void AimReticle(Vector2 dir)
    {
        AimingReticle.transform.localPosition = dir.normalized;
    }

    public void ChangeGravity(Vector2 dir)
    {
        _dataService.InsertAction(new PlayerAction(ActionType.ChangeGrav, dir));
        GetComponent<GravityObjectRigidBody>().ChangeGravityDirection(dir);
        AttachedObjects.ForEach(x => x.ChangeGravityDirection(dir));
    }

    public void Jump()
    {
        throw new System.NotImplementedException();
    }

    public void ShootGravityGun(Vector2 dir)
    {
        if (!coolingDown)
        {
            _dataService.InsertAction(new PlayerAction(ActionType.FireGravGun, dir));
            GameObject projectileClone = (GameObject)Instantiate(Projectile, AimingReticle.transform.position, AimingReticle.transform.rotation);
            projectileClone.GetComponent<GravityGunProjectileController>().Owner = this;
            projectileClone.GetComponent<Rigidbody2D>().velocity = dir * ShootSpeed;
            StartCoroutine(CoolDown());
        }
    }

    IEnumerator CoolDown()
    {
        coolingDown = true;
        yield return new WaitForSeconds(RechargeTime);
        coolingDown = false;
    }

    public void AttachGORB(GravityObjectRigidBody gravityObjectRB)
    {
        AttachedObjects.Add(gravityObjectRB);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var impulse = (collision.relativeVelocity * collision.rigidbody.mass).magnitude;
        if (impulse > ImpulseToKill && collision.collider.GetComponent<GravityObjectRigidBody>())
        {
            Destroy(gameObject);
        }
    }

    protected Vector2 ClosestDirection(Vector2 v)
    {

        var maxDot = -Mathf.Infinity;
        var ret = Vector3.zero;

        foreach (var dir in _compass)
        {
            var t = Vector3.Dot(v, dir);
            if (t > maxDot)
            {
                ret = dir;
                maxDot = t;
            }
        }

        return ret;
    }
}
