using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLivesIndicatorGroupController : MonoBehaviour {

    public GameObject PlayerLifeIndicatorPrefab;
    private List<GameObject> _indicators = new List<GameObject>();

    private float _size = .75f;

    public Material AliveMaterial;
    public Material DeadMaterial;

    public PlayerController AttachedPlayer;

    void Init () {

        if(_indicators.Count > 0)
        {
            _indicators.ForEach(x => Destroy(x));
            _indicators.Clear();
        }

        float bottom = -_size/2;
        float top = _size/2;
        float indicatorSize = _size / AttachedPlayer.ControlledPlayer.NumLives;
        for (int i = 0; i < AttachedPlayer.ControlledPlayer.NumLives; i++)
        {
            var indicator = Instantiate(PlayerLifeIndicatorPrefab);
            _indicators.Add(indicator);
            indicator.transform.SetParent(transform);
            float y = 0;
            if (AttachedPlayer.ControlledPlayer.NumLives != 1)
            {
                y = Mathf.Lerp(bottom, top, i / (float)(AttachedPlayer.ControlledPlayer.NumLives - 1));
            }
                
            indicator.transform.localPosition = new Vector3(1f, y, 0);
            indicator.transform.localScale = new Vector3(.25f,indicatorSize,.25f);
        }

        _indicators.Reverse();
    }
	
	// Update is called once per frame
	void Update() {
        if(AttachedPlayer.ControlledPlayer.NumLives != _indicators.Count)
        {
            Init();
        }

        transform.position = AttachedPlayer.transform.position;

        int i = 0;
        for (; i < AttachedPlayer.ControlledPlayer.NumDeaths; i++)
        {
            _indicators[i].GetComponent<Renderer>().material = DeadMaterial;
        }

        for (; i < AttachedPlayer.ControlledPlayer.NumLives; i++)
        {
            _indicators[i].GetComponent<Renderer>().material = AliveMaterial;
        }
    }
}
