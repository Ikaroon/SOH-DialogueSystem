using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(JumpToPointNode))]
    public class JumpToPointNodeEditor : Editor, INodeInspector
    {
        JumpToPointNode node;

        private void OnEnable()
        {
            node = (JumpToPointNode)target;
        }

        public void OnDrawNodeGUI(Rect rect)
        {
            node.pointKey = EditorGUI.TextField(rect, node.pointKey);
        }
    }
}