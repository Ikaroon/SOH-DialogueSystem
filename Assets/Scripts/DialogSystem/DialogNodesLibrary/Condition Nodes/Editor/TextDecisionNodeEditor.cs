using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

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

            float height = (rect.height - 32f) / 3f;

            GUILayout.BeginArea(new Rect(0f, 0f, rect.width, height), styleGroup);
            string copyTextA = node.decisionAText.text;
            FillingTextField(new Rect(8f, 8f, rect.width - 16f, height - 40f), ref copyTextA, node.decisionAText[LanguageSystem.LangSys.DATA.mainLang]);
            node.decisionAText.text = copyTextA;
            node.decisionAAudio.audio = (AudioClip)EditorGUI.ObjectField(new Rect(8f, height - 24f, rect.width - 16f, 16f), node.decisionAAudio.audio, typeof(AudioClip), false);
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(0f, height + 16f, rect.width, height), styleGroup);
            string copyTextB = node.decisionBText.text;
            FillingTextField(new Rect(8f, 8f, rect.width - 16f, height - 40f), ref copyTextB, node.decisionBText[LanguageSystem.LangSys.DATA.mainLang]);
            node.decisionBText.text = copyTextB;
            node.decisionBAudio.audio = (AudioClip)EditorGUI.ObjectField(new Rect(8f, height - 24f, rect.width - 16f, 16f), node.decisionBAudio.audio, typeof(AudioClip), false);
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(0f, height * 2f + 32f, rect.width, height), styleGroup);
            string copyTextC = node.decisionCText.text;
            FillingTextField(new Rect(8f, 8f, rect.width - 16f, height - 40f), ref copyTextC, node.decisionCText[LanguageSystem.LangSys.DATA.mainLang]);
            node.decisionCText.text = copyTextC;
            node.decisionCAudio.audio = (AudioClip)EditorGUI.ObjectField(new Rect(8f, height - 24f, rect.width - 16f, 16f), node.decisionCAudio.audio, typeof(AudioClip), false);
            GUILayout.EndArea();
        }

        #region Extra Styles

        //The Styles used for the editor window
        private static GUIStyle styleTextField, styleTextFieldEmpty, styleGroup;

        /// <summary>
        /// Initialize the Styles used for the character editor
        /// </summary>
        private static void InitStyles()
        {
            if (styleTextField != null)
            {
                return;
            }

            styleTextField = new GUIStyle(GUI.skin.GetStyle("LargeTextField"));
            styleTextField.fixedHeight = 0f;

            styleTextFieldEmpty = new GUIStyle(GUI.skin.GetStyle("WhiteLabel"));
            styleTextFieldEmpty.fixedHeight = 0f;
            styleTextFieldEmpty.border = styleTextField.border;
            styleTextFieldEmpty.font = styleTextField.font;
            styleTextFieldEmpty.fontStyle = styleTextField.fontStyle;
            styleTextFieldEmpty.fontSize = styleTextField.fontSize;
            styleTextFieldEmpty.normal.textColor *= 0.5f;

            styleGroup = GUI.skin.GetStyle("GroupBox");

        }

        #endregion

        #region Extra Fields

        /// <summary>
        /// A Text Field with a placeholder option
        /// </summary>
        /// <param name="rect">The Rect for the field</param>
        /// <param name="input">The string which should be edited</param>
        /// <param name="placeholder">The placeholder for the field</param>
        void FillingTextField(Rect rect, ref string input, string placeholder)
        {
            input = GUI.TextField(rect, input, styleTextField);
            if (input == "")
            {
                GUI.Label(rect, placeholder, styleTextFieldEmpty);
            }
        }

        #endregion
    }
}