using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetShower : MonoBehaviour
{
    [SerializeField]
    public GameObject gb;
    [SerializeField]
    public int numAstro = 0;
    [SerializeField]
    private List<GameObject> astroArray;

    // Use this for initialization
    void Start()
    {

        for (int i = 0; i < numAstro; ++i)
        {
            Rigidbody2D rd;
            GameObject tempObject;
            MeshRenderer mr;
            int height = Random.Range(110, 90);
            int width = Random.Range(120, 85);
            tempObject = Instantiate(gb);
            tempObject.transform.localPosition = new Vector3(width, height, 0);
            astroArray.Add(tempObject);
            float randScale = Random.Range(0.1f, 0.01f);
            astroArray[i].transform.localScale = new Vector3(randScale, randScale, randScale);
            rd = astroArray[i].GetComponent<Rigidbody2D>();
            mr = astroArray[i].GetComponent<MeshRenderer>();
            mr.enabled = false;
            rd.AddForce(new Vector2(Random.Range(-0.1f, -1f), Random.Range(0.1f, 0.3f)), ForceMode2D.Impulse);
        }
    }

    // Update is called once per frame
    void Update()
    {

      
        for (int i = 0; i < numAstro; ++i)
        {
            if (astroArray[i].transform.position.x <= 85f || astroArray[i].transform.position.y > 110f || astroArray[i].transform.position.y <= 85f)
            {
                Rigidbody2D rd;
                MeshRenderer mr;
                mr = astroArray[i].GetComponent<MeshRenderer>();
                mr.enabled = true;
                int height = Random.Range(120, 90);
                float forceVertical = Random.Range(-1, -2);
                astroArray[i].transform.position = new Vector3(120f, height, 0f);
                float randScale = Random.Range(0.1f, 0.01f);
                astroArray[i].transform.localScale = new Vector3(randScale, randScale, randScale);
                rd = astroArray[i].GetComponent<Rigidbody2D>();
                rd.velocity = Vector3.zero;
                rd.AddForce(new Vector2(Random.Range(-0.1f,-1f), Random.Range(0.1f,0.3f)), ForceMode2D.Impulse);
            }
        }
    }
}
