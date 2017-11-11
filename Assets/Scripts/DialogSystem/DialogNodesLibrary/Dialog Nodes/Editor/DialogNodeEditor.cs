using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(DialogNode))]
    public class DialogNodeEditor : Editor, INodeInspector
    {
        DialogNode node;

        void OnEnable()
        {
            node = (DialogNode)target;
        }

        public void OnDrawNodeGUI(Rect rect)
        {
            node.dialog = (DialogCanvas)EditorGUILayout.ObjectField(node.dialog, typeof(DialogCanvas), false);
        }
    }
}