using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetShower : MonoBehaviour {
    [SerializeField]
    public GameObject gb;
    [SerializeField]
    public int numAstro = 0;
    [SerializeField]
    private List<GameObject> temp;
   
    // Use this for initialization
    void Start () {
       
        for(int i = 0; i < numAstro; ++i)
        {
            Rigidbody2D rd;
            GameObject tempObject;
            int height = Random.Range(0, 5);
            int width = Random.Range(0, 5);
            tempObject = Instantiate(gb);
            tempObject.transform.localPosition = new Vector3(width, height, 0);
            temp.Add(tempObject);
            rd = temp[i].GetComponent<Rigidbody2D>();
            rd.AddForce(new Vector2(-15f,2f),ForceMode2D.Impulse);

        }
	}
	
	// Update is called once per frame
	void Update () {
          for(int i = 0; i < 10; ++i)
        {
            if (temp[i])
            {

            }
        }
    }
}
