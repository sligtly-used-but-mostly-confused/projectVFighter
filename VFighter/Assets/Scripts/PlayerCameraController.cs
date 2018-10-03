﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Camera))]
public class PlayerCameraController : MonoBehaviour
{
    private Rigidbody2D _rB;

    [SerializeField]
    private float _minCameraSize = 5;
    [SerializeField]
    private float _maxCameraSize = 15;
    [SerializeField]
    private float _cameraSizePadding = 1.3f;

    [SerializeField]
    private float _targetCameraSize = 5;
    [SerializeField]
    private Vector3 _targetCenter = Vector3.zero;

    void Start()
    {
        _rB = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        UpdateTargets();

        var displacement = _targetCenter - transform.position;
        _rB.velocity = displacement.normalized * Mathf.Pow(displacement.magnitude, 2f);

        var deltaSize = _targetCameraSize - GetComponent<Camera>().orthographicSize;
        GetComponent<Camera>().orthographicSize += deltaSize * Mathf.Pow(Mathf.Abs(deltaSize), 2f) * Time.fixedDeltaTime * 2;
    }

    private void UpdateTargets()
    {
        var alivePlayers = FindObjectsOfType<PlayerController>().ToList().Where(x => !x.IsDead);

        float minX = alivePlayers.Min(x => x.transform.position.x);
        float maxX = alivePlayers.Max(x => x.transform.position.x);
        float deltaX = maxX - minX;

        float minY = alivePlayers.Min(x => x.transform.position.y);
        float maxY = alivePlayers.Max(x => x.transform.position.y);
        float deltaY = maxY - minY;

        Vector3 center = new Vector3(minX + deltaX / 2, minY + deltaY / 2, -10);
        _targetCenter = center;

        float aspectRatio = Screen.width / Screen.height;

        //assume that the y axis was bigger
        var yRatio = (new Vector2(deltaY * aspectRatio, deltaY)) / 2;
        //assume that the y axis was bigger
        var xRatio = (new Vector2(deltaX, deltaX * (1 / aspectRatio))) / 2;

        if (yRatio.magnitude > xRatio.magnitude)
        {
            _targetCameraSize = yRatio.y * _cameraSizePadding;
        }
        else
        {
            _targetCameraSize = xRatio.y * _cameraSizePadding;
        }

        _targetCameraSize = Mathf.Clamp(_targetCameraSize, _minCameraSize, _maxCameraSize);
    }
}
