using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(DefinePointNode))]
    public class DefinePointNodeEditor : NodeInspector
    {
        DefinePointNode node;

        private void OnEnable()
        {
            node = (DefinePointNode)target;
        }

        public override void OnDrawNodeGUI(Rect rect, DialogCanvas canvas)
        {
            EditorGUI.BeginChangeCheck();
            string newKey = EditorGUI.DelayedTextField(new Rect(0f, 0f, rect.width, 16f), node.pointKey.ToUpper().Replace(" ", "")).ToUpper().Replace(" ", "");
            if (EditorGUI.EndChangeCheck())
            {
                node.pointKey = node.pointKey.ToUpper().Replace(" ", "");
                newKey = newKey.ToUpper().Replace(" ", "");
                if (newKey != "")
                {
                    if (canvas.definedPoints.ContainsKey(node.pointKey))
                    {
                        canvas.definedPoints.Remove(node.pointKey);
                    }
                    if (!canvas.definedPoints.ContainsKey(newKey))
                    {
                        canvas.definedPoints.Add(newKey, node);
                        node.isValid = true;
                    }
                    else
                    {
                        node.isValid = false;
                    }
                    node.pointKey = newKey.ToUpper().Replace(" ", "");
                }
            }

            if (!node.isValid && node.pointKey != "")
            {
                EditorGUI.LabelField(new Rect(0f, 16f, rect.width, 16f), new GUIContent("Already used"), GUI.skin.GetStyle("ErrorLabel"));
            } else if(node.pointKey == "")
            {
                EditorGUI.LabelField(new Rect(0f, 16f, rect.width, 16f), new GUIContent("Has to be set!"), GUI.skin.GetStyle("ErrorLabel"));
            }
        }
    }
}