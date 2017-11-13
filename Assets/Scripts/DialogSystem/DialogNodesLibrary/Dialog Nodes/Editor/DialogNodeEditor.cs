using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(DialogNode))]
    public class DialogNodeEditor : NodeInspector
    {
        DialogNode node;

        void OnEnable()
        {
            node = (DialogNode)target;
        }

        public override void OnDrawNodeGUI(Rect rect)
        {
            node.dialog = (DialogCanvas)EditorGUILayout.ObjectField(node.dialog, typeof(DialogCanvas), false);
        }
    }
}