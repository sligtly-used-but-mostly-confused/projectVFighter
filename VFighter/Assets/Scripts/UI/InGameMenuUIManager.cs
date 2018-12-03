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

    public void ToggleMenu(PlayerController player)
    {
        _playerWhoCalledMenu = player;
        ToggleMenu();
    }

    public void ToggleMenu()
    {
        GameManager.Instance.TogglePause();
        if (SettingsUIController.IsSettingMenuDisplayed)
        {
            SettingsUIController.ToggleSettingsMenu();
            return;
        }

        _menuObject.SetActive(!_menuObject.activeSelf);
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
        GameManager.Instance.TogglePause();
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
