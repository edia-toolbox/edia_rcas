using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RCAS_Peer))]
[CanEditMultipleObjects]
public class RCAS_Peer_Editor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RCAS_Peer peer = (RCAS_Peer)target;
    }
}
