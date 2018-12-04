using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityChange : MonoBehaviour
{
    [SerializeField]
    public ParticleSystem gP;
    [SerializeField]
    public GameObject gravChange;
    [SerializeField]
    public KeyboardPlayerController kp;
    [SerializeField]
    public GravityObjectRigidBody rd;
    GameObject objectTemp;
    float pos = -1.5f;
    private Coroutine _playEffectCoroutine;
    void Start()
    {
        rd = GetComponentInParent<GravityObjectRigidBody>();
        kp = GetComponentInParent<KeyboardPlayerController>();
    }

    //1 for up, -1 for down
    public void PlayEffect(GravityObjectRigidBody GORB)
    {
        int dir = GORB.GravityDirection.y > 0 ? 1 : -1;

        if (_playEffectCoroutine != null)
        {
            StopCoroutine(_playEffectCoroutine);


        }

        _playEffectCoroutine = StartCoroutine(PlayEffectOvertime(dir));
    }

    private IEnumerator PlayEffectOvertime(int dir)
    {
        objectTemp = Instantiate(gravChange);
        objectTemp.transform.position = kp.transform.position;

        gP = objectTemp.GetComponent<ParticleSystem>();
        var angle = 0f;
        if (dir == -1)
        {
            angle = 180f;
        }

        float gDir = rd.GravityDirection.x;
        var main = gP.main;

        //gP.Play();
        //this.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        objectTemp.transform.rotation = Quaternion.Euler(angle, 0, 0);

        yield return new WaitForSeconds(.5f);

        //gP.Stop();

       // if (!gP.isPlaying)
        //{
       //     Debug.Log("stop");

        //}
    }
}
