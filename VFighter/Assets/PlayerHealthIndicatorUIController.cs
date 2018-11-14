using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHealthIndicatorUIController : MonoBehaviour {

    [SerializeField]
    private GameObject PlayerHealthUIPrefab;

	// Use this for initialization
	void Start () {
        GameManager.Instance.OnPlayerJoin += MakeHealthDisplay;
        var players = FindObjectsOfType<PlayerController>().ToList();
        players = players.OrderBy(x => x.netId.Value).ToList();
        players.ForEach(x => MakeHealthDisplay(x));
	}

    private void OnDestroy()
    {
        GameManager.Instance.OnPlayerJoin -= MakeHealthDisplay;
    }

    private void MakeHealthDisplay(PlayerController player)
    {
        var healthIndicator = Instantiate(PlayerHealthUIPrefab);
        healthIndicator.transform.SetParent(transform);
        healthIndicator.GetComponent<PlayerHealthIndicatorCardController>().Init(player);
    }
}
