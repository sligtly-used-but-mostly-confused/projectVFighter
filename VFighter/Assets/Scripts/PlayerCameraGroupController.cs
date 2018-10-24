using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

public class PlayerCameraGroupController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var players = FindObjectsOfType<PlayerController>().Select(x => {
            var target = new CinemachineTargetGroup.Target
            {
                target = x.transform,
                weight = 1
            };
            
            return target;
        }).ToArray();

        GetComponent<CinemachineTargetGroup>().m_Targets = players;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
