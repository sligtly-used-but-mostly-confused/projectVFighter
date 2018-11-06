using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalGravityGunProjectile : GravityGunProjectileController {

    public Collider2D MagnetCollider;

    public void Update()
    {
        Collider2D[] res = new Collider2D[100];
        var numRes = MagnetCollider.OverlapCollider(new ContactFilter2D(), res);
        for(int i = 0; i < numRes; i++)
        {
            var obj = res[i];
            Debug.Log(obj.transform.position - transform.position + " " + obj.name);
            if (obj.GetComponent<ControllableGravityObjectRigidBody>())
            {
                Debug.Log(obj.transform.position - transform.position);
            }
        }
        
    }
}
