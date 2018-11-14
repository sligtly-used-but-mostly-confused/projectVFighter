using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthIndicatorCardController : MonoBehaviour {

    public Material AliveMaterial;
    public Material DeadMaterial;

    [SerializeField]
    private GameObject _healthIndicatorCellPrefab;
    [SerializeField]
    private GameObject _healthCellContainer;

    private PlayerController _attachedPlayer;

    private List<GameObject> _cells = new List<GameObject>();

    public void Init(PlayerController player)
    {
        _attachedPlayer = player;
    }

    // Update is called once per frame
    void Update () {
        if(_attachedPlayer)
        {
            if (_cells.Count != _attachedPlayer.ControlledPlayer.NumLives)
            {
                _cells.ForEach(x => Destroy(x));
                _cells.Clear();
                _cells = new List<GameObject>();
                for (int k = 0; k < _attachedPlayer.ControlledPlayer.NumLives; k++)
                {
                    var cell = Instantiate(_healthIndicatorCellPrefab);
                    cell.transform.SetParent(_healthCellContainer.transform, false);
                    _cells.Add(cell);
                }
            }

            int i = 0;
            for (; i < _attachedPlayer.ControlledPlayer.NumDeaths; i++)
            {
                _cells[i].GetComponent<Image>().material = DeadMaterial;
            }

            for (; i < _attachedPlayer.ControlledPlayer.NumLives; i++)
            {
                _cells[i].GetComponent<Image>().material = AliveMaterial;
            }
        }
	}
}
