using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScorePanelController : MonoBehaviour {

    [SerializeField]
    private GameObject _playerCardPrefab;

	// Use this for initialization
	void Start () {
        FindObjectsOfType<PlayerController>().ToList().ForEach(x => {
            var card = Instantiate(_playerCardPrefab);
            card.transform.SetParent(transform);
            card.GetComponent<PlayerCardController>().AttachToPlayer(x);
        });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
