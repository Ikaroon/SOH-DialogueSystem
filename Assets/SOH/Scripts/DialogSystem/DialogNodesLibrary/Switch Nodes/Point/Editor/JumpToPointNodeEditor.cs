using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(JumpToPointNode))]
    public class JumpToPointNodeEditor : NodeInspector
    {
        JumpToPointNode node;

        private void OnEnable()
        {
            node = (JumpToPointNode)target;
        }

        public override void OnDrawNodeGUI(Rect rect, DialogCanvas canvas)
        {
            if (!canvas.definedPoints.ContainsKey(node.pointKey))
            {
                node.pointKey = "";
            }
            if (GUI.Button(rect, new GUIContent(node.pointKey == "" ? "-----" : node.pointKey), EditorStyles.popup))
            {
                List<string> keys = new List<string>(canvas.definedPoints.Keys);
                GenericMenu menu = new GenericMenu();

                for (int k = 0; k < keys.Count; k++)
                {
                    menu.AddItem(new GUIContent(keys[k]), false, ChangeKey, keys[k]);
                }

                menu.DropDown(new Rect(0f, 16f, 0f, 0f));
            }
        }

        private void ChangeKey(object obj)
        {
            node.pointKey = (string)obj;
        }
    }
}