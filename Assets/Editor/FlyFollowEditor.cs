using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FlyFollowState))]

public class CircleRangeEditor : Editor
{
    private void OnSceneGUI()
    {
        FlyFollowState circling = (FlyFollowState)target;
        Handles.color = Color.yellow;
        Handles.DrawWireArc(circling.transform.position, Vector3.forward, Vector3.right, 360, circling.circleRange);
    }
}
