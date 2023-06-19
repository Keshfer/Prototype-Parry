using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PatrolState))]
public class FOVEditor : Editor
{

    private void OnSceneGUI()
    {
        PatrolState fov = (PatrolState)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.forward, fov.transform.right, 360, fov.radius);

        Vector3 viewAngle1;
        Vector3 viewAngle2;
        if (fov.transform.eulerAngles.y == 0)
        {
            viewAngle1 = DirectionFromAngle(fov.transform.eulerAngles.z, -fov.angle / 2);
            viewAngle2 = DirectionFromAngle(fov.transform.eulerAngles.z, fov.angle / 2);
        } else
        {
            viewAngle1 = Vector3.Reflect(DirectionFromAngle(fov.transform.eulerAngles.z, (-fov.angle / 2)), Vector3.right );
            viewAngle2 = Vector3.Reflect(DirectionFromAngle(fov.transform.eulerAngles.z, (fov.angle / 2)), Vector3.right);
        }
        if(fov.playerDetected)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.player.transform.position);
        } else
        {
           
        }
        
        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + (viewAngle1 * fov.radius));
        Handles.DrawLine(fov.transform.position, fov.transform.position + (viewAngle2 * fov.radius));
            
    }

    private Vector3 DirectionFromAngle(float eulerZ, float angleInDegrees)
    {
        angleInDegrees += eulerZ;
        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
    }
}
