using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(JumpToDialogNode))]
    public class JumpToDialogNodeEditor : NodeInspector
    {
        JumpToDialogNode node;

        void OnEnable()
        {
            node = (JumpToDialogNode)target;
        }

        public override void OnDrawNodeGUI(Rect rect)
        {
            node.dialog = (DialogCanvas)EditorGUILayout.ObjectField(node.dialog, typeof(DialogCanvas), false);
        }
    }
}