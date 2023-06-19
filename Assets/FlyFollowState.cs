using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyFollowState : State
{
    public State nextState;
    private FaceTarget faceTargetScript;
    public GameObject body;
    public GameObject target;
    public float detectRadius;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    private Collider2D[] obstacleArray;
    private Collider2D[] targetArray;
    private void Start()
    {
        faceTargetScript = body.GetComponent<FaceTarget>();
    }
    public override State RunCurrentState()
    {
        obstacleArray = Physics2D.OverlapCircleAll(body.transform.position, detectRadius, obstacleMask);
        targetArray = Physics2D.OverlapCircleAll(body.transform.position, detectRadius, targetMask);
        return this;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(body.transform.position, detectRadius);
        if(obstacleArray != null)
        {
            Gizmos.color = Color.red;
            foreach (Collider2D obstacle in obstacleArray)
            {
                Gizmos.DrawSphere(obstacle.transform.position, 0.5f);
            }
        }
        if(targetArray != null)
        {
            Gizmos.color = Color.green;
            foreach (Collider2D target in targetArray)
            {
                Gizmos.DrawSphere(target.transform.position, 0.5f);
            }
        }
    }
}
