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
    private void Start()
    {
        atA = false;
        rb = body.GetComponent<Rigidbody2D>();
        colliderA = patrolA.GetComponent<Collider2D>();
        colliderB = patrolB.GetComponent<Collider2D>();
        directionDecided = false;
        coroutineRunning = false;
        pause = false;
        StartCoroutine("Wait");
    }
    public override State RunCurrentState()
    {
        if(atA)
        {
            ApproachPoint(patrolB);
        } else // at Point B
        {
            
            ApproachPoint(patrolA);
        }
        return this;
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
                if (directionalDist < 0) //body is ahead of point B in world space
                {
                    faceMovement(-1);
                    directionDecided = true;
                    

                }
                else //body is behind point B in world space
                {
                    faceMovement(1);
                    directionDecided = true;
                    

                }
            }
            if (!pause)
            {
                rb.velocity = transform.right * speed * Time.deltaTime;
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
        rb.velocity = new Vector2(0,0);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        print("hit");
        if (other.gameObject.Equals(patrolA) || other.gameObject.Equals(patrolB))
        {
            
            coroutineRunning = true;
            StartCoroutine("SwitchWait");

        }
        
    }
}
