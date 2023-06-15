using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrafeDanceState : State
{
    public GameObject body;
    private Rigidbody2D rb;
    public float minDistance;
    public float maxDistance;
    public float outRange;
    public GameObject target;
    public float speed;
    private bool isStrafing;
    private bool coroutineRunning;
    private Vector2 strafeDist;
    private bool pause;
    public AttackState attack;
    private FaceTarget faceTargetScript;
    public FollowState followScript;
    private bool isCooldown;


    public void Start()
    {
        rb = body.GetComponent<Rigidbody2D>();
        isStrafing = false;
        coroutineRunning = false;
        strafeDist = new Vector2(0, 0);
        faceTargetScript = body.gameObject.GetComponent<FaceTarget>();
        isCooldown = false;
    }
    public override State RunCurrentState()
    {
        
        faceTargetScript.FacetheTarget();
        Vector2 targetVector = new Vector2(target.transform.position.x, body.transform.position.y);
        float distance = Vector2.Distance(body.transform.position, targetVector);
        if(!coroutineRunning)
        {
           if(distance > outRange)
           {
                isStrafing = false;
                coroutineRunning = false;
                isCooldown = true;
                rb.velocity = Vector2.zero;
                return followScript;
                
           }
            if (distance < minDistance)
            {
                int randomAttack = Random.Range(0, 2);

                if(randomAttack == 1 && !isCooldown)
                {
                    randomAttack = 0;
                    isStrafing = false;
                    coroutineRunning = false;
                    isCooldown = true;
                    rb.velocity = Vector2.zero;
                    StartCoroutine("Cooldown");
                    return attack;
                }

                strafeDist = -1 * transform.right * speed;

                isStrafing = true;

            }
            else if (distance > maxDistance)
            {

                strafeDist = transform.right * speed;

                isStrafing = true;
            }
            
            if (isStrafing)
            {

                isStrafing = false;
                coroutineRunning = true;
                StartCoroutine("StrafeDuration");
            }
        }
        if(!pause)
        {
            velocity = strafeDist * Time.deltaTime;
            //rb.velocity = strafeDist * Time.deltaTime;
        } else
        {
            velocity = Vector2.zero;
        }

        return this;
    }
    private IEnumerator StrafeDuration()
    {
        yield return new WaitForSeconds(0.5f);
        velocity = Vector2.zero;
        //rb.velocity = Vector2.zero;
        pause = true;
        yield return StartCoroutine("Wait");
        coroutineRunning = false;


    }
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.2f);
        pause = false;
    }
    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(1);
        isCooldown = false;
    }

}
