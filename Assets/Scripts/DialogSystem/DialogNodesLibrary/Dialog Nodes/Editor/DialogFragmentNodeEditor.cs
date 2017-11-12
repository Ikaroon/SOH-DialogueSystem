using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;
using Northwind.Essentials;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(DialogFragmentNode))]
    public class DialogFragmentNodeEditor : Editor, INodeInspector
    {
        DialogFragmentNode node;

        void OnEnable()
        {
            node = (DialogFragmentNode)target;
        }

        public void OnDrawNodeGUI(Rect rect)
        {
            node.speaker = DialogSystemDatabase.CharacterDropDown(new Rect(0f, 0f, rect.width, 16f), node.speaker);

            string copyText = node.text.text;
            NorthwindGUI.FillingTextField(new Rect(0f, 24f, rect.width, rect.height - 48f), ref copyText, node.text[LanguageSystem.LangSys.DATA.mainLang]);// EditorGUI.TextArea(rect, node.text.text);
            node.text.text = copyText;

            node.audio.audio = (AudioClip)EditorGUI.ObjectField(new Rect(0f, rect.height - 16f, rect.width, 16f), node.audio.audio, typeof(AudioClip), false);
        }
    }
}