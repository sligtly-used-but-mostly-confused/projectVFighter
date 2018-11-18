﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimScript : MonoBehaviour {
    public Animator currentAnimator;
    Rigidbody2D rb;
    bool facingRight;
    bool interrupt;

    [SerializeField]
    protected GameObject characterContainer;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        interrupt = false;
	}
	
	// Update is called once per frame
	void Update () {
        int angleY = 180;
        if(rb.velocity.y > 5 || rb.velocity.y < -5 && (GetComponent<GravityObjectRigidBody>().GravityDirection.y > .97 || GetComponent<GravityObjectRigidBody>().GravityDirection.y < -.97) ){
            if (!interrupt && (currentAnimator.GetBool("IsFloating") == false))
            {

                currentAnimator.SetTrigger("FloatTrigger");
                interrupt = true;
            }
            currentAnimator.SetBool("IsFloating", true);
        }
        else{
            if ((currentAnimator.GetBool("IsFloating") == true) || currentAnimator.GetCurrentAnimatorStateInfo(1).IsName("FloatCycle"))
            {

                currentAnimator.SetTrigger("LandTrigger");
                interrupt = false;
            }

            currentAnimator.SetBool("IsFloating", false);
        }
        if(rb.velocity.x < 0){
            facingRight = false;
        }
        if(rb.velocity.x > 0){
            facingRight = true;
        }
        if(Mathf.Abs(rb.velocity.x) < 0.1){
            currentAnimator.SetBool("IsRunning", false);
            
        }
        else{
            currentAnimator.SetBool("IsRunning", true);
            
        }
        if(facingRight){
            angleY = 0;

        }
        else{
            currentAnimator.SetFloat("Horizontal", -currentAnimator.GetFloat("Horizontal"));
        }

        var rotX = 180f;
        if (GetComponent<GravityObjectRigidBody>().GravityDirection.y < 0)
        {
            rotX = 0;
        }

        var newRotX = Quaternion.Euler(new Vector3(rotX, 0, 0));
        var newRotY = Quaternion.Euler(new Vector3(0, angleY, 0));
        characterContainer.transform.rotation = newRotX * newRotY;
    }
}
