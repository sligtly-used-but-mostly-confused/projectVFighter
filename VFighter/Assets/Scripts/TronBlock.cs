using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

[RequireComponent(typeof(GravityObjectRigidBody))]
[RequireComponent(typeof(BoxCollider2D))]
public class TronBlock : NetworkBehaviour {
    private GravityObjectRigidBody _gORB;

    [SerializeField]
    private GameObject _tronBlockTailPrefab;
    private GameObject _lastTailPlaced;
    [SerializeField]
    private float _secondsForTailToSurvive = 1f;

    private List<GameObject> _tails = new List<GameObject>();

	void Start () {
        _gORB = GetComponent<GravityObjectRigidBody>();
    }

    private void OnDestroy()
    {
        if (isServer)
            _tails.ForEach(x => Destroy(x));
    }

    private void SpawnNewTail()
    {
            
        var newTail = Instantiate(_tronBlockTailPrefab);
        _lastTailPlaced = newTail;
        _lastTailPlaced.transform.localPosition = transform.position;
        _lastTailPlaced.GetComponent<ControllableGravityObjectRigidBody>().LastShotBy = GetComponent<ControllableGravityObjectRigidBody>().LastShotBy;
        _lastTailPlaced.GetComponent<TronTail>().SecondsForTailToSurvive = _secondsForTailToSurvive;
        StartCoroutine(DestoryTailCoroutine(newTail));
        _tails.Add(_lastTailPlaced);
        NetworkServer.Spawn(newTail);
        
    }

    private void Update()
    {
        if(isServer)
        {
            if (_lastTailPlaced == null)
            {
                SpawnNewTail();
            }
            else
            {
                var min = _lastTailPlaced.GetComponent<BoxCollider2D>().bounds.min;
                var max = _lastTailPlaced.GetComponent<BoxCollider2D>().bounds.max;
                float _tailSize = (max - min).sqrMagnitude;
                if ((_lastTailPlaced.transform.position - transform.position).magnitude > _tailSize / 8)
                {
                    SpawnNewTail();
                }
            }
        }
    }
    

    private IEnumerator DestoryTailCoroutine(GameObject tailBlock)
    {
        yield return new WaitForSeconds(_secondsForTailToSurvive);
        if(_lastTailPlaced = tailBlock)
        {
            _lastTailPlaced = null;
        }
        _tails.Remove(tailBlock);
        //Destroy(tailBlock);
    }
}
