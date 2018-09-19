using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[RequireComponent(typeof(GamepadInput))]
public class MappedInput : MonoBehaviour {

	public float LastInputTime {get; private set;}

	public static MappedInput Instance {
		get; private set;
	}

	public static bool UseDisabledDevices;

	static InputDevice _activeDevice;
	public static InputDevice ActiveDevice
	{
		get{
			return _activeDevice;
		}
		set{
			if( _activeDevice == value )
				return;

			_activeDevice = value;
			if( OnActiveDeviceChanged != null )
				OnActiveDeviceChanged(_activeDevice);
		}
	}

	GamepadInput _gamepadInput;

	public GamepadInput GamepadInput {
		get {
			if (!_gamepadInput)
				_gamepadInput = GetComponent<GamepadInput> ();
			return _gamepadInput;
		}
	}

	public static System.Action<InputDevice> OnDeviceAdded;
	public static System.Action<InputDevice> OnDeviceRemoved;

	public static List<InputDevice> InputDevices = new List<InputDevice>();

	public GamepadInputMapping GamepadInputMapping;
	public KeyboardInputMapping KeyboardInputMapping;
	public MouseInputMapping MouseInputMapping;

    public static System.Action<InputDevice> OnActiveDeviceChanged;

    public static InputDevice Mouse;
    public static InputDevice KeyBoard;


    void Awake()
	{
        if(Instance)
        {
            Destroy(gameObject);
            return;
        }
#if UNITY_STANDALONE || UNITY_EDITOR

		if( MouseInputMapping != null)
			AddMouseDevice ();

		if(KeyboardInputMapping != null)
			AddKeyboardDevice ();

        if (MouseInputMapping != null && KeyboardInputMapping != null)
            AddKeyboardMouseDevice();

#elif UNITY_IOS || UNITY_ANDROID
		AddMouseDevice ();
#endif

            Instance = this;
	}

	void Start()
	{
		for (int i = 0; i < GamepadInput.gamepads.Count; i++)
		{
			OnGamepadAdded (GamepadInput .gamepads[i]);
		}

		GamepadInput.OnGamepadAdded += OnGamepadAdded;
		GamepadInput.OnGamepadRemoved += OnGamepadRemoved;
	}

	void OnDestroy()
	{
		GamepadInput.OnGamepadAdded -= OnGamepadAdded;
		GamepadInput.OnGamepadRemoved -= OnGamepadRemoved;
	}

	void Update()
	{
		for (int i = 0; i < InputDevices.Count; i++)
		{
			if (!InputDevices [i].autoActive && !UseDisabledDevices)
				continue;
			if (!ActiveDevice || InputDevices [i].lastInputTime > ActiveDevice.lastInputTime)
				ActiveDevice = InputDevices [i];
			LastInputTime = Mathf.Max (InputDevices [i].lastInputTime, LastInputTime);
		}
	}
		
	void OnGamepadAdded(GamepadDevice gamepad)
	{
		GameObject obj = new GameObject ();
		var device = obj.AddComponent<GamepadInputDevice> ();
		(device as GamepadInputDevice).gamepad = gamepad;
		obj.transform.parent = transform;

		obj.name = "Device: "+ gamepad.displayName;
		InputDevices.Add (device);
		
		if( OnDeviceAdded != null )
			OnDeviceAdded(device);
	}

	void OnGamepadRemoved(GamepadDevice gamepad)
	{
		for(int i = 0; i < InputDevices.Count; i++)
		{
			if( InputDevices[i] is GamepadInputDevice && (InputDevices[i] as GamepadInputDevice).gamepad == gamepad)
			{
				var device = InputDevices[i];
				InputDevices.Remove (device);

				if( OnDeviceRemoved != null )
					OnDeviceRemoved(device);
				Destroy(device.gameObject);
			}
		}
	}

	void AddKeyboardDevice()
	{
		GameObject obj = new GameObject ("Keyboard",typeof(KeyboardInputDevice));
		obj.transform.parent = transform;
		KeyBoard = obj.GetComponent<KeyboardInputDevice> ();
		InputDevices.Add(KeyBoard);
	}

	void AddMouseDevice()
	{
        Debug.Log("setting mouse");
        GameObject obj = new GameObject ("Mouse",typeof(MouseInputDevice));
		obj.transform.parent = transform;
		Mouse = obj.GetComponent<MouseInputDevice> ();
		InputDevices.Add(Mouse);
	}

    void AddKeyboardMouseDevice()
    {
        Debug.Log("setting KeyboardMouse");
        GameObject obj = new GameObject("KeyboardMouse", typeof(KeyboardMouseInputDevice));
        obj.transform.parent = transform;
        var KeyboardMouse = obj.GetComponent<KeyboardMouseInputDevice>();
        InputDevices.Add(KeyboardMouse);
    }
}