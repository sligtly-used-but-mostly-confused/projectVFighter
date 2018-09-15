using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

[RequireComponent(typeof(AIPlayerController))]
public class AIAgent : Agent {
    private Vector3 startingPos;

    Rigidbody2D AIRigidBody;
    GravityObjectRigidBody AIGravityObjectRB;
    AIPlayerController AIController;

    public GravityObjectRigidBody AttachedGravityRigidBody;

    void Start()
    {
        startingPos = transform.position;
        AIRigidBody = GetComponent<Rigidbody2D>();
        AIGravityObjectRB = GetComponent<GravityObjectRigidBody>();
        AIController = GetComponent<AIPlayerController>();
    }
    
    public override void AgentReset()
    {
        if (AIController.IsDead)
        {
            // The Agent fell
            this.transform.position = startingPos;
            this.AIRigidBody.angularVelocity = 0;
            this.AIRigidBody.velocity = Vector3.zero;
            AIController.GetComponent<GravityObjectRigidBody>().ChangeGravityDirection(new Vector2(0,-1));
            this.AIController.IsDead = false;
        }
        else
        {
            // Move the target to a new spot
            /*
            Target.position = new Vector3(Random.value * 8 - 4,
                                          0.5f,
                                          Random.value * 8 - 4);
                                          */
        }
    }

    public override void CollectObservations()
    {
        // Calculate relative position
        Vector3 relativePosition = AttachedGravityRigidBody.transform.position - this.transform.position;

        float h = LevelManager.Instance.height / 2;
        float w = LevelManager.Instance.width / 2;

        // Relative position to grav obj
        AddVectorObs(relativePosition.x / w);
        AddVectorObs(relativePosition.y / h);

        //distances to walls
        AddVectorObs((this.transform.position.x + w) / w);
        AddVectorObs((this.transform.position.x - w) / w);

        AddVectorObs((this.transform.position.y + h) / h);
        AddVectorObs((this.transform.position.y - h) / h);

        //velocity of the the AI
        AddVectorObs(AIRigidBody.velocity.x / AIGravityObjectRB.MaxComponentSpeed);
        AddVectorObs(AIRigidBody.velocity.y / AIGravityObjectRB.MaxComponentSpeed);

        //velocity of gravity object
        AddVectorObs(AttachedGravityRigidBody.GetComponent<Rigidbody2D>().velocity.x / AttachedGravityRigidBody.MaxComponentSpeed);
        AddVectorObs(AttachedGravityRigidBody.GetComponent<Rigidbody2D>().velocity.y / AttachedGravityRigidBody.MaxComponentSpeed);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {

        if (AIController.IsDead)
        {
            AddReward(-1);
            Done();
        }

        Vector3 relativePosition = AttachedGravityRigidBody.transform.position - this.transform.position;
        
        float h = LevelManager.Instance.height;
        float w = LevelManager.Instance.width;
        float relativeDistanceScale = relativePosition.magnitude / new Vector2(w, h).magnitude;

        AddReward(relativeDistanceScale * .1f);
        //Debug.Log(relativePosition.magnitude);

        // Time award
        //AddReward(0.05f);

        // Actions, size = 4
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[1];
        controlSignal.y = vectorAction[2];

        bool isChangeingGrav = vectorAction[0] > 0;
        if(isChangeingGrav)
        {
            AIController.ChangeGravity(controlSignal);
        }
    }

}
