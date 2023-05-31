using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public GameObject target;
    public float speed;
    public float minDistance;
    private Vector2 targetVector2;
    private FollowTarget followTargetScript;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        targetVector2 = new Vector2(target.transform.position.x, transform.position.y);
        if (Vector2.Distance(transform.position, targetVector2) >= minDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetVector2, speed * Time.deltaTime);
        } else
        {
            followTargetScript.enabled = false;
            
        }
        
    }
}
