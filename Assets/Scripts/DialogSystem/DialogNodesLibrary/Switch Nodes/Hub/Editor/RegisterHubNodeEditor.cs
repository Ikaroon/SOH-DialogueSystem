using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(RegisterHubNode))]
    public class RegisterHubNodeEditor : NodeInspector
    {
        //RegisterHubNode node;

        private void OnEnable()
        {
            //node = (RegisterHubNode)target;
        }

        public override void OnDrawNodeGUI(Rect rect)
        {
        }
    }
}