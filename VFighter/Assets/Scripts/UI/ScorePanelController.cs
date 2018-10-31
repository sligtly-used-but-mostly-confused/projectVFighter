using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ScorePanelController : MonoBehaviour {

    [SerializeField]
    private GameObject _playerCardPrefab;
    [SerializeField]
    private GameObject PlayerCardContainer;
    [SerializeField]
    private Text RoundNumberText;
	// Use this for initialization
	void Start () {
        FindObjectsOfType<PlayerController>().ToList().ForEach(x => {
            var card = Instantiate(_playerCardPrefab);
            card.transform.SetParent(PlayerCardContainer.transform);
            card.GetComponent<PlayerCardController>().AttachToPlayer(x);
        });

        RoundNumberText.text = "Round " + GameManager.Instance.RoundNumber + " of " + GameManager.Instance.NumRounds;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
