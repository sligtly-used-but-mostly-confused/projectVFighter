using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionToPlayerController : MonoBehaviour
{
    [SerializeField]
    LineRenderer _connectionToAttachedPlayer;

    private PlayerController _attachedPlayer;

    public void ConnectToPlayer(PlayerController player)
    {
        _attachedPlayer = player;
    }

    public void DisconnectPlayer()
    {
        _attachedPlayer = null;
    }

    private void Update()
    {
        Vector3[] positions;
        if (_attachedPlayer)
        {
            positions = new Vector3[] { transform.position, _attachedPlayer.transform.position };
            _connectionToAttachedPlayer.material = _attachedPlayer.GetComponentInChildren<PlayerIdentifier>().GetComponent<Renderer>().material;
        }
        else
        {
            positions = new Vector3[] { Vector3.zero, Vector3.zero };
        }

        _connectionToAttachedPlayer.SetPositions(positions);
    }
}