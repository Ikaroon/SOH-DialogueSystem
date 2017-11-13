using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(NoteNode))]
    public class NoteNodeEditor : NodeInspector
    {
        NoteNode node;

        private void OnEnable()
        {
            node = (NoteNode)target;
        }

        public override void OnDrawNodeGUI(Rect rect)
        {
            node.note = EditorGUI.TextArea(rect, node.note);
        }
    }
}