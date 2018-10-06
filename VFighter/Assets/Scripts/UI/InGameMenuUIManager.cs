using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenuUIManager : MonoBehaviour {
    public static InGameMenuUIManager Instance;
    [SerializeField]
    private GameObject _menuObject;

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

    public void ToggleMenu()
    {
        Debug.Log(gameObject + " " + _menuObject);
        _menuObject.SetActive(!_menuObject.activeSelf);
    }

    public void Disconnect()
    {
        _menuObject.SetActive(false);
        CustomNetworkManager.Instance.StopHost();
        Destroy(CustomNetworkManager.Instance.gameObject);
    }
}
