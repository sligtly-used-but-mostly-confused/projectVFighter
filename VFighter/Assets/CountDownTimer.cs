using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownTimer : MonoBehaviour {
    public static CountDownTimer Instance;

    [SerializeField]
    private Text _countDownText;
    [SerializeField]
    private float _timePerTick = 1;

    private void Awake()
    {
        Instance = this;
    }

    void Start () {
        GameManager.Instance.TimeScale = 0;
        //StartCoroutine(CountDown());
	}
	
	public IEnumerator CountDown()
    {
        _countDownText.text = "3";
        yield return new WaitForSeconds(_timePerTick);
        _countDownText.text = "2";
        yield return new WaitForSeconds(_timePerTick);
        _countDownText.text = "1";
        yield return new WaitForSeconds(_timePerTick);
        if(FindObjectOfType<PlayerController>().isServer)
            LevelManager.Instance.StartGame();
        Destroy(gameObject);
    }
}
