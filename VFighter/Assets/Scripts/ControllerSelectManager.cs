using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSelectManager : MonoBehaviour {

    [SerializeField]
    private List<Material> _playerMaterials = new List<Material>();

    [SerializeField]
    private List<InputDevice> _usedDevices = new List<InputDevice>();

    private Dictionary<InputDevice, bool> readyControllers = new Dictionary<InputDevice, bool>();
    
	void Update () {
        
		foreach(var inputDevice in MappedInput.InputDevices)
        {
            if(!_usedDevices.Contains(inputDevice) && 
                (inputDevice.GetIsAxisTapped(MappedAxis.ShootGravGun) || inputDevice.GetButton(MappedButton.ShootGravGun)) &&
                !(inputDevice is KeyboardInputDevice || inputDevice is MouseInputDevice) )
            {
                _usedDevices.Add(inputDevice);
                int matIndex = (int)(Random.value * (_playerMaterials.Count - 1));
                Material mat = _playerMaterials[matIndex];
                _playerMaterials.RemoveAt(matIndex);
                Player player = new Player(inputDevice, mat);
                sadasdasd
                    //make an add keyboard player
                PlayerManager.Instance.AddPlayer(player);
                readyControllers.Add(inputDevice, false);
            }  
        }

        bool allplayersReady = _usedDevices.Count > 1;

        foreach(var usedInput in _usedDevices)
        {
            var readyPressed = usedInput.GetButtonDown(MappedButton.Ready);
            if(readyPressed)
            {
                readyControllers[usedInput] = !readyControllers[usedInput];
            }

            allplayersReady &= readyControllers[usedInput];
        }

        if(allplayersReady)
        {
            Debug.Log("ready");
        }

        //Debug.Log(MappedInput.ActiveDevice.name);
	}
}
