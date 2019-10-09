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
    
    public Vector3 Seek()
    {
        Vector3 linear_acc = target.position - agent.position;
        linear_acc.Normalize();
        linear_acc *= maxAcceleration;
        return linear_acc;
    }

    public Vector3 Flee()
    {
        Vector3 linear_acc = agent.position - target.position;
        linear_acc.Normalize();
        linear_acc *= maxAcceleration;
        return linear_acc;
    }

    public Vector3 Evade()
    {
        float dist = (target.position - agent.position).magnitude;
        float speed = agent.velocity.magnitude;

        float prediction = 0f;
        if(speed <= dist / maxPrediction)
        {
            prediction = maxPrediction;
        }
        else
        {
            prediction = dist / speed;
        }
        Vector3 linear_acc = agent.position - (target.position + target.velocity * prediction);
        linear_acc.Normalize();
        linear_acc *= maxAcceleration;
        return linear_acc;
    }

    public Vector3 Pursue()
    {
        float dist = (target.position - agent.position).magnitude;
        float speed = agent.velocity.magnitude;

        float prediction = 0f;
        if (speed <= dist / maxPrediction)
        {
            prediction = maxPrediction;
        }
        else
        {
            prediction = dist / speed;
        }
        Vector3 linear_acc = (target.position + target.velocity * prediction) - agent.position;
        linear_acc.Normalize();
        linear_acc *= maxAcceleration;
        return linear_acc;
    }

    public Vector3 Arrive()
    {
        Vector3 direct = target.position - agent.position;
        float dist = direct.magnitude;
        float speed = agent.velocity.magnitude;
        if (dist < targetRadiusL)
        {
            return new Vector3(0, 0, 0);
        }
        else if (dist > slowRadiusL)
        {
            speed = maxSpeed;
        }
        else
        {
            speed = maxSpeed * dist / slowRadiusL;
        }
        Vector3 targetVelocity = direct;
        targetVelocity.Normalize();
        targetVelocity *= speed;
        Vector3 linear_acc = (targetVelocity - agent.velocity) / timeToTarget;

        if (linear_acc.magnitude > maxAcceleration)
        {
            linear_acc.Normalize();
            linear_acc *= maxAcceleration;
        }
        return linear_acc;
    }

    public float Align()
    {
        if (agent.velocity.magnitude == 0)
        {
            return 0;
        }
        float targetRotation;

        float x = agent.velocity.x;
        float y = agent.velocity.z;
        float orient = Mathf.Atan2(x, y);
        float turn = 2 * Mathf.PI;
        orient -= agent.orientation;
        if (orient > Mathf.PI)
        {
            while (orient > Mathf.PI)
            {
                orient -= turn;
            }
        }
        if (orient < -Mathf.PI)
        {
            while (orient < -Mathf.PI)
            {
                orient += turn;
            }
        }
        float absoluteOrient = Mathf.Abs(orient);
        if (absoluteOrient < (targetRadiusA))
        {
            agent.rotation = 0;
        }
        if (absoluteOrient > (slowRadiusA))
        {
            targetRotation = maxRotation;
        }
        else
        {
            targetRotation = maxRotation * absoluteOrient / slowRadiusA;
        }
        targetRotation *= orient / absoluteOrient;
        float angular = targetRotation - agent.rotation;
        angular /= timeToTarget;
        //Debug.Log("Rot: " + angular);
        float angularAcceleration = Mathf.Abs(angular);
        //Debug.Log("Agent " + agent.orientation);
        return angular;
    }

    public float Face()
    {
        float targetRotation;

        Vector3 direction = target.position - agent.position;
        if (direction.magnitude == 0)
        {
            return 0;
        }
        float orient = Mathf.Atan2(direction.x, direction.z);
        float turn = 2 * Mathf.PI;
        orient -= agent.orientation;
        if (orient > Mathf.PI)
        {
            while (orient > Mathf.PI)
            {
                orient -= turn;
            }
        }
        if (orient < -Mathf.PI)
        {
            while (orient < -Mathf.PI)
            {
                orient += turn;
            }
        }
        float absoluteOrient = Mathf.Abs(orient);
        if (absoluteOrient < (targetRadiusA / 2))
        {
            agent.rotation = 0;
        }
        if (absoluteOrient > (slowRadiusA))
        {
            targetRotation = maxRotation;
        }
        else
        {
            targetRotation = maxRotation * absoluteOrient / slowRadiusA;
        }


        targetRotation *= orient / absoluteOrient;
        float angular = targetRotation - agent.rotation;
        angular /= timeToTarget;
        //Debug.Log("Rot: " + angular);
        float angularAcceleration = Mathf.Abs(angular);
        if (angularAcceleration > maxAngularAcceleration)
        {
            angular /= angularAcceleration;
            angular *= maxAngularAcceleration;
        }
        //Debug.Log("Agent " + agent.orientation);
        return angular;
    }

    public float Wander(out Vector3 linear)
    {
        //float angle = Random.Range(0, 359) * wanderRate;
        //Vector3 
        //float x = Mathf.Sin(Mathf.Deg2Rad * agent.orientation) * wanderRadius;
        //float z = Mathf.Cos(Mathf.Deg2Rad * agent.orientation) * wanderRadius;





        //wanderOrientation += Random.Range(0, 359) * wanderRate;
        wanderOrientation += (Random.value - Random.value) * wanderRate;
        Vector3 target = agent.position + wanderOffset * new Vector3(Mathf.Sin(agent.orientation), 0, Mathf.Cos(agent.orientation));
        float orientation = wanderOrientation + agent.orientation;
        agent.DrawCircle(target, wanderRadius);
        target += wanderRadius * new Vector3(Mathf.Sin(orientation), 0, Mathf.Cos(orientation));
        float targetRotation;

        Vector3 direction = target - agent.position;
        if (direction.magnitude == 0)
        {
            linear = Vector3.zero;
            return 0;
        }
        float orient = Mathf.Atan2(direction.x, direction.z);
        float turn = 2 * Mathf.PI;
        orient -= agent.orientation;
        if (orient > Mathf.PI)
        {
            while (orient > Mathf.PI)
            {
                orient -= turn;
            }
        }
        if (orient < -Mathf.PI)
        {
            while (orient < -Mathf.PI)
            {
                orient += turn;
            }
        }
        float absoluteOrient = Mathf.Abs(orient);
        if (absoluteOrient < (targetRadiusA / 2))
        {
            agent.rotation = 0;
        }
        if (absoluteOrient > (slowRadiusA))
        {
            targetRotation = maxRotation;
        }
        else
        {
            targetRotation = maxRotation * absoluteOrient / slowRadiusA;
        }
        targetRotation *= orient / absoluteOrient;
        float angular = targetRotation - agent.rotation;
        angular /= timeToTarget;
        //Debug.Log("Rot: " + angular);
        float angularAcceleration = Mathf.Abs(angular);
        if (angularAcceleration > maxAngularAcceleration)
        {
            angular /= angularAcceleration;
            angular *= maxAngularAcceleration;
        }

        linear = maxAcceleration * new Vector3(Mathf.Sin(agent.orientation), 0, Mathf.Cos(agent.orientation));
        //Debug.Log("Agent " + agent.orientation);
        return angular;


        //Vector3 towards = new Vector3(x, 0f, z);

        /*float centerX = Mathf.Sin(Mathf.Deg2Rad * agent.orientation) * wanderOffset;
        float centerZ = Mathf.Cos(Mathf.Deg2Rad * agent.orientation) * wanderOffset;
        Debug.Log("Circle: " + centerX + " " + centerZ);
        Debug.Log("Agent orientation " + agent.orientation);
        Vector3 centerCircle = new Vector3 (centerX, 0f, centerZ);
        Vector3 newPos = centerCircle + agent.position;
        //Debug.Log("New position of circle" + newPos);
        //centerCircle += agent.position;
        //Debug.Log("Position of circle: " + centerCircle);
        //towards += agent.position;
        //Debug.Log("Towards: " + towards);
        //towards += centerCircle;
        //Debug.Log("Towards: " + towards);
        towards *= maxAcceleration;

        return towards;*/
    }
}
