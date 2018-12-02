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
        GameManager.Instance.TimeScale = (int)GameManager.Instance.TimeScale == 0 ? 1 : 0;
        _playerWhoCalledMenu = player;
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

        SceneManager.LoadScene(MainMenu.SceneName);
    }

    public bool IsMenuDisplayed()
    {
        return _menuObject.activeSelf;
    }

    public void DropPlayer()
    {
        _playerWhoCalledMenu.DropPlayer();
        ToggleMenu(null);
    }
}
