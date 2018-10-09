﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;
using System.Linq;

public class ControllerSelectManager : NetworkBehaviour {

    private static ControllerSelectManager _instance;
    public static ControllerSelectManager Instance { get { return _instance; } }

    [SerializeField]
    private List<Material> _playerMaterials = new List<Material>();
    [SerializeField]
    private List<InputDevice> _usedDevices = new List<InputDevice>();
    [SerializeField]
    private GameObject NetworkPlayerPrefab;
    
    public List<InputDevice> DevicesWaitingForPlayer = new List<InputDevice>();

    private void Awake()
    {
        if(_instance)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        FindObjectsOfType<PlayerController>().ToList().ForEach(x =>
        {
            if (x.InputDevice)
            {
                _usedDevices.Add(x.InputDevice);
            }
        });
    }

    void Update () {
        CheckForNewControllers();
    }

    public void ClearUsedInputDevices()
    {
        _usedDevices.Clear();
        DevicesWaitingForPlayer.Clear();
    }

    private short SpawnPlayer(short id)
    {
        if(!ClientScene.AddPlayer(id))
        {
           return SpawnPlayer((short)(id + 1));
        }

        return id;
    }

    private void CheckForNewControllers()
    {
        foreach (var inputDevice in MappedInput.InputDevices)
        {
            if (!_usedDevices.Contains(inputDevice) &&
                (inputDevice.GetIsAxisTapped(MappedAxis.ShootGravGun) || inputDevice.GetButton(MappedButton.ShootGravGun)) &&
                !(inputDevice is KeyboardInputDevice || inputDevice is MouseInputDevice) &&
                ClientScene.readyConnection != null)
            {
                _usedDevices.Add(inputDevice);

                var player = new Player(LevelSelectManager.Instance.numLivesPerPlayer);
                player.NetworkControllerId = 1;
                
                SpawnPlayer(0);
                
                DevicesWaitingForPlayer.Add(inputDevice);
            }
        }  
    }
}
