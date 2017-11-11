using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(TextDecisionNode))]
    public class TextDecisionNodeEditor : Editor, INodeInspector
    {
        TextDecisionNode node;

        void OnEnable()
        {
            node = (TextDecisionNode)target;
        }

        public void OnDrawNodeGUI(Rect rect)
        {
            float height = (rect.height - 8f) / 3f;

            string copyTextA = node.decisionAText.text;
            FillingTextField(new Rect(0f, 0f, rect.width, height), ref copyTextA, node.decisionAText[LanguageSystem.LangSys.DATA.mainLang]);
            node.decisionAText.text = copyTextA;

            string copyTextB = node.decisionBText.text;
            FillingTextField(new Rect(0f, height + 4f, rect.width, height), ref copyTextB, node.decisionBText[LanguageSystem.LangSys.DATA.mainLang]);
            node.decisionBText.text = copyTextB;

            string copyTextC = node.decisionCText.text;
            FillingTextField(new Rect(0f, height * 2f + 8f, rect.width, height), ref copyTextC, node.decisionCText[LanguageSystem.LangSys.DATA.mainLang]);
            node.decisionCText.text = copyTextC;
        }

        #region Extra Styles

        //The Styles used for the editor window
        private static GUIStyle styleTextField, styleTextFieldEmpty;

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
            InitStyles();

            input = GUI.TextField(rect, input, styleTextField);
            if (input == "")
            {
                GUI.Label(rect, placeholder, styleTextFieldEmpty);
            }
        }

        #endregion
    }
}