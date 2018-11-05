using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ScorePanelController : MonoBehaviour {

    [SerializeField]
    private GameObject _playerCardPrefab;
    [SerializeField]
    private GameObject _emptyCardPrefab;
    [SerializeField]
    private GameObject PlayerCardContainer;
    [SerializeField]
    private Text RoundNumberText;
	// Use this for initialization
	void Start () {

        List<PlayerController> players = FindObjectsOfType<PlayerController>().ToList();
        int i = 0;
        for(; i < players.Count; i++)
        {
            var x = players[i];
            var card = Instantiate(_playerCardPrefab);
            card.transform.SetParent(PlayerCardContainer.transform);
            card.GetComponent<PlayerCardController>().AttachToPlayer(x);
        }
        
        for(; i < 4; i++)
        {
            var card = Instantiate(_emptyCardPrefab);
            card.transform.SetParent(PlayerCardContainer.transform);
        }


        RoundNumberText.text = "Round " + GameManager.Instance.RoundNumber + " of " + GameManager.Instance.NumRounds;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
