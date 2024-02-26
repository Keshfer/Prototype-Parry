using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject flyProjectile;
    public void FlyShoot(GameObject shooter, GameObject target)
    {
        float angleRad = Mathf.Atan2(target.transform.position.y - shooter.transform.position.y, target.transform.position.x - shooter.transform.position.x);
        float angleDeg = angleRad * (180 / Mathf.PI);
        Instantiate(flyProjectile, new Vector3(shooter.transform.position.x, shooter.transform.position.y, shooter.transform.position.z), Quaternion.Euler(0,0, angleDeg));
    }
}
