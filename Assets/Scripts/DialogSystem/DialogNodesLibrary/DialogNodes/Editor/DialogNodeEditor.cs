using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [CustomEditor(typeof(DialogNode))]
    public class DialogNodeEditor : Editor, INodeInspector
    {
        DialogNode node;

        void OnEnable()
        {
            node = (DialogNode)target;
        }

        public void OnDrawNodeGUI(Rect rect)
        {
            string copyText = node.text.text;
            FillingTextField(rect, ref copyText, node.text[LanguageSystem.LangSys.DATA.mainLang]);// EditorGUI.TextArea(rect, node.text.text);
            node.text.text = copyText;
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