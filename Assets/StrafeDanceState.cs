using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafeDanceState : State
{
    public GameObject body;
    private Rigidbody2D rb;
    public float minDistance;
    public float maxDistance;
    public GameObject target;
    public float speed;
    private bool isStrafing;
    private bool coroutineRunning;
    private Vector2 strafeDist;
    private bool pause;
    public AttackState attack;

    public void Start()
    {
        rb = body.GetComponent<Rigidbody2D>();
        isStrafing = false;
        coroutineRunning = false;
        strafeDist = new Vector2(0, 0);
    }
    public override State RunCurrentState()
    {
        //StartCoroutine("Strafe");
        Vector2 targetVector = new Vector2(target.transform.position.x, body.transform.position.y);
        float distance = Vector2.Distance(body.transform.position, targetVector);
        if(!coroutineRunning)
        {
           
            if (distance < minDistance)
            {
                int randomAttack = Random.Range(0, 2);
                print(randomAttack);
                if(randomAttack == 1)
                {
                    return attack;
                }
                //print("min");
                float distanceToTarget = targetVector.x - body.transform.position.x;
                if(distanceToTarget < 0) //body is ahead of target in world space
                {
                    //print("ahead");
                    strafeDist = transform.right * speed;
                } else //body is behind target in world space
                {
                    //print("behind");
                    strafeDist = -1 * transform.right * speed;
                    //print(strafeDist);
                }
                
                isStrafing = true;

            }
            else if (distance > maxDistance)
            {
                //print("max");
                float distanceToTarget = targetVector.x - body.transform.position.x;
                if(distanceToTarget < 0) //body is ahead of target in world space
                {
                    strafeDist = -1 * transform.right * speed;
                } else //body is behind target in world space
                {
                    strafeDist = transform.right * speed;
                    //print(strafeDist);
                }
                
                isStrafing = true;
            }
            
            if (isStrafing)
            {
                //print("strafing true");
                isStrafing = false;
                coroutineRunning = true;
                StartCoroutine("StrafeDuration");
            }
        }
        //print("HI looke at me");
        if(!pause)
        {
            rb.velocity = strafeDist * Time.deltaTime;
        }

        return this;
    }
    private IEnumerator StrafeDuration()
    {

        //print("running coroutine");
        
        yield return new WaitForSeconds(0.5f);
        rb.velocity = new Vector2(0, 0);
        pause = true;
        yield return StartCoroutine("Wait");
        coroutineRunning = false;
        
        //print("coroutine done");

    }
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.2f);
        pause = false;
    }




    /*private IEnumerator Strafe()
    {

        yield return new WaitForSeconds(0.2f);
        Vector2 targetVector = new Vector2(target.transform.position.x, body.transform.position.y);
        float distance = Vector2.Distance(body.transform.position, targetVector);
        //print(distance);
        if (distance < minDistance)
        {
            Vector2 strafeDist = Vector2.MoveTowards(body.transform.position, targetVector, -speed * Time.deltaTime);
            //print(strafeDist);
            StartCoroutine("StrafeMove", strafeDist);

            
        }
        else if (distance > maxDistance)
        {
            print("bahaha");
            Vector2 strafeDist = Vector2.MoveTowards(body.transform.position, targetVector, speed * Time.deltaTime);
            print(strafeDist);
            yield return StartCoroutine("StrafeMove", strafeDist);

        } else
        {
            yield return null;  
        }
        

    }
    private IEnumerator StrafeMove(Vector2 dist)
    {
        
        float randSec = Random.Range(1, 3);
        rb.velocity = dist;
        
        yield return new WaitForSeconds(1f);
        rb.velocity = new Vector2(0, 0);
        
    }
    */

}
