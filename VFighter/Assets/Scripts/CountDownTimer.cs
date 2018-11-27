using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountDownTimer : MonoBehaviour {
    public static CountDownTimer Instance;

    [SerializeField]
    private TextMeshProUGUI _countDownText;
    [SerializeField]
    private float _timePerTick = 1;
    [SerializeField]
    private GameObject _countDownObjects;
    [SerializeField]
    private GameObject _getReadyImage;
    [SerializeField]
    private GameObject _terminateImage;

    private void Awake()
    {
        if(Instance)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
    }

    void Start () {
        GameManager.Instance.TimeScale = 0;
        StartCoroutine(CountDown());
    }
	
	public IEnumerator CountDown()
    {
        if(_countDownText)
            _countDownText.text = "3";
        yield return new WaitForSeconds(_timePerTick);
        if (_countDownText)
            _countDownText.text = "2";
        yield return new WaitForSeconds(_timePerTick);
        if (_countDownText)
            _countDownText.text = "1";
        yield return new WaitForSeconds(_timePerTick);
        _countDownObjects.SetActive(false);
        _getReadyImage.SetActive(true);
        yield return new WaitForSeconds(_timePerTick);
        _terminateImage.SetActive(true);
        yield return new WaitForSeconds(_timePerTick);
        LevelManager.Instance.StartGame();
        if(gameObject)
            Destroy(gameObject);
    }
}
