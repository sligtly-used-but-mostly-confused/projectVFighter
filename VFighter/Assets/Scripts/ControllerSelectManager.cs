using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class ControllerSelectManager : MonoBehaviour
{

    private static ControllerSelectManager _instance;
    public static ControllerSelectManager Instance { get { return _instance; } }

    [SerializeField]
    private List<Material> _playerMaterials = new List<Material>();
    [SerializeField]
    private List<InputDevice> _usedDevices = new List<InputDevice>();
    [SerializeField]
    private GameObject PlayerPrefab;
    
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

    public void RemoveUsedDevice(InputDevice device)
    {
        _usedDevices.Remove(device);
    }

    public void ClearUsedInputDevices()
    {
        _usedDevices.Clear();
        DevicesWaitingForPlayer.Clear();
    }

    private short SpawnPlayer(short id)
    {
        Instantiate(PlayerPrefab);
        return id;
    }

    private void CheckForNewControllers()
    {
        foreach (var inputDevice in MappedInput.InputDevices)
        {
            if (!_usedDevices.Contains(inputDevice) &&
                (inputDevice.GetIsAxisTappedPos(MappedAxis.ShootGravGun) || inputDevice.GetButton(MappedButton.ShootGravGun)) &&
                !(inputDevice is KeyboardInputDevice || inputDevice is MouseInputDevice) &&
                !InGameMenuUIManager.Instance.IsMenuDisplayed())
            {
                _usedDevices.Add(inputDevice);
                
                SpawnPlayer(0);
                
                DevicesWaitingForPlayer.Add(inputDevice);
            }
        }  
    }
}
