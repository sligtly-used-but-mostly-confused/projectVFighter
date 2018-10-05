using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpikesController : NetworkBehaviour {

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() && isServer)
        {
            collision.gameObject.GetComponent<PlayerController>().Kill();
        }
    }
}
