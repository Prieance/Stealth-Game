using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(EnemyFOV))]
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyFOV ScriptFOV = (EnemyFOV)target;

        Handles.color = Color.white;
        Handles.DrawWireArc(ScriptFOV.transform.position, Vector3.up, Vector3.forward, 360, ScriptFOV.Radius);

        Vector3 viewAngle1 = ScriptFOV.DirectionFromAngle(-ScriptFOV.Angle / 2, false);
        Vector3 viewAngle2 = ScriptFOV.DirectionFromAngle(ScriptFOV.Angle / 2, false);
        Handles.DrawLine(ScriptFOV.transform.position, ScriptFOV.transform.position + viewAngle1 * ScriptFOV.Radius);
        Handles.DrawLine(ScriptFOV.transform.position, ScriptFOV.transform.position + viewAngle2 * ScriptFOV.Radius);

        Handles.color = Color.magenta;
        foreach(Transform visibleTargets in ScriptFOV.visibleTargets)
        {
            Handles.DrawLine(ScriptFOV.transform.position, visibleTargets.position);
        }
    }
}
