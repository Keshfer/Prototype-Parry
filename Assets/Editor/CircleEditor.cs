using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CircleState))]
public class CircleEditor : Editor
{
    private void OnSceneGUI()
    {
        CircleState circle = (CircleState)target;
        Handles.color = Color.cyan;
        Handles.DrawWireArc(circle.transform.position, Vector3.forward, circle.transform.right, 360, circle.circleRadius);
    }
}
