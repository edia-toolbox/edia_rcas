using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RCAS;

namespace RCAS_Editor
{
    [CustomEditor(typeof(RCAS_Peer))]
    [CanEditMultipleObjects]
    public class RCAS_Peer_Editor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            RCAS_Peer peer = (RCAS_Peer)target;

            // Nothing here yet!
        }
    }
}
