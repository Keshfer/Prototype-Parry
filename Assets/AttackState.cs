using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public GameObject body;
    public GameObject thrustObject;
    private Rigidbody2D rb;
    private SpriteRenderer renderThrust;
    private CapsuleCollider2D colliderThrust;
    private bool isCharging;
    private bool isLunge;
    public GameObject target;
    private Vector2 chargeDist;
    public float speed;
    private FaceTarget faceTargetScript;
    public StrafeDanceState strafeDance;
    private bool hasLunged;
    private void Start()
    {
        isCharging = false;
        renderThrust = thrustObject.GetComponent<SpriteRenderer>();
        colliderThrust = thrustObject.GetComponent<CapsuleCollider2D>();
        rb = body.GetComponent<Rigidbody2D>();
        faceTargetScript = body.GetComponent<FaceTarget>();
        hasLunged = false;
        
    }

    public override State RunCurrentState()
    {
        print("attack");
        if(hasLunged)
        {
            hasLunged = false;
            isCharging = false;
            renderThrust.enabled = false;
            colliderThrust.enabled = false;
            return strafeDance;
        }
        if (!isCharging)
        {
            isCharging = true;
            StartCoroutine("Charging");
        }
        if(isLunge)
        {
            rb.velocity = chargeDist * Time.deltaTime;
        }
        return this;

    }
    private IEnumerator Charging()
    {
        yield return new WaitForSeconds(1);
        renderThrust.enabled = true;
        colliderThrust.enabled = true;
        /*
        Vector2 targetVector = new Vector2(target.transform.position.x, body.transform.position.y);
        float distanceToTarget = targetVector.x - body.transform.position.x;
        if(distanceToTarget < 0) //body is ahead of target in world space
        {
            chargeDist = transform.right * speed;
        } else //body is behind the target in world space
        {
            chargeDist = transform.right * speed;
        }
        */
        chargeDist = transform.right * speed;
        isLunge = true;
        //print("Attack");
        yield return StartCoroutine("ChargeDuration");
    }
    private IEnumerator ChargeDuration()
    {
        yield return new WaitForSeconds(1);
        rb.velocity = new Vector2(0, 0);
        isLunge = false;
        renderThrust.enabled = false;
        colliderThrust.enabled = false;
        isCharging = false;
        hasLunged = true;
        
        //print("lunge done");
    }
}
