using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class NetworkConnectionUIManager : MonoBehaviour {

    public SceneAsset ControllerSelect;

	public void StartServer()
    {
        //CustomNetworkManager.Instance.StartHost();
        SceneManager.LoadScene(ControllerSelect.name);
    }

    public void ConnectToServer(Text NetworkAddressInput)
    {
        //CustomNetworkManager.Instance.networkAddress = NetworkAddressInput.text;
        //CustomNetworkManager.Instance.StartClient();
    }

    public void StartMachMaking()
    {

    }
}
