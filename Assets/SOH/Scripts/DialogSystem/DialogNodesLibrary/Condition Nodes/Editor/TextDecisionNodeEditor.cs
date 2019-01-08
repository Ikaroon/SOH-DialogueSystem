using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;
using Northwind.Essentials;

using SpyOnHuman.CharacterSystem;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(TextDecisionNode))]
    public class TextDecisionNodeEditor : NodeInspector
    {
        TextDecisionNode node;

        void OnEnable()
        {
            node = (TextDecisionNode)target;
        }

        public override void OnDrawNodeGUI(Rect rect)
        {
            InitStyles();

            node.speaker = CharacterGUI.CharacterDropDown(new Rect(0f, 0f, rect.width, 16f), node.speaker);

            float height = (rect.height - 64f) / 3f;

            GUILayout.BeginArea(new Rect(0f, 32f, rect.width, height), styleGroup);
            EditorGUI.LabelField(new Rect(8f, 4f, rect.width - 16f, 16f), new GUIContent("Friendly"));
            string copyTextA = node.decisionAText.text;
            NorthwindGUI.FillingTextField(new Rect(8f, 24f, rect.width - 16f, height - 56f), ref copyTextA, node.decisionAText[LanguageSystem.LangSys.DATA.mainLang]);
            node.decisionAText.text = copyTextA;
            node.decisionAAudio.audio = (AudioClip)EditorGUI.ObjectField(new Rect(8f, height - 24f, rect.width - 16f, 16f), node.decisionAAudio.audio, typeof(AudioClip), false);
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(0f, 32f + height + 16f, rect.width, height), styleGroup);
            EditorGUI.LabelField(new Rect(8f, 4f, rect.width - 16f, 16f), new GUIContent("Strategic"));
            string copyTextB = node.decisionBText.text;
            NorthwindGUI.FillingTextField(new Rect(8f, 24f, rect.width - 16f, height - 56f), ref copyTextB, node.decisionBText[LanguageSystem.LangSys.DATA.mainLang]);
            node.decisionBText.text = copyTextB;
            node.decisionBAudio.audio = (AudioClip)EditorGUI.ObjectField(new Rect(8f, height - 24f, rect.width - 16f, 16f), node.decisionBAudio.audio, typeof(AudioClip), false);
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(0f, 32f + height * 2f + 32f, rect.width, height), styleGroup);
            EditorGUI.LabelField(new Rect(8f, 4f, rect.width - 16f, 16f), new GUIContent("Aggressive"));
            string copyTextC = node.decisionCText.text;
            NorthwindGUI.FillingTextField(new Rect(8f, 24f, rect.width - 16f, height - 56f), ref copyTextC, node.decisionCText[LanguageSystem.LangSys.DATA.mainLang]);
            node.decisionCText.text = copyTextC;
            node.decisionCAudio.audio = (AudioClip)EditorGUI.ObjectField(new Rect(8f, height - 24f, rect.width - 16f, 16f), node.decisionCAudio.audio, typeof(AudioClip), false);
            GUILayout.EndArea();
        }

        #region Extra Styles

        //The Styles used for the editor window
        private static GUIStyle styleGroup;

        /// <summary>
        /// Initialize the Styles used for the character editor
        /// </summary>
        private static void InitStyles()
        {
            if (styleGroup != null)
            {
                return;
            }

            styleGroup = GUI.skin.GetStyle("GroupBox");

        }

        #endregion
    }
}