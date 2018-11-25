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
    [SerializeField]
    private SceneAsset MainMenu;

    private KeyboardPlayerController menuToggler;

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

    public void ToggleMenu(KeyboardPlayerController kbpc)
    {
        menuToggler = kbpc;
        Debug.Log(gameObject + " " + _menuObject);
        _menuObject.SetActive(!_menuObject.activeSelf);
    }

    public void Disconnect()
    {
        _menuObject.SetActive(false);
        SceneManager.LoadScene(MainMenu.name);
    }

    public void RemoveCharacter(){
        Destroy(menuToggler.gameObject);
        _menuObject.SetActive(false);
    }
}
