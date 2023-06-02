using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTarget : MonoBehaviour
{
    public GameObject body;
    public GameObject target;
    
    public void FacetheTarget()
    {
        if (target.activeSelf.Equals(true))
        {
            Vector2 targetVector = new Vector2(target.transform.position.x, body.transform.position.y);
            float distanceToTarget = targetVector.x - body.transform.position.x;
            if (distanceToTarget < 0) //body is ahead of target in world space
            {
                body.transform.eulerAngles = new Vector3(body.transform.rotation.x, 180, body.transform.rotation.z);
            }
            else //body is behind the target in world space
            {
                body.transform.eulerAngles = new Vector3(body.transform.rotation.x, 0, body.transform.rotation.z);
            }
        }
    }
}
