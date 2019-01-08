using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;
using Northwind.Essentials;
using SpyOnHuman.CharacterSystem;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(DialogFragmentNode))]
    public class DialogFragmentNodeEditor : NodeInspector
    {
        DialogFragmentNode node;

        void OnEnable()
        {
            node = (DialogFragmentNode)target;
        }

        public override void OnDrawNodeGUI(Rect rect)
        {
            node.speaker = CharacterGUI.CharacterDropDown(new Rect(0f, 0f, rect.width, 16f), node.speaker);

            GUILayout.BeginArea(new Rect(new Rect(0f, 18f, rect.width, 16f)));
            GUILayout.BeginHorizontal();
            node.isAuto = GUILayout.Toggle(node.isAuto, new GUIContent("Auto"));
            node.showTime = EditorGUILayout.FloatField(new GUIContent("Time"), node.showTime);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            string copyText = node.textSource.text;
            NorthwindGUI.FillingTextField(new Rect(0f, 40f, rect.width, rect.height - 64f), ref copyText, node.textSource[LanguageSystem.LangSys.DATA.mainLang]);// EditorGUI.TextArea(rect, node.text.text);
            node.textSource.text = copyText;

            node.audioSource.audio = (AudioClip)EditorGUI.ObjectField(new Rect(0f, rect.height - 16f, rect.width, 16f), node.audioSource.audio, typeof(AudioClip), false);
        }
    }
}