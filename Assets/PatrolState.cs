using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    public GameObject patrolA;
    public GameObject patrolB;
    private Collider2D colliderA;
    private Collider2D colliderB;

    
    public GameObject body;
    private Rigidbody2D rb;
    private bool atA;
    public float speed;
    private bool directionDecided;
    private bool pause;
    private bool coroutineRunning;
    private bool FOVCheckRunning;
    public float radius;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    [Range(0,360)]
    public float angle;
    public bool playerDetected;
    public State nextState;
    public GameObject player;
    
    
    private void Start()
    {
        atA = false;
        rb = body.GetComponent<Rigidbody2D>();
        colliderA = patrolA.GetComponent<Collider2D>();
        colliderB = patrolB.GetComponent<Collider2D>();
        directionDecided = false;
        coroutineRunning = false;
        pause = false;
        FOVCheckRunning = false;
        playerDetected = false;
        StartCoroutine("Wait");
    }
    public override State RunCurrentState()
    {
        if(!FOVCheckRunning)
        {
            StartCoroutine("FOVRoutine");
        }
        if(atA)
        {
            ApproachPoint(patrolB);
        } else // at Point B
        {
            
            ApproachPoint(patrolA);
        }
        if (playerDetected)
        {
            return nextState;
            
            
        }
        else
        {
            return this;
        }
    }

    private void faceMovement(int sign)
    {
        if(sign < 0)
        {
            body.transform.eulerAngles = new Vector3(0, 180, 0);
        } else
        {
            body.transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }

    private void ApproachPoint(GameObject point)
    {
        float directionalDist = point.transform.position.x - body.transform.position.x;
        //print(directionalDist);
        if (!coroutineRunning)
        {
            if (!directionDecided)
            {
                if (directionalDist < 0) //body is ahead of point in world space
                {
                    faceMovement(-1);
                    directionDecided = true;
                    

                }
                else //body is behind point in world space
                {
                    faceMovement(1);
                    directionDecided = true;
                    

                }
            }
            if (!pause)
            {
                velocity = body.transform.right * speed * Time.deltaTime;
                //rb.velocity = transform.right * speed * Time.deltaTime;
            } else
            {
                velocity = new Vector2(0, 0);
            }
        }
    }
    private void SwitchPoints()
    {
        if(atA)
        {
            atA = false;
            
        } else
        {
            atA = true;
            
        }
    }
    private IEnumerator SwitchWait()
    {
        pause = true;
        velocity = new Vector2(0, 0);
        //rb.velocity = new Vector2(0,0);
        yield return new WaitForSeconds(2);
        //print("switch");
        SwitchPoints();
        directionDecided = false;
        pause = false;
        coroutineRunning = false;
    }
    private IEnumerator Wait()
    {
        coroutineRunning = true;
        yield return new WaitForSeconds(0.2f);
        coroutineRunning = false;
    }
    private IEnumerator FOVRoutine()
    {
        FOVCheckRunning = true;
        yield return new WaitForSeconds(0.2f);
        FOVCheck();
        FOVCheckRunning = false;
    }

    private void FOVCheck()
    {
        Collider2D[] colliderArray = Physics2D.OverlapCircleAll(body.transform.position, radius, targetMask);

        if(colliderArray.Length != 0)
        {
            GameObject target = colliderArray[0].gameObject;
            Vector2 directionToTarget = (target.transform.position - body.transform.position).normalized;

            if(Vector2.Angle(gameObject.transform.right, directionToTarget) < (angle / 2))
            {
                float distanceToTarget = Vector2.Distance(body.transform.position, target.transform.position);

                if(!(Physics2D.Raycast(body.transform.position, directionToTarget, radius, obstructionMask)))
                {
                    //print("See player");
                    playerDetected = true;
                    
                }
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //print("hit");
        if ((atA && other.gameObject.Equals(patrolB)) || (!atA && other.gameObject.Equals(patrolA)))
        {
            
            coroutineRunning = true;
            StartCoroutine("SwitchWait");

        } 
        
    }
}
