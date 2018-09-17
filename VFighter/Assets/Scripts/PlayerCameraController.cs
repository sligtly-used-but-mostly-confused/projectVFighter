using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Camera))]
public class PlayerCameraController : MonoBehaviour {

    private PlayerController _player;
    private Rigidbody2D _rB;
    void Start()
    {
        _player = GameManager.Instance.Player;
        _rB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        var displacement = _player.transform.position - transform.position;
        _rB.velocity = displacement.normalized * Mathf.Pow(displacement.magnitude, 1.2f );
    }
}
