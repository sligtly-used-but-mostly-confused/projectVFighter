using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHealthIndicatorUIController : MonoBehaviour {

    [SerializeField]
    private GameObject PlayerHealthUIPrefab;

    // Use this for initialization
    private void OnEnable()
    {
        StartCoroutine(WaitForGameManagerToSpawn());
	}

    public IEnumerator WaitForGameManagerToSpawn()
    {
        while(!GameManager.Instance)
        {
            yield return new WaitForEndOfFrame();
        }
        GameManager.Instance.OnPlayerJoin += MakeHealthDisplay;
        
        var players = FindObjectsOfType<PlayerController>().ToList();
        players.Sort((x, y) => x.PlayerId.CompareTo(y.PlayerId));
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
        player.GetComponent<CharacterSelectController>().RefreshCurrentMaterial();
    }
}
