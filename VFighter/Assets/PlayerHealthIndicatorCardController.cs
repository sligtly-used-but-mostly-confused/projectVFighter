using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class PlayerHealthIndicatorCardController : MonoBehaviour {

    public Color AliveColor;
    public Color DeadColor;

    [SerializeField]
    private GameObject _healthIndicatorCellPrefab;
    [SerializeField]
    private GameObject _healthCellContainer;

    private PlayerController _attachedPlayer;

    private List<GameObject> _cells = new List<GameObject>();

    [System.Serializable]
    public class CharacterTypeIconPair
    {
        public PlayerCharacterType Type;
        public Sprite Icon;
    }

    public List<CharacterTypeIconPair> IconMappings;
    public Image CharacterIcon;
    public TextMeshProUGUI PlayerNumberText;
    public TextMeshProUGUI CharacterTypeText;

    private PlayerCharacterType DisplayedCharacterType = PlayerCharacterType.ShotGun;

    public void Init(PlayerController player)
    {
        _attachedPlayer = player;
        PlayerNumberText.text = "P" + _attachedPlayer.PlayerId;
        player.GetComponent<CharacterSelectController>().OnCharacterChanged += OnCharacterTypeChange;
        player.GetComponent<CharacterSelectController>().OnPlayerColorChanged += OnPlayerColorChange;
    }

    public void OnDestroy()
    {
        //player got destoryed before we could detach things
        if(!_attachedPlayer)
        {
            return;
        }

        _attachedPlayer.GetComponent<CharacterSelectController>().OnCharacterChanged -= OnCharacterTypeChange;
        _attachedPlayer.GetComponent<CharacterSelectController>().OnPlayerColorChanged -= OnPlayerColorChange;
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
                _cells[i].GetComponent<Image>().color = DeadColor;
            }

            for (; i < _attachedPlayer.ControlledPlayer.NumLives; i++)
            {
                _cells[i].GetComponent<Image>().color = AliveColor;
            }
        }
	}

    public void OnPlayerColorChange(Color color)
    {
        CharacterIcon.color = color;
        CharacterTypeText.color = color;
        PlayerNumberText.color = color;
    }

    public void OnCharacterTypeChange(PlayerCharacterType type)
    {
        DisplayedCharacterType = type;
        var pair = IconMappings.Find(x => x.Type == DisplayedCharacterType);
        CharacterIcon.sprite = pair.Icon;
        CharacterTypeText.text = type.ToString();
    }
}
