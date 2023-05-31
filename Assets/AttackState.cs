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
    private void Start()
    {
        isCharging = false;
        renderThrust = thrustObject.GetComponent<SpriteRenderer>();
        colliderThrust = thrustObject.GetComponent<CapsuleCollider2D>();
        rb = body.GetComponent<Rigidbody2D>();
        
    }

    public override State RunCurrentState()
    {
        
        if(!isCharging)
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

        Vector2 targetVector = new Vector2(target.transform.position.x, body.transform.position.y);
        float distanceToTarget = targetVector.x - body.transform.position.x;
        if(distanceToTarget < 0) //body is ahead of target in world space
        {
            chargeDist = -1 * transform.right * speed;
        } else //body is behind the target in world space
        {
            chargeDist = transform.right * speed;
        }
        isLunge = true;
        print("Attack");
        StartCoroutine("ChargeDuration");
    }
    private IEnumerator ChargeDuration()
    {
        yield return new WaitForSeconds(1);
        isLunge = false;
        print("lunge done");
    }
}
