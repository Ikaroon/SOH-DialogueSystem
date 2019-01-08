using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(DiscardHubNode))]
    public class DiscardHubNodeEditor : NodeInspector
    {
        //DiscardHubNode node;

        private void OnEnable()
        {
            //node = (DiscardHubNode)target;
        }

        public override void OnDrawNodeGUI(Rect rect)
        {
        }
    }
}