using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowState : State
{
    public GameObject target;
    public float speed;
    public float minDistance;
    private Vector2 targetVector2;
    public StrafeDanceState strafeDance;
    public GameObject body;
    private FaceTarget faceTargetScript;

    private void Start()
    {
        faceTargetScript = body.GetComponent<FaceTarget>();
    }
    public override State RunCurrentState()
    {




        targetVector2 = new Vector2(target.transform.position.x, body.transform.position.y);
        if (Vector2.Distance(body.transform.position, targetVector2) >= minDistance)
        {

            faceTargetScript.FacetheTarget();
            body.transform.position = Vector2.MoveTowards(body.transform.position, targetVector2, speed * Time.deltaTime);
            return this;
        } else
        {
            return strafeDance;
        }

    }

    

}
