using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GravityObjectRigidBody))]
[RequireComponent(typeof(BoxCollider2D))]
public class TronBlock : MonoBehaviour {
    private GravityObjectRigidBody _gORB;

    [SerializeField]
    private GameObject _tronBlockTailPrefab;

    private GameObject _lastTailPlaced;
    [SerializeField]
    private float _tailSize = 0f;
    [SerializeField]
    private float _secondsForTailToSurvive = 1f;
	// Use this for initialization
	void Start () {
        _gORB = GetComponent<GravityObjectRigidBody>();
    }

    private void SpawnNewTail()
    {
        var newTail = Instantiate(_tronBlockTailPrefab);
        _lastTailPlaced = newTail;
        _lastTailPlaced.transform.localPosition = transform.position;
        StartCoroutine(DestoryTailCoroutine(newTail));
    }

    private void Update()
    {
        if(_lastTailPlaced == null)
        {
            SpawnNewTail();
        }
        else 
        {
            var min = _lastTailPlaced.GetComponent<BoxCollider2D>().bounds.min;
            var max = _lastTailPlaced.GetComponent<BoxCollider2D>().bounds.max;
            _tailSize = (max - min).sqrMagnitude;
            if (_lastTailPlaced.GetComponent<BoxCollider2D>().bounds.SqrDistance(transform.position) > _tailSize / 8)
                SpawnNewTail();
        }
    }

    private IEnumerator DestoryTailCoroutine(GameObject tailBlock)
    {
        yield return new WaitForSeconds(_secondsForTailToSurvive);
        if(_lastTailPlaced = tailBlock)
        {
            _lastTailPlaced = null;
        }

        Destroy(tailBlock);
    }
}
