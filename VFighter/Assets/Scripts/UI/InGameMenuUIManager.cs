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
        _menuObject.SetActive(!_menuObject.activeSelf);
        GameManager.Instance.TimeScale = _menuObject.activeInHierarchy ? 0 : 1;
    }

    public void Disconnect()
    {
        _menuObject.SetActive(false);

        foreach(var player in FindObjectsOfType<PlayerController>())
        {
            player.DropPlayerInternal();
        }

        GameManager.Instance.IsInCharacterSelect = true;

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
