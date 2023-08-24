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
    public float enemyDetectRadius;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public LayerMask enemyMask;
    public LayerMask sightMask;
    private Collider2D[] obstacleArray;
    private Collider2D[] enemyArray;
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
    private Vector2 avoidEnemyDirection;
    private bool enemyPresent;
    private void Start()
    {
        faceTargetScript = body.GetComponent<FaceTarget>();
        bodyRB = body.GetComponent<Rigidbody2D>();
        colliderSize = gameObject.GetComponent<CircleCollider2D>().radius;
        reflect = false;
        
    }
    public override State RunCurrentState()
    {
        Vector2 targetDirection;

        if (!coroutineRunning)
        {
            enemyPresent = false;
            obstacleArray = Physics2D.OverlapCircleAll(body.transform.position, detectRadius, obstacleMask);
            enemyArray = Physics2D.OverlapCircleAll(body.transform.position, enemyDetectRadius, enemyMask);
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
            avoidWeight = new float[16];
            if (obstacleArray != null)
            {

                avoidDirections = CalculateWeight(obstacleArray);
            }
            if(enemyArray != null)
            {
                //avoidEnemyDIrections is a global variable
                avoidEnemyDirections = CalculateEnemyWeight(enemyArray);
            }
           
            interestWeight = new float[16];
            interestDirections = CalculateTargetWeight(targetCachedPosition, interestWeight);

            moveDirection = EnemyDirection(interestDirections, avoidDirections);
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
        Gizmos.DrawWireSphere(body.transform.position, enemyDetectRadius);
        
        if (enemyArray != null)
        {
            Gizmos.color = Color.red;
            foreach (Collider2D obstacle in enemyArray)
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
            
            /*
            for (int i = 0; i < crossDirectionArrayWeights.Length; i++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawRay(gameObject.transform.position, MoreDirections.SixteenDirections[i] * crossDirectionArrayWeights[i] * 5);
                Gizmos.color = Color.red;
                //Gizmos.DrawRay(gameObject.transform.position, MoreDirections.SixteenDirections[i] * avoidEnemyDirections[i] * 5);
            }
            */
        }
        if(avoidEnemyDirection != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(gameObject.transform.position, avoidEnemyDirection);
        }

    }
    private IEnumerator DetectWait()
    {
        coroutineRunning = true;
        yield return new WaitForSeconds(0.02f);
        coroutineRunning = false;
    }

    private float[] CalculateWeight(Collider2D[] array)
    {
        
        float[] avoidArray = new float[16];
        Vector2 position2D = new Vector2(body.transform.position.x, body.transform.position.y);
        
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
            
            for (int i = 0; i < MoreDirections.SixteenDirections.Count; i++)
            {
                
                float result = Vector2.Dot(directionToObjectNormalized, MoreDirections.SixteenDirections[i]);
                float finalWeightValue = result * weight;
                
                
                //replace weight value in i slot if new weight value is higher. Sometimes there are two colliders in the same direction with one being closer.
                if (finalWeightValue > avoidArray[i])
                {

                    avoidArray[i] = finalWeightValue;
                }
                
            }
        }
        return avoidArray;
    }
    private float[] CalculateEnemyWeight(Collider2D[] array)
    {
        
        
        Vector2 position2D = new Vector2(body.transform.position.x, body.transform.position.y);
        float[] returnEnemyArray = new float[16];
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
                weight = (enemyDetectRadius - distanceToObject) / enemyDetectRadius; // less than 1
            }

            Vector2 directionToObjectNormalized = directionToObject.normalized;

            for (int i = 0; i < MoreDirections.SixteenDirections.Count; i++)
            {

                float result = Vector2.Dot(directionToObjectNormalized, MoreDirections.SixteenDirections[i]);
                float finalWeightValue = result * weight;
                if (thingCollider.CompareTag("Enemy"))
                {
                    if (!(thingCollider.gameObject.Equals(body)))
                    {
                        //Debug.Log("ENEMY DECTECTED");
                        enemyPresent = true;
                        if (finalWeightValue > returnEnemyArray[i])
                        {
                            returnEnemyArray[i] = finalWeightValue;
                        }
                    }
                    //Debug.Log(body.name + " " + returnEnemyArray[i]);
                }

            }
        }
        return returnEnemyArray;
    }


    //no collider array needed since game will only have one target: the player
    private float[] CalculateTargetWeight(Vector2 targetCachedPosition, float[] returnArray)
    {
        Vector2 position2D = new Vector2(body.transform.position.x, body.transform.position.y);
        Vector2 directionToObject = targetCachedPosition - position2D;


        Vector2 directionToObjectNormalized = directionToObject.normalized;
        for (int i = 0; i < MoreDirections.SixteenDirections.Count; i++)
        {
            float result = Vector2.Dot(directionToObjectNormalized, MoreDirections.SixteenDirections[i]);
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

    private Vector2 EnemyDirection(float[] interest, float[] avoid)
    {
        
        Vector2 position2D = new Vector2(body.transform.position.x, body.transform.position.y);
        Vector2 directionToObject = targetCachedPosition - position2D;
        Vector2 targetDirection = directionToObject.normalized;
        targetDirection.Normalize();
        
        float[] netInterest = new float[16];
        //16 because there are 16 directions so 16 interest directions and 16 avoid directions
        for (int i = 0; i < 16; i++)
        {
            netInterest[i] = Mathf.Clamp01(interest[i] - avoid[i]);
            //Debug.Log(netInterest[i]);
        }
        //getting highest interest direction
        Vector2 netDirection = Vector2.zero;
        for (int i = 0; i < 16; i++)
        {
            netDirection += MoreDirections.SixteenDirections[i] * netInterest[i];
            
        }
        
        
        Debug.DrawRay(body.transform.position, targetDirection, Color.cyan);
        Vector2 crossNetDirection = Vector3.Cross(netDirection, Vector3.forward); // has magnitude of 1
        crossNetDirection = crossNetDirection.normalized;
        //Debug.Log(netDirection.magnitude);
        if (reflect)
        {
            
            crossNetDirection = ShapingCross(crossNetDirection, targetCachedPosition);
        }
        float[] avoidEnemyResult = new float[16];
        avoidEnemyDirection = Vector2.zero;
        Vector2 netEnemyDirection = Vector2.zero;
        crossDirectionArrayWeights = new float[16];
        float crossAndEnemyDirectionDot = 0;
        float[] crossEnemydotArray = new float[16];
        float[] crossInterestDotArray = new float[16];
        float[] netCrossArray = new float[16];
        Vector2 decidedDirection = Vector2.zero;
        if (enemyPresent)
        {
            
            for (int i = 0; i < MoreDirections.SixteenDirections.Count; i++)
            {
                crossInterestDotArray[i] = Vector2.Dot(crossNetDirection, MoreDirections.SixteenDirections[i]);
                netCrossArray[i] = Mathf.Clamp01(crossInterestDotArray[i] - avoidEnemyDirections[i]);
                decidedDirection += netCrossArray[i] * MoreDirections.SixteenDirections[i];
                Debug.DrawRay(gameObject.transform.position, crossInterestDotArray[i] * MoreDirections.SixteenDirections[i] * 5, Color.cyan);
                Debug.DrawRay(gameObject.transform.position, avoidEnemyDirections[i] * MoreDirections.SixteenDirections[i] * 5, Color.red);
                
            }
            Debug.DrawRay(gameObject.transform.position, decidedDirection, Color.white);
            float avoidEnemyWeight = 0f;
            for (int i = 0; i < MoreDirections.SixteenDirections.Count; i++)
            {
                float avoidEnemyResultTemp = Vector2.Dot(crossNetDirection, MoreDirections.SixteenDirections[i]);
                //Debug.Log(i + " avoid " + avoidEnemyResultTemp + " " + body.name);
                Debug.DrawRay(gameObject.transform.position, netEnemyDirection.normalized, Color.red);
                float dot = Vector2.Dot(netEnemyDirection.normalized, MoreDirections.SixteenDirections[i]);
                
                float dotOfCrossAndNetEnemy = Vector2.Dot(crossNetDirection.normalized, netEnemyDirection.normalized);
                if (dotOfCrossAndNetEnemy >= 0)
                {
                    
                    crossDirectionArrayWeights[i] = Mathf.Clamp01(1f - Mathf.Abs(dot - angleWeight));
                    Debug.Log(i + " cross " + crossDirectionArrayWeights[i] + " " + body.name);
                    //crossDirectionArrayWeights[i] = dot;
                } else
                {
                    Debug.Log(i + " avoidEnemyResultTemp " + avoidEnemyResultTemp + " " + body.name);
                }

                
                if (avoidEnemyWeight < crossDirectionArrayWeights[i])
                {
                    avoidEnemyWeight = crossDirectionArrayWeights[i];
                    
                    avoidEnemyDirection = avoidEnemyWeight * MoreDirections.SixteenDirections[i];
                }
            }
            //Debug.Log("Chosen avoidEnemyWeight: " + avoidEnemyWeight);
            crossAndEnemyDirectionDot = Vector2.Dot(crossNetDirection, avoidEnemyDirection);
            //Debug.Log("crossAndEnemyDirectionDot: " + crossAndEnemyDirectionDot + " " + body.name);
        }
        
        //If any of the 16 directions are greater than the number in the if statement, switch the crossNetDirection
        for (int i = 0; i < MoreDirections.SixteenDirections.Count; i++)
        {
            float avoidResult = Vector2.Dot(crossNetDirection, MoreDirections.SixteenDirections[i] * avoid[i]);
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

            
        }
        
        Vector2 netVector;
        //If the angle between the crossNetDirection and the avoidEnemyDirection is near a right angle or obtuse,
        //ignore avoidEnemyDirection
        //Debug.Log(enemyPresent + " " + body.name);
        
        if(!enemyPresent || crossAndEnemyDirectionDot <= 0)
        {
            Debug.DrawRay(gameObject.transform.position, crossNetDirection, Color.green);
            Debug.DrawRay(gameObject.transform.position, avoidEnemyDirection, Color.cyan * 5);
            //Debug.Log("Not present " + body.name);
            netVector = crossNetDirection;
            return netVector.normalized;
        }
        
        //Debug.Log("Present " + body.name);
        Debug.DrawRay(gameObject.transform.position, crossNetDirection, Color.green);
        Debug.DrawRay(gameObject.transform.position, avoidEnemyDirection, Color.cyan * 5);
        netVector =avoidEnemyDirection;
        return netVector.normalized;
        
        
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
