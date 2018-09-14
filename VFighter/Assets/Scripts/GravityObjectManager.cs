using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GravityObjectManager : MonoBehaviour {

    private static GravityObjectManager _instance;
    public static GravityObjectManager Instance { get { return _instance; } }

    public List<PlayerController> Players;
    public List<GravityObjectRigidBody> GravityObjects;
    public List<GravityObjectRigidBody> GravityObjectsNotPlayers;

    void Awake () {
        _instance = this;
        Players = FindObjectsOfType<PlayerController>().ToList();
        GravityObjects = FindObjectsOfType<GravityObjectRigidBody>().ToList();

        GravityObjects.Where(x => x);
        GravityObjectsNotPlayers = (from x in GravityObjects
        where !x.GetComponent<PlayerController>()
        select x).ToList();
    }
	
	public List<PlayerController> GetOtherPlayers(PlayerController player)
    {
        return null;
    }
}
