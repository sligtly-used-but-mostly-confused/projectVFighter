using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

public class PlayerCameraGroupController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(CheckForPlayers());
	}
	
	private IEnumerator CheckForPlayers()
    {
        var players = FindObjectsOfType<PlayerController>().Select(x => {
            var target = new CinemachineTargetGroup.Target
            {
                target = x.transform,
                weight = 1
            };

            return target;
        }).ToArray();

        GetComponent<CinemachineTargetGroup>().m_Targets = players;
        yield return new WaitForSeconds(.1f);
        yield return CheckForPlayers();
    }
}
