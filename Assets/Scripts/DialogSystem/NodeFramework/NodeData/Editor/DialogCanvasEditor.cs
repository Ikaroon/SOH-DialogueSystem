using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    [CustomEditor(typeof(DialogCanvas))]
    public class DialogCanvasEditor : Editor
    {
        DialogCanvas canvas;

        private void OnEnable()
        {
            canvas = (DialogCanvas)target;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical(GUI.skin.GetStyle("GroupBox"));
            
            GUILayout.Label(new GUIContent("Dialog Canvas"), EditorStyles.largeLabel);


            GUILayout.Label(new GUIContent(canvas.canvasName == "" ? "No Name" : canvas.canvasName));
            GUILayout.Label(new GUIContent(canvas.canvasDescription == "" ? "No Name" : canvas.canvasDescription));

            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUI.skin.GetStyle("GroupBox"));

            GUILayout.Label(new GUIContent(canvas.canvasTimestamp), EditorStyles.boldLabel);

            GUILayout.EndVertical();
        }
    }
}