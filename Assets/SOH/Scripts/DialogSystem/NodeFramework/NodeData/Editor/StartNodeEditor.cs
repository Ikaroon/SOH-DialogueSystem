using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(StartNode))]
    public class StartNodeEditor : NodeInspector
    {
        //StartNode node;

        void OnEnable()
        {
          //node = (StartNode)target;
        }

        public override void OnDrawNodeGUI(Rect rect)
        {
            
        }
    }
}
