using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

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

            if (CheckForPlayerReady())
            {
                _isWaitingForReady = false;
                foreach (var usedInput in _usedDevices)
                {
                    readyControllers[usedInput] = false;
                }

                GameManager.Instance.StartGame("LongLevel", 10);
            }
        }
    }

    private void SpawnPlayer(Player player)
    {
        if(!ClientScene.AddPlayer(player.NetworkControllerId))
        {
            player.NetworkControllerId = ++Player.IdCnt;
            SpawnPlayer(player);
        }
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
                int matIndex = (int)(UnityEngine.Random.value * (_playerMaterials.Count - 1));
                Material mat = _playerMaterials[matIndex];
                _playerMaterials.RemoveAt(matIndex);
                bool isKeyboard = inputDevice is KeyboardMouseInputDevice;
                Player player = new Player(numLivesPerPlayer, isKeyboard);
                player.NetworkControllerId = 1;
                PlayerManager.Instance.AddPlayer(player);
                SpawnPlayer(player);
                
                readyControllers.Add(inputDevice, false);
            }
        }
        
        
    }

    public void Init()
    {
        _isWaitingForReady = true;
    }

    private bool CheckForPlayerReady()
    {
        bool allplayersReady = _usedDevices.Count > 1;

        foreach (var usedInput in _usedDevices)
        {
            var readyPressed = usedInput.GetButtonDown(MappedButton.Ready);
            if (readyPressed)
            {
                readyControllers[usedInput] = !readyControllers[usedInput];
            }

            allplayersReady &= readyControllers[usedInput];
        }

        return allplayersReady;
    }
}
