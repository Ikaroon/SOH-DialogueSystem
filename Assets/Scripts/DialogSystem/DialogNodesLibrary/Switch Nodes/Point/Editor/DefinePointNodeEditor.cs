using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(DefinePointNode))]
    public class DefinePointNodeEditor : Editor, INodeInspector
    {
        DefinePointNode node;

        private void OnEnable()
        {
            node = (DefinePointNode)target;
        }

        public void OnDrawNodeGUI(Rect rect)
        {
            node.pointKey = EditorGUI.TextField(rect, node.pointKey);
        }
    }
}