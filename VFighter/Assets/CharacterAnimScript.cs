using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimScript : MonoBehaviour {
    Animator m_animator;
    Rigidbody2D rb;
    bool facingRight;

    [SerializeField]
    protected GameObject character;
    [SerializeField]
    protected GameObject characterContainer;

	// Use this for initialization
	void Start () {
        m_animator = character.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        facingRight = true;
	}
	
	// Update is called once per frame
	void Update () {
        int angleY = 180;
        if(rb.velocity.x < 0){
            facingRight = false;
        }
        if(rb.velocity.x > 0){
            facingRight = true;
        }
        if(Mathf.Abs(rb.velocity.x) < 0.1){
            m_animator.SetBool("IsRunning", false);
            
        }
        else{
            m_animator.SetBool("IsRunning", true);
            
        }
        if(facingRight){
            angleY = 0;

        }
        else{
            m_animator.SetFloat("Horizontal", -m_animator.GetFloat("Horizontal"));
        }
        //Vector3 angles = characterContainer.transform.rotation.eulerAngles;
        characterContainer.transform.rotation = Quaternion.Euler(characterContainer.transform.localRotation.x, angleY, 0);
	}
}
