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
        //print("attack");
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
            velocity = chargeDist * Time.deltaTime; 
            //rb.velocity = chargeDist * Time.deltaTime;
        }
        return this;

    }
    private IEnumerator Charging()
    {
        yield return new WaitForSeconds(1);
        renderThrust.enabled = true;
        colliderThrust.enabled = true;
        chargeDist = transform.right * speed;
        isLunge = true;
        yield return StartCoroutine("ChargeDuration");
    }
    private IEnumerator ChargeDuration()
    {
        yield return new WaitForSeconds(1);
        velocity = Vector2.zero;
        //rb.velocity = new Vector2(0, 0);
        isLunge = false;
        renderThrust.enabled = false;
        colliderThrust.enabled = false;
        isCharging = false;
        hasLunged = true;
        
        //print("lunge done");
    }
}
