using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;
using Northwind.Essentials;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(ChooseNode))]
    public class ChooseNodeEditor : NodeInspector
    {
        ChooseNode node;

        void OnEnable()
        {
            node = (ChooseNode)target;
        }

        public override void OnDrawNodeGUI(Rect rect)
        {
            // Draw the Inspector
        }
    }
}