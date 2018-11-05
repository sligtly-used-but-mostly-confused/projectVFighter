using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityChange : MonoBehaviour
{
   [SerializeField]
    public ParticleSystem gP;
    [SerializeField]
    public GravityObjectRigidBody rd;
    float pos = -1.5f;
    private Coroutine _playEffectCoroutine;
    void Start()
    {
        gP = GetComponent<ParticleSystem>();
        rd = GetComponentInParent<GravityObjectRigidBody>();
    }

    //1 for up, -1 for down
    public void PlayEffect(GravityObjectRigidBody GORB)
    {
        Debug.Log("effect");
        int dir = GORB.GravityDirection.y > 0 ? 1 : -1;

        if(_playEffectCoroutine != null)
        {
            StopCoroutine(_playEffectCoroutine);
            if (!gP.isPlaying)
            {
                gP.Stop();
            }
        }

        _playEffectCoroutine = StartCoroutine(PlayEffectOvertime(dir));
    }

    private IEnumerator PlayEffectOvertime(int dir)
    {
        var angle = 0f;
        if (dir == -1)
        {
            angle = 180;
        }

        float gDir = rd.GravityDirection.x;
        var main = gP.main;

        gP.Play();
        //this.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        this.transform.rotation = Quaternion.Euler(angle, 0, 0);

        yield return new WaitForSeconds(.5f);

        gP.Stop();

        if (!gP.isPlaying)
        {
            Debug.Log("stop");
            
        }
    }
}
