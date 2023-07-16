using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleState : State
{
    public State nextState;
    private FaceTarget faceTargetScript;
    public GameObject body;
    private Rigidbody2D bodyRB;
    public GameObject target;
    public float detectRadius;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public LayerMask sightMask;
    private Collider2D[] obstacleArray;
    private Collider2D targetCollider;
    private bool coroutineRunning;
    private float colliderSize;
    private float[] avoidWeight;
    private float[] avoidDirections;
    private float[] interestWeight;
    private float[] interestDirections;
    private Vector2 targetCachedPosition;
    private Vector2 moveDirection;
    public float speed;
    private bool reflect;
    private void Start()
    {
        faceTargetScript = body.GetComponent<FaceTarget>();
        bodyRB = body.GetComponent<Rigidbody2D>();
        avoidWeight = new float[8];
        interestWeight = new float[8];
        colliderSize = gameObject.GetComponent<CircleCollider2D>().radius;
        reflect = false;
    }
    public override State RunCurrentState()
    {
        Vector2 targetDirection;

        if (!coroutineRunning)
        {
            obstacleArray = Physics2D.OverlapCircleAll(body.transform.position, detectRadius, obstacleMask);
            targetCollider = Physics2D.OverlapCircle(body.transform.position, detectRadius, targetMask);
            if (targetCollider != null)
            {
                targetDirection = (targetCollider.transform.position - body.transform.position);
                RaycastHit2D hit = Physics2D.Raycast(body.transform.position, targetDirection.normalized, detectRadius, sightMask);
                int targetLayerNum = LayerMask.NameToLayer("Target");
                if (hit.collider != null && hit.collider.gameObject.layer.Equals(targetLayerNum))
                {

                    Debug.DrawRay(body.transform.position, targetDirection.normalized * detectRadius, Color.magenta);
                    targetCachedPosition = targetCollider.ClosestPoint(body.transform.position);
                    
                }
                else
                {
                    
                }
            }
            avoidWeight = new float[8];
            if (obstacleArray != null)
            {

                avoidDirections = CalculateWeight(obstacleArray, avoidWeight);
            }
            interestWeight = new float[8];
            interestDirections = CalculateTargetWeight(targetCachedPosition, interestWeight);

            moveDirection = EnemyDirection(interestDirections, avoidDirections);
            StartCoroutine("DetectWait");
        }
        bodyRB.velocity = moveDirection * speed * Time.deltaTime;

        return this;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(body.transform.position, detectRadius);
        
        if (obstacleArray != null)
        {
            Gizmos.color = Color.red;
            foreach (Collider2D obstacle in obstacleArray)
            {
                Gizmos.DrawSphere(obstacle.transform.position, 0.5f);
            }
        }
        
        if (targetCachedPosition != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(targetCachedPosition, 0.5f);

        }

        if (avoidDirections != null)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < avoidDirections.Length; i++)
            {
                Gizmos.DrawRay(gameObject.transform.position, Directions.EightDirections[i] * avoidDirections[i]);
            }
        }
        if (interestDirections != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < interestDirections.Length; i++)
            {
                Gizmos.DrawRay(gameObject.transform.position, Directions.EightDirections[i] * interestDirections[i]);
            }
        }
        if (interestDirections != null && avoidDirections != null)
        {
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(gameObject.transform.position, moveDirection);
        }


    }
    private IEnumerator DetectWait()
    {
        coroutineRunning = true;
        yield return new WaitForSeconds(0.05f);
        coroutineRunning = false;
    }

    private float[] CalculateWeight(Collider2D[] array, float[] returnArray)
    {
        Vector2 position2D = new Vector2(body.transform.position.x, body.transform.position.y);
        foreach (Collider2D thingCollider in array)
        {
            Vector2 directionToObject = thingCollider.ClosestPoint(body.transform.position) - position2D;
            float distanceToObject = directionToObject.magnitude;
            float weight;

            if (distanceToObject <= colliderSize) // prevents being too close to obstacle
            {
                
                weight = 1;
            }
            else
            {
                weight = (detectRadius - distanceToObject) / detectRadius; // less than 1
            }


            Vector2 directionToObjectNormalized = directionToObject.normalized;
            for (int i = 0; i < Directions.EightDirections.Count; i++)
            {
                float result = Vector2.Dot(directionToObjectNormalized, Directions.EightDirections[i]);
                float finalWeightValue = result * weight;
                
                //replace weight value in i slot if new weight value is higher. Sometimes there are two colliders in the same direction with one being closer.
                if (finalWeightValue > returnArray[i])
                {
                    returnArray[i] = finalWeightValue;
                }
            }
        }
        return returnArray;
    }


    //no collider array needed since game will only have one target: the player
    private float[] CalculateTargetWeight(Vector2 targetCachedPosition, float[] returnArray)
    {
        Vector2 position2D = new Vector2(body.transform.position.x, body.transform.position.y);
        Vector2 directionToObject = targetCachedPosition - position2D;


        Vector2 directionToObjectNormalized = directionToObject.normalized;
        for (int i = 0; i < Directions.EightDirections.Count; i++)
        {
            float result = Vector2.Dot(directionToObjectNormalized, Directions.EightDirections[i]);
            // 1 to excluding 0 is mapped
            if (result > 0)
            {
                
                returnArray[i] = result;
                
            }

        }
        return returnArray;
    }

    private Vector2 EnemyDirection(float[] interest, float[] avoid)
    {
        
        float[] netInterest = new float[8];
        //8 because there are 8 directions so 8 interest directions and 8 avoid directions
        for (int i = 0; i < 8; i++)
        {
            netInterest[i] = Mathf.Clamp01(interest[i] - avoid[i]);
            //Debug.Log(netInterest[i]);
        }
        //getting highest interest direction
        Vector2 netDirection = Vector2.zero;
        for (int i = 0; i < 8; i++)
        {
            netDirection += Directions.EightDirections[i] * netInterest[i];
        }
        
        netDirection.Normalize();
        Debug.DrawRay(body.transform.position, netDirection, Color.cyan);
        Vector2 crossNetDirection = Vector3.Cross(netDirection, Vector3.forward);
        //Debug.Log(netDirection.magnitude);
        if (reflect)
        {
            Debug.Log("reflected");
            crossNetDirection = ShapingCross(crossNetDirection, targetCachedPosition);
        }
        for (int i = 0; i < Directions.EightDirections.Count; i++)
        {
            float result = Vector2.Dot(crossNetDirection, Directions.EightDirections[i] * avoid[i]);
            if (result >= 0.9)
            {
                
                if (!reflect)
                {
                    reflect = true;
                } else
                {
                    reflect = false;
                }
            }
        }
        
        return crossNetDirection;
    }
    //reflects the cross product calculations along the target vector line
    private Vector2 ShapingCross(Vector2 cross, Vector2 targetCachedPosition)
    {
        Vector2 newCross = cross;
        Vector2 position2D = new Vector2(body.transform.position.x, body.transform.position.y);
        Vector2 targetVector = targetCachedPosition - position2D;
        Vector2 normalTargetVector = Quaternion.AngleAxis(90, Vector3.forward) * targetVector;
        newCross = Vector2.Reflect(cross, normalTargetVector);
        newCross = newCross.normalized;

        return newCross;
    }


    public static class Directions
    {
        public static List<Vector2> EightDirections = new List<Vector2>
        {
            new Vector2(1,0).normalized,
            new Vector2(1,1).normalized,
            new Vector2(0,1).normalized,
            new Vector2(-1,1).normalized,
            new Vector2(-1,0).normalized,
            new Vector2(-1,-1).normalized,
            new Vector2(0,-1).normalized,
            new Vector2(1,-1).normalized,

        };
    }
}
