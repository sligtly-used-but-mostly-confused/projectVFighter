using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownTimer : MonoBehaviour {
    [SerializeField]
    private Text _countDownText;
    [SerializeField]
    private float _timePerTick = 1;

	void Start () {
        GravityObjectRigidBody.TimeScale = 0;
        StartCoroutine(CountDown());
	}
	
	private IEnumerator CountDown()
    {
        _countDownText.text = "3";
        yield return new WaitForSeconds(_timePerTick);
        _countDownText.text = "2";
        yield return new WaitForSeconds(_timePerTick);
        _countDownText.text = "1";
        yield return new WaitForSeconds(_timePerTick);

        GravityObjectRigidBody.TimeScale = 1;
        LevelManager.Instance.StartGame();
        Destroy(gameObject);
    }
}
