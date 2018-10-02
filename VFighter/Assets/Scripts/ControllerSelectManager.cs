using System.Collections;
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
    private bool _isWaitingForReady = true;

    private Dictionary<InputDevice, bool> readyControllers = new Dictionary<InputDevice, bool>();
    //private Dictionary<short, InputDevice> _controllerIdToInputDevice = new Dictionary<short, InputDevice>();

    public List<InputDevice> DevicesWaitingForPlayer = new List<InputDevice>();
    //[SerializeField]
    //private List<short> _localControllerIds = new List<short>();

    [SerializeField]
    private GameObject NetworkPlayerPrefab;

    public int numLivesPerPlayer;

    private void Awake()
    {
        if(_instance)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update () {
        if(_isWaitingForReady)
        {
            CheckForNewControllers();
            if (CheckForAllPlayersReady())
            {
                _isWaitingForReady = false;
                FindObjectsOfType<PlayerController>().ToList().ForEach(x => x.IsReady = false);
                GameManager.Instance.StartGame("LongLevel", 10);
            }
        }
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

                bool isKeyboard = inputDevice is KeyboardMouseInputDevice;

                var player = new Player(numLivesPerPlayer, isKeyboard);
                player.NetworkControllerId = 1;

                
                SpawnPlayer(0);

                PlayerManager.Instance.AddPlayer(player);

                //_controllerIdToInputDevice.Add(player.NetworkControllerId, inputDevice);

                readyControllers.Add(inputDevice, false);
                DevicesWaitingForPlayer.Add(inputDevice);
            }
        }
        
        
    }

    /*
    public InputDevice GetPairedInputDevice(short controllerId)
    {
        InputDevice device = null;
        if (_controllerIdToInputDevice.TryGetValue(controllerId, out device))
        {
            return device;
        }

        return null;
    }
    */

    public void Init()
    {
        _isWaitingForReady = true;
    }

    private bool CheckForAllPlayersReady()
    {
        bool allplayersReady = _usedDevices.Count > 0;

        if (isServer)
        {
            return allplayersReady && FindObjectsOfType<PlayerController>().All(x => x.IsReady);
        }

        return false;
    }
}
