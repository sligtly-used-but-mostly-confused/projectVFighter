using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEditor;

public class InGameMenuUIManager : MonoBehaviour {
    public static InGameMenuUIManager Instance;
    [SerializeField]
    private GameObject _menuObject;
    public SceneField MainMenu;
    public RoundSettingsUIController SettingsUIController;
    private PlayerController _playerWhoCalledMenu;

    private float _prevTimeScale;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        GameManager.Instance.TimeScale = _menuObject.activeInHierarchy ? 0 : GameManager.Instance.TimeScale;
    }

    public void ToggleMenu(PlayerController player)
    {
        _playerWhoCalledMenu = player;
        ToggleMenu();
    }

    public void ToggleMenu()
    {
        if(SettingsUIController.IsSettingMenuDisplayed)
        {
            SettingsUIController.ToggleSettingsMenu();
            return;
        }

        _menuObject.SetActive(!_menuObject.activeSelf);

        if(_menuObject.activeInHierarchy)
        {
            _prevTimeScale = GameManager.Instance.TimeScale;
        }
        else
        {
            GameManager.Instance.TimeScale = _prevTimeScale;
        }

    }

    public void Disconnect()
    {
        _menuObject.SetActive(false);

        
        foreach(var player in FindObjectsOfType<PlayerController>())
        {
            player.DropPlayerInternal();
        }

        GameManager.Instance.IsInCharacterSelect = true;
        GameManager.Instance.TimeScale = 1;
        SceneManager.LoadScene(MainMenu.SceneName);
    }

    public bool IsMenuDisplayed()
    {
        return _menuObject.activeSelf;
    }

    public void DropPlayer()
    {
        Debug.Log($"dropping {_playerWhoCalledMenu.name}");
        _playerWhoCalledMenu.DropPlayer();
        ToggleMenu(null);
    }
}
