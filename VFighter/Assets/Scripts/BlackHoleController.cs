using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlackHoleController : MonoBehaviour {

    public Transform center;
    public float MeanTimeToStart = 5;
    public float GrowthScale = .1f;
    public float force = 5;

    public bool HasStarted = false;

	// Use this for initialization
	void Start () {
        StartCoroutine(Progression());
	}
	
	private IEnumerator Progression()
    {
        HasStarted = false;
        MeanTimeToStart = MeanTimeToStart * Random.value;
        GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(MeanTimeToStart);
        GetComponent<Renderer>().enabled = true;
        HasStarted = true;
        var objs = FindObjectsOfType<ControllableGravityObjectRigidBody>().ToList();
        center = objs[(int)(Random.value * objs.Count)].transform;
        transform.position = center.position;
        float timeElapsed = 0;
        while(true)
        {
            timeElapsed += Time.deltaTime * GrowthScale * GameManager.Instance.TimeScale;
            transform.localScale = Vector3.one * timeElapsed * timeElapsed;
            yield return new WaitForEndOfFrame();
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if(HasStarted)
        {
            var gravityObjectRB = collision.GetComponent<ControllableGravityObjectRigidBody>();

            if (gravityObjectRB)
            {
                Collider2D MagnetCollider = GetComponent<Collider2D>();
                var dis = collision.Distance(MagnetCollider);
                var dir = -1 * dis.normal * Mathf.Abs(dis.distance);
                var forceVector = Vector3.zero;
                var changeInGravDirection = dir.normalized - gravityObjectRB.GravityDirection;
                var newGravDirection = gravityObjectRB.GravityDirection + changeInGravDirection * force * Time.deltaTime;

                gravityObjectRB.ChangeGravityDirectionInternal(newGravDirection.normalized);
            }

            var player = collision.GetComponent<PlayerController>();

            if (player)
            {
                player.Kill();
            }
        }
    }
}
