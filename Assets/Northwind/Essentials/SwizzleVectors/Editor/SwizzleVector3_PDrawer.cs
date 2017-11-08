using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Northwind.Essentials
{
    [CustomPropertyDrawer(typeof(SwizzleVector3))]
    public class SwizzleVector3_PDrawer : PropertyDrawer
    {

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var xRect = new Rect(position.x, position.y, position.width / 3f - 2f, position.height);
            var yRect = new Rect(position.x + position.width / 3f + 2f, position.y, position.width / 3f - 4f, position.height);
            var zRect = new Rect(position.x + position.width / 3f * 2f + 2f, position.y, position.width / 3f - 2f, position.height);

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(xRect, property.FindPropertyRelative("x"), GUIContent.none);
            EditorGUI.PropertyField(yRect, property.FindPropertyRelative("y"), GUIContent.none);
            EditorGUI.PropertyField(zRect, property.FindPropertyRelative("z"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}