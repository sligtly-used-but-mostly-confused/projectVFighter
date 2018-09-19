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

    public PlayerController OtherPlayer;

    public GravityObjectRigidBody GORB1;
    public GravityObjectRigidBody GORB2;

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
            this.transform.position = startingPos;
            this.AIRigidBody.angularVelocity = 0;
            this.AIRigidBody.velocity = Vector3.zero;
            AIController.GetComponent<GravityObjectRigidBody>().ChangeGravityDirection(new Vector2(0,-1));
            this.AIController.IsDead = false;
            float h = LevelManager.Instance.height;
            float w = LevelManager.Instance.width;
            GORB1.transform.localPosition = new Vector2(Random.value*w,Random.value*h) - new Vector2(w/2, h/2);
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
        var distanceToGORB1 = GetRelativePosition(GORB1.transform, transform);
        AddVectorObs(distanceToGORB1.x);
        AddVectorObs(distanceToGORB1.y);

        var distanceToGORB2 = GetRelativePosition(GORB2.transform, transform);
        AddVectorObs(distanceToGORB2.x);
        AddVectorObs(distanceToGORB2.y);

        var distanceToOtherPlayer = GetRelativePosition(OtherPlayer.transform, transform);
        AddVectorObs(distanceToOtherPlayer.x);
        AddVectorObs(distanceToOtherPlayer.y);

        /*
        var distanceFromAttachedRBtoOtherPlayer = GetRelativePosition(OtherPlayer.transform, GORB1.transform);
        AddVectorObs(distanceFromAttachedRBtoOtherPlayer.x);
        AddVectorObs(distanceFromAttachedRBtoOtherPlayer.y);
        */
        
        float h = LevelManager.Instance.height / 2;
        float w = LevelManager.Instance.width / 2;

        //distances to walls
        AddVectorObs((this.transform.localPosition.x + w) / w);
        AddVectorObs((this.transform.localPosition.x - w) / w);

        AddVectorObs((this.transform.localPosition.y + h) / h);
        AddVectorObs((this.transform.localPosition.y - h) / h);

        //velocity of the the AI
        AddVectorObs(AIRigidBody.velocity.x / AIGravityObjectRB.MaxComponentSpeed);
        AddVectorObs(AIRigidBody.velocity.y / AIGravityObjectRB.MaxComponentSpeed);

        //velocity of the the other AI
        AddVectorObs(OtherPlayer.GetComponent<Rigidbody2D>().velocity.x / OtherPlayer.GetComponent<GravityObjectRigidBody>().MaxComponentSpeed);
        AddVectorObs(OtherPlayer.GetComponent<Rigidbody2D>().velocity.y / OtherPlayer.GetComponent<GravityObjectRigidBody>().MaxComponentSpeed);

        //velocity of gravity object
        AddVectorObs(GORB1.GetComponent<Rigidbody2D>().velocity.x / GORB1.MaxComponentSpeed);
        AddVectorObs(GORB1.GetComponent<Rigidbody2D>().velocity.y / GORB1.MaxComponentSpeed);

        //velocity of other gravity object
        AddVectorObs(GORB2.GetComponent<Rigidbody2D>().velocity.x / GORB2.MaxComponentSpeed);
        AddVectorObs(GORB2.GetComponent<Rigidbody2D>().velocity.y / GORB2.MaxComponentSpeed);
    }

    private Vector3 GetRelativePosition(Transform to, Transform from)
    {
        Vector3 relativePosition = to.localPosition - from.localPosition;

        float h = LevelManager.Instance.height / 2;
        float w = LevelManager.Instance.width / 2;
        relativePosition.Scale(new Vector3(1 / w, 1 / h));

        return relativePosition;
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {

        if(AIController.IsDead && OtherPlayer.IsDead)
        {
            AddReward(-1);
            OtherPlayer.GetComponent<AIAgent>()?.AddReward(-1);
            Done();
        }

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
        
        Vector3 relativePositionFromOtherPlayerToAttachedGORB = OtherPlayer.transform.localPosition - GORB1.transform.localPosition;
        float relativeDistanceFromOtherPlayerToAttachedGORBScale = 1f - (relativePositionFromOtherPlayerToAttachedGORB.magnitude / new Vector2(w, h).magnitude);
        AddReward(relativeDistanceFromOtherPlayerToAttachedGORBScale * .02f);
        */
        //Debug.Log(relativeDistanceFromOtherPlayerToAttachedGORBScale * .1f);

        //Debug.Log(relativePosition.magnitude);

        AddReward(-0.01f);

        // Actions, size = 4
        Vector3 aimSignal = Vector3.zero;
        aimSignal.x = vectorAction[1];
        aimSignal.y = vectorAction[2];
        AIController.AimReticle(aimSignal);

        bool isShooting = vectorAction[0] > 0;
        if(isShooting)
        {
            
            AIController.ShootGravityGun(aimSignal);
        }
        
        float moveSignal = 0;
        moveSignal = vectorAction[4];

        AIController.Move(moveSignal);
        
        bool isChangingGrav = vectorAction[3] > 0;
        if (isChangingGrav)
        {
            AIController.FlipGravity();
        }


    }

}
