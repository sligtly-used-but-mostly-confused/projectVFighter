using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamepadInput : MonoBehaviour
{
    private static GamepadInput _instance;
    public static GamepadInput Instance { get { return _instance; } }
	GamepadManager manager;

	public List<GamepadDevice> gamepads { 
		get{
			return manager.gamepads;
		}
	}

	public GamepadLayout xGamepadLayout;
	public List<UnGamepadConfig> unGamepadConfigs;

	public event System.Action<GamepadDevice> OnGamepadAdded;
	public event System.Action<GamepadDevice> OnGamepadRemoved;
    /*
    private void Awake()
    {

    }
    */
    void Awake()
	{
        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
#if UNITY_STANDALONE_WIN
        manager = new XGamepadManager (xGamepadLayout);
#else
		manager = new UnGamepadManager (unGamepadConfigs);
#endif
	
		manager.OnGamepadAdded += GamepadAdded;
		manager.OnGamepadRemoved += GamepadRemoved;
		
		manager.Init ();
    }

	void OnDestroy()
	{
		manager.OnGamepadAdded -= GamepadAdded;
		manager.OnGamepadRemoved -= GamepadRemoved;
	}

	void Update()
	{
		manager.Update ();
	}
	
	void GamepadAdded(GamepadDevice gamepadDevice)
	{
		if (OnGamepadAdded != null)
			OnGamepadAdded (gamepadDevice);
	}

	void GamepadRemoved(GamepadDevice gamepadDevice)
	{
		if (OnGamepadRemoved != null)
			OnGamepadRemoved (gamepadDevice);
	}
}

