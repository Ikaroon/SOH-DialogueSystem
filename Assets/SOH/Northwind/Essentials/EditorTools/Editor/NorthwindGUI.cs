using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Northwind.Essentials
{
    public static class NorthwindGUI
    {
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

            styleTextField = new GUIStyle(EditorStyles.textArea);
            styleTextField.fixedHeight = 0f;

            styleTextFieldEmpty = new GUIStyle(EditorStyles.whiteLabel);
            styleTextFieldEmpty.fixedHeight = 0f;
            styleTextFieldEmpty.fontSize = styleTextField.fontSize;
            styleTextFieldEmpty.normal.textColor *= 0.5f;
            styleTextFieldEmpty.wordWrap = styleTextField.wordWrap;

        }

        #endregion

        #region Extra Fields

        /// <summary>
        /// A Text Field with a placeholder option
        /// </summary>
        /// <param name="rect">The Rect for the field</param>
        /// <param name="input">The string which should be edited</param>
        /// <param name="placeholder">The placeholder for the field</param>
        public static void FillingTextField(Rect rect, ref string input, string placeholder)
        {
            InitStyles();

            input = EditorGUI.TextArea(rect, input, styleTextField);
            if (input == "")
            {
                EditorGUI.LabelField(rect, placeholder, styleTextFieldEmpty);
            }
        }

        #endregion
    }
}