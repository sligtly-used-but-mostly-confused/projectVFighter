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

    public PlayerController OtherPlayer;
    public GravityObjectRigidBody OtherPlayerGORB;

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
        var distanceToAttachedObject = GetRelativePosition(AttachedGravityRigidBody.transform);
        AddVectorObs(distanceToAttachedObject.x);
        AddVectorObs(distanceToAttachedObject.y);

        var distanceToOtherPlayer = GetRelativePosition(OtherPlayer.transform);
        AddVectorObs(distanceToOtherPlayer.x);
        AddVectorObs(distanceToOtherPlayer.y);

        var distanceToOtherPlayerGORB = GetRelativePosition(OtherPlayerGORB.transform);
        AddVectorObs(distanceToOtherPlayerGORB.x);
        AddVectorObs(distanceToOtherPlayerGORB.y);

        float h = LevelManager.Instance.height / 2;
        float w = LevelManager.Instance.width / 2;

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

        //velocity of the the other AI
        AddVectorObs(OtherPlayer.GetComponent<Rigidbody2D>().velocity.x / OtherPlayer.GetComponent<GravityObjectRigidBody>().MaxComponentSpeed);
        AddVectorObs(OtherPlayer.GetComponent<Rigidbody2D>().velocity.y / OtherPlayer.GetComponent<GravityObjectRigidBody>().MaxComponentSpeed);

        //velocity of other gravity object
        AddVectorObs(OtherPlayerGORB.GetComponent<Rigidbody2D>().velocity.x / OtherPlayerGORB.MaxComponentSpeed);
        AddVectorObs(OtherPlayerGORB.GetComponent<Rigidbody2D>().velocity.y / OtherPlayerGORB.MaxComponentSpeed);
    }

    private Vector3 GetRelativePosition(Transform other)
    {
        Vector3 relativePosition = other.position - this.transform.position;

        float h = LevelManager.Instance.height / 2;
        float w = LevelManager.Instance.width / 2;
        relativePosition.Scale(new Vector3(1 / w, 1 / h));

        return relativePosition;
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {

        if (AIController.IsDead)
        {
            AddReward(-1);
            OtherPlayer.GetComponent<AIAgent>()?.AddReward(1);
            Done();
        }

        if(OtherPlayer.IsDead)
        {
            AddReward(1);
            OtherPlayer.GetComponent<AIAgent>()?.AddReward(-1);
            Done();
        }

        
        float h = LevelManager.Instance.height;
        float w = LevelManager.Instance.width;
        /*
        Vector3 relativePositionToAttachedGORB = AttachedGravityRigidBody.transform.position - this.transform.position;
        float relativeDistanceToAttachedGORBScale = relativePositionToAttachedGORB.magnitude / new Vector2(w, h).magnitude;
        AddReward(relativeDistanceToAttachedGORBScale * .01f);
        */
        Vector3 relativePositionFromOtherPlayerToAttachedGORB = OtherPlayer.transform.position - AttachedGravityRigidBody.transform.position;
        float relativeDistanceFromOtherPlayerToAttachedGORBScale = 1 - (relativePositionFromOtherPlayerToAttachedGORB.magnitude / new Vector2(w, h).magnitude);
        AddReward(relativeDistanceFromOtherPlayerToAttachedGORBScale * .5f);
        //Debug.Log(relativeDistanceFromOtherPlayerToAttachedGORBScale * .1f);

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
