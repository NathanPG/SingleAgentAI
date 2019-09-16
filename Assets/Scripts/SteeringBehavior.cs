using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the place to put all of the various steering behavior methods we're going
/// to be using. Probably best to put them all here, not in NPCController.
/// </summary>

public class SteeringBehavior : MonoBehaviour {

    // The agent at hand here, and whatever target it is dealing with
    public NPCController agent;
    public NPCController target;

    // Below are a bunch of variable declarations that will be used for the next few
    // assignments. Only a few of them are needed for the first assignment.

    // For pursue and evade functions
    public float maxPrediction;
    public float maxAcceleration;

    // For arrive function
    public float maxSpeed;
    public float targetRadiusL;
    public float slowRadiusL;
    public float timeToTarget;

    // For Face function
    public float maxRotation;
    public float maxAngularAcceleration;
    public float targetRadiusA;
    public float slowRadiusA;

    // For wander function
    public float wanderOffset;
    public float wanderRadius;
    public float wanderRate;
    private float wanderOrientation;

    // Holds the path to follow
    public GameObject[] Path;
    public int current = 0;

    protected void Start() {
        agent = GetComponent<NPCController>();
        //wanderOrientation = agent.orientation;
    }
    public Vector3 seek()
    {
        Vector3 linear_acc = target.position - agent.position;
        linear_acc.Normalize();
        linear_acc *= maxAcceleration;
        return linear_acc;
    }

    public Vector3 flee()
    {
        Vector3 linear_acc = agent.position - target.position;
        linear_acc.Normalize();
        linear_acc *= maxAcceleration;
        return linear_acc;
    }

    public float Align()
    {
        float x = agent.velocity.x;
        float y = agent.velocity.z;
        float orient = Mathf.Atan2(x, y);
        //Vector3 turn = new Vector3(0, 0, agent.orientation);
        if (orient > 180)
        {
            return (agent.orientation / -orient) ;
        }
        else
        {
            return (agent.orientation / orient );
        }
        //float a = Vector3.Angle(agent.velocity, turn);
        //return orient * agent.orientation;
    }

    public float Face()
    {
        Vector3 direction = agent.position - target.position;
        float orient = Mathf.Atan2(direction.x, direction.z);
        if (orient > 180)
        {
            //return ((-orient*maxAngularAcceleration) / agent.orientation);
            return (maxAngularAcceleration * -orient);
        }
        else
        {
            //return ((orient * maxAngularAcceleration) / agent.orientation) * maxAngularAcceleration;
            return (maxAngularAcceleration * (orient));
        }
        //float o = Vector3.Angle(agent.orientation, direction)


        //return orient / agent.orientation;
    }

    public Vector3 Wander()
    {
        wanderOrientation += Random.Range(0, 359) * wanderOffset;
        float targetOrientation = wanderOrientation + agent.rotation ;
        Vector3 targetPos = agent.position + wanderOffset * new Vector3(0f, agent.orientation, 0f);


        return new Vector3();
    }
}
