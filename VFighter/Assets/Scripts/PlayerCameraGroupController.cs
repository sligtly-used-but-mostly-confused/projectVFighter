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
        List<PlayerController> playerObjects = FindObjectsOfType<PlayerController>().ToList();

        if(playerObjects.Count == 0)
        {
            yield return new WaitForSeconds(.1f);
            yield return CheckForPlayers();
        }

        var players = playerObjects.Select(x => {
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
