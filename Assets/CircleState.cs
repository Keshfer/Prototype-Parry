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
    private float[] avoidEnemyDirections;
    private float[] interestWeight;
    private float[] interestDirections;
    private float[] crossDirectionArrayWeights;
    private Vector2 targetCachedPosition;
    private Vector2 moveDirection;
    public float speed;
    private bool reflect;
    public bool move;
    public float angleWeight;
    private void Start()
    {
        faceTargetScript = body.GetComponent<FaceTarget>();
        bodyRB = body.GetComponent<Rigidbody2D>();
        avoidWeight = new float[8];
        interestWeight = new float[8];
        colliderSize = gameObject.GetComponent<CircleCollider2D>().radius;
        reflect = false;
        crossDirectionArrayWeights = new float[8];
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

                (avoidDirections, avoidEnemyDirections) = CalculateWeight(obstacleArray);
            }
            interestWeight = new float[8];
            interestDirections = CalculateTargetWeight(targetCachedPosition, interestWeight);

            moveDirection = EnemyDirection(interestDirections, avoidDirections, avoidEnemyDirections);
            StartCoroutine("DetectWait");
        }
        if (move)
        {
            bodyRB.velocity = moveDirection * speed * Time.deltaTime;
        }
        return this;
    }

    private void OnDrawGizmos()
    {
        /*
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
        */
        /*
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
        */
        if (interestDirections != null && avoidDirections != null)
        {
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(gameObject.transform.position, moveDirection);
        }
        
        if (avoidEnemyDirections != null)
        {
            
            Gizmos.color = Color.white;
            for (int i = 0; i < crossDirectionArrayWeights.Length; i++)
            {
                Gizmos.DrawRay(gameObject.transform.position, Directions.EightDirections[i] * crossDirectionArrayWeights[i] * 5);
            }
        }

    }
    private IEnumerator DetectWait()
    {
        coroutineRunning = true;
        yield return new WaitForSeconds(0.02f);
        coroutineRunning = false;
    }

    private (float[], float[]) CalculateWeight(Collider2D[] array)
    {
        float[] avoidArray = new float[8];
        Vector2 position2D = new Vector2(body.transform.position.x, body.transform.position.y);
        float[] returnEnemyArray = new float[8];
        foreach (Collider2D thingCollider in array)
        {
            Vector2 directionToObject = thingCollider.ClosestPoint(body.transform.position) - position2D;
            float distanceToObject = directionToObject.magnitude;
            float weight;
            if (distanceToObject <= colliderSize) // prevents being too close to obstacle
            {

                weight = 1;
                //Debug.Log("touching");
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
                if (thingCollider.CompareTag("Enemy"))
                {
                    if (finalWeightValue > returnEnemyArray[i])
                    {
                        returnEnemyArray[i] = finalWeightValue;
                    }
                    //Debug.Log(body.name + " " + returnEnemyArray[i]);
                }
                else
                {
                    //replace weight value in i slot if new weight value is higher. Sometimes there are two colliders in the same direction with one being closer.
                    if (finalWeightValue > avoidArray[i])
                    {
                        avoidArray[i] = finalWeightValue;
                    }
                }
            }
        }
        return (avoidArray, returnEnemyArray);
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
    // I want the fly to
    //1)change directions to avoid obstacles (done)
    //2)manuever around fellow enemies at an angle
    //3)angle towards the targte if target is a little far away
    //4) return to follow state if target too far away

    // ****** ANGLE THE NET DIRECTION BY REDUCING OR INCREASING THE AVOIDENEMYDIRECTION'S MAGNITUDE **********
    
    private Vector2 EnemyDirection(float[] interest, float[] avoid, float[] avoidEnemy)
    {
        float temp = 0;
        Vector2 position2D = new Vector2(body.transform.position.x, body.transform.position.y);
        Vector2 directionToObject = targetCachedPosition - position2D;
        Vector2 targetDirection = directionToObject.normalized;
        targetDirection.Normalize();
        
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
        

        Debug.DrawRay(body.transform.position, targetDirection, Color.cyan);
        Vector2 crossNetDirection = Vector3.Cross(targetDirection, Vector3.forward); // has magnitude of 1
        //Debug.Log(netDirection.magnitude);
        if (reflect)
        {
            
            crossNetDirection = ShapingCross(crossNetDirection, targetCachedPosition);
        }
        float[] avoidEnemyResult = new float[8];
        Vector2 avoidEnemyDirection = Vector2.zero;
        float avoidEnemyWeight = 0f;
        crossDirectionArrayWeights = new float[8];
        //If any of the 8 directions are greater than the number in the if statement, switch the crossNetDirection
        for (int i = 0; i < Directions.EightDirections.Count; i++)
        {
            float avoidResult = Vector2.Dot(crossNetDirection, Directions.EightDirections[i] * avoid[i]);
            if (avoidResult >= 0.9)
            {
                
                if (!reflect)
                {
                    reflect = true;
                } else
                {
                    reflect = false;
                }
            }
            
            
            float avoidEnemyResultTemp = Vector2.Dot(crossNetDirection, Directions.EightDirections[i] * avoidEnemy[i]);
            //if statement only includes directions that in the same semi circle as the crossNetDirection and the direction of avoidEnemy
            // put in avoidEnemyResult array if the dot product is greater than 0.2 (0.2 instead of 0 so that directions REALLy close to 0 isn't included)
            if (avoidEnemyResultTemp > 0)
            {

                //how do I teach the computer to shape the weights by itself?
                crossDirectionArrayWeights[i] = 1f - Mathf.Abs(avoidEnemy[i] - 0.65f);
                
                
                
                
            }
            Debug.Log("angleWeight: " + angleWeight);
            
            if (avoidEnemyWeight < crossDirectionArrayWeights[i])
            {
                avoidEnemyWeight = crossDirectionArrayWeights[i];
                avoidEnemyDirection = avoidEnemyWeight * Directions.EightDirections[i];
                
                temp = avoidEnemyWeight;
            } 
        }
        //Debug.Log("Best avoid EnemyDirection " + temp + " " + body.name);
        Debug.DrawRay(gameObject.transform.position, crossNetDirection, Color.green);
        Debug.DrawRay(gameObject.transform.position, avoidEnemyDirection, Color.cyan);
        return (crossNetDirection + avoidEnemyDirection).normalized;
        
        
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
    public static class MoreDirections
    {
        public static List<Vector2> SixteenDirections = new List<Vector2>
        {
            new Vector2(1,0).normalized,
            new Vector2(1, 0.5f).normalized,
            new Vector2(1,1).normalized,
            new Vector2(0.5f,1).normalized,
            new Vector2(0,1).normalized,
            new Vector2(-0.5f,1).normalized,
            new Vector2(-1,1).normalized,
            new Vector2(-1,0.5f).normalized,
            new Vector2(-1,0).normalized,
            new Vector2(-1,-0.5f).normalized,
            new Vector2(-1,-1).normalized,
            new Vector2(-0.5f,-1).normalized,
            new Vector2(0,-1).normalized,
            new Vector2(0.5f, -1).normalized,
            new Vector2(1,-1).normalized,
            new Vector2(1, -0.5f).normalized,

        };
    }
}
