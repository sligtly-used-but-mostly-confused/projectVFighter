using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkConnectionUIManager : MonoBehaviour {

	public void StartServer()
    {
        CustomNetworkManager.Instance.StartHost();
    }

    public void ConnectToServer(Text NetworkAddressInput)
    {
        CustomNetworkManager.Instance.networkAddress = NetworkAddressInput.text;
        CustomNetworkManager.Instance.StartClient();
    }

    public void StartMachMaking()
    {

    }
}
