using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(DiscardHubNode))]
    public class DiscardHubNodeEditor : Editor, INodeInspector
    {
        DiscardHubNode node;

        private void OnEnable()
        {
            node = (DiscardHubNode)target;
        }

        public void OnDrawNodeGUI(Rect rect)
        {
        }
    }
}