using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]

public class LevelZoneController : MonoBehaviour {

    public string levelName;
    public int playersInside = 0;
    public Vector3 emptySize;
    public Vector3 occupiedSize;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>()) ++playersInside;

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>()) --playersInside;
    }

    private void Update()
    {
        transform.localScale = playersInside > 0 ? occupiedSize : emptySize;
    }
}
