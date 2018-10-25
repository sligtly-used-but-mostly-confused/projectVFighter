using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ProjectilePool : NetworkBehaviour {

    public static ProjectilePool Instance;

    public GameObject NormalProjectilePrefab;
    public GameObject RocketProjectilePrefab;
    public GameObject ShotGunProjectilePrefab;
    private Dictionary<Type, GameObject> ProjectileTypeToPrefabMapping = new Dictionary<Type, GameObject>();
    private Dictionary<Type, List<GravityGunProjectileController>> _projectilePools = new Dictionary<Type, List<GravityGunProjectileController>>();

    [SerializeField]
    private int _startingPoolSize = 100;

    private void Awake()
    {
        if(Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public override void OnStartServer()
    {
        ProjectileTypeToPrefabMapping.Add(typeof(GravityGunProjectileController), NormalProjectilePrefab);
        ProjectileTypeToPrefabMapping.Add(typeof(RocketProjectileController), RocketProjectilePrefab);
        ProjectileTypeToPrefabMapping.Add(typeof(ShotgunProjectileController), ShotGunProjectilePrefab);

        MakePool(NormalProjectilePrefab, typeof(GravityGunProjectileController));
        MakePool(RocketProjectilePrefab, typeof(RocketProjectileController));
        MakePool(ShotGunProjectilePrefab, typeof(ShotgunProjectileController));
    }

    public GravityGunProjectileController GetProjectile(Type type)
    {
        var pool = _projectilePools[type];
        if(pool.Count == 0)
        {
            var prefab = ProjectileTypeToPrefabMapping[type];
            MakeNewItem(prefab, type);
        }

        var obj = pool[0];
        pool.RemoveAt(0);
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void ReturnProjectile(GravityGunProjectileController projectile, Type type)
    {
        _projectilePools[type].Add(projectile);
        projectile.transform.rotation = Quaternion.identity;

        projectile.transform.position = new Vector3(1000000, 1000000, 0);
    }

    private void MakePool(GameObject prefab, Type type)
    {
        List<GravityGunProjectileController> projectiles = new List<GravityGunProjectileController>();
        _projectilePools.Add(type, projectiles);
        for (int i = 0; i < _startingPoolSize; i++)
        {
            MakeNewItem(prefab, type);
        }
    }

    private void MakeNewItem(GameObject prefab, Type type)
    {
        var obj = Instantiate(prefab);
        _projectilePools[type].Add(obj.GetComponent<GravityGunProjectileController>());
        NetworkServer.Spawn(obj);
        //we cant just set it to be unactive because then it wouldent sync
        obj.transform.position = new Vector3(1000000, 1000000, 0);

    }

    public static Type ConvertProjectileControllerTypeToType(ProjectileControllerType type)
    {
        if (type == ProjectileControllerType.Rocket)
        {
            return typeof(RocketProjectileController);
        }
        if (type == ProjectileControllerType.Shotgun)
        {
            return typeof(ShotgunProjectileController);
        }
        else
        {
            return typeof(GravityGunProjectileController);
        }
    }
}
