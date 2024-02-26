using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAttackState : State
{
    private GameManager gameManager;
    private GameObject body;
    private GameObject target;
    public State nextState;
    public GameObject flyProjectile;
    private bool fired;
    private bool waitDone;
    private void Start()
    {
        fired = false;
        waitDone = false;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        target = GameObject.Find("Player");
        body = GameObject.Find("Fly");
    }
    public override State RunCurrentState()
    {
        
        if (!fired)
        {
            float angleRad = Mathf.Atan2(target.transform.position.y - gameObject.transform.position.y, target.transform.position.x - body.transform.position.x);
            float angleDeg = angleRad * (180 / Mathf.PI);
            Instantiate(flyProjectile, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z), Quaternion.Euler(0, 0, angleDeg));
            Debug.Log("Firing");
            fired = true;
            StartCoroutine("Wait");
        }
        

        //gameManager.FlyShoot(body, target);
        if (waitDone)
        {
            //resetting bool switches;
            waitDone = false;
            fired = false;
            return nextState;
        } else
        {
            return this;
        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f);
        waitDone = true;
    }


}
