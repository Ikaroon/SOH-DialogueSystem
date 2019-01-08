using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.CharacterSystem
{
    public class CharacterEditWindow : EditorWindow
    {

        #region Backend Data

        //The image used as Profile placeholder
        public Texture2D profile;

        //The last path used to save the Character
        private string lastSavedPath;

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Style Data
        
        //The Styles used for the editor window
        private static GUIStyle styleBox, styleHeader, styleTimestamp, styleCenterLabel, styleToolbar, styleToolbarButton, styleTextField, styleTextFieldNoText, styleTextFieldEmpty;

        #endregion

        #region Stlye Methods

        /// <summary>
        /// Initialize the Styles used for the character editor
        /// </summary>
        private static void InitStyles()
        {
            if (styleBox != null)
            {
                return;
            }

            styleBox = new GUIStyle(GUI.skin.GetStyle("GroupBox"));

            styleHeader = new GUIStyle(GUI.skin.GetStyle("HeaderLabel"));
            styleHeader.alignment = TextAnchor.MiddleLeft;

            styleTimestamp = new GUIStyle(GUI.skin.GetStyle("HeaderLabel"));
            styleTimestamp.alignment = TextAnchor.MiddleRight;

            styleCenterLabel = new GUIStyle(GUI.skin.GetStyle("HeaderLabel"));
            styleCenterLabel.alignment = TextAnchor.MiddleCenter;

            styleToolbar = new GUIStyle(GUI.skin.GetStyle("Toolbar"));

            styleToolbarButton = new GUIStyle(GUI.skin.GetStyle("toolbarbutton"));

            styleTextField = new GUIStyle(GUI.skin.GetStyle("LargeTextField"));
            styleTextField.fixedHeight = 0f;
            styleTextField.alignment = TextAnchor.MiddleLeft;

            styleTextFieldNoText = new GUIStyle(GUI.skin.GetStyle("LargeTextField"));
            styleTextFieldNoText.fixedHeight = 0f;
            styleTextFieldNoText.fontSize = 0;
            styleTextFieldNoText.normal.textColor = Color.clear;
            styleTextFieldNoText.focused.textColor = Color.clear;
            styleTextFieldNoText.active.textColor = Color.clear;

            styleTextFieldEmpty = new GUIStyle(GUI.skin.GetStyle("WhiteLabel"));
            styleTextFieldEmpty.fixedHeight = 0f;
            styleTextFieldEmpty.alignment = TextAnchor.MiddleLeft;
            styleTextFieldEmpty.normal.textColor *= 0.5f;

        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Editor Data

        //The current edited Character Sheet
        private Character sheet;

        //The current scroll amount for the entire view
        private static Vector2 scrollView = Vector2.zero;

        #endregion

        #region Window Functions

        [MenuItem("SpyOnHuman/Character Editor", priority = 1)]
        static void Init()
        {
            CharacterEditWindow window = (CharacterEditWindow)EditorWindow.GetWindow(typeof(CharacterEditWindow));
            window.Show();
        }

        void OnEnable()
        {
            lastSavedPath = "";
            minSize = new Vector2(400f, 200f);
            titleContent = new GUIContent("Char Editor");
        }

        //The window draw method
        void OnGUI()
        {
            //Initialize Styles
            InitStyles();

            //Draw the Toolbar for the Character editor
            Toolbar(new Rect(0f, 0f, position.width, 18f));

            //Draw the Character Sheet editor when a sheet is available
            if (sheet != null)
            {
                scrollView = GUI.BeginScrollView(new Rect(0f, 18f, position.width, position.height - 18f), scrollView, new Rect(8f, 8f, position.width - 16f, 272f), false, true);
                CharacterSheet(new Rect(16f, 16f, position.width, position.height));
                GUI.EndScrollView();
            }

            //Check for Events
            if (Event.current.commandName == "ObjectSelectorClosed")
            {
                if (EditorGUIUtility.GetObjectPickerControlID() == 10)
                {
                    sheet.profile = (Texture2D)EditorGUIUtility.GetObjectPickerObject();
                    Repaint();
                }
            }
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Toolbar Methods

        /// <summary>
        /// A method which draws the toolbar for the editor view
        /// </summary>
        /// <param name="rect">The Area used for the Toolbar</param>
        void Toolbar(Rect rect)
        {
            GUILayout.BeginArea(rect, styleToolbar);

            if (GUI.Button(new Rect(8f, 0f, 100f, rect.height), "Data", styleToolbarButton))
            {
                DrawDataMenu(new Rect(8f, 18f, 0f, 0f));
            }

            if (GUI.Button(new Rect(108f, 0f, 100f, rect.height), "Edit", styleToolbarButton))
            {
                DrawEditMenu(new Rect(108f, 18f, 0f, 0f));
            }

            GUILayout.EndArea();
        }

        #endregion

        #region Data Menu Methods

        /// <summary>
        /// Draws the Data DropDown Menu
        /// </summary>
        /// <param name="rect">The position for the menu</param>
        void DrawDataMenu(Rect rect)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("New"), false, New);
            menu.AddItem(new GUIContent("Load"), false, Load);

            if (sheet)
            {
                menu.AddItem(new GUIContent("Save"), false, Save);
                menu.AddItem(new GUIContent("Save As"), false, SaveAs);
            } else
            {
                menu.AddDisabledItem(new GUIContent("Save"));
                menu.AddDisabledItem(new GUIContent("Save As"));
            }

            menu.DropDown(rect);
        }

        //Creates a new Character sheet
        void New()
        {
            sheet = (Character)ScriptableObject.CreateInstance<Character>();
            lastSavedPath = "";
        }

        //Loads a Character sheet
        void Load()
        {
            string path = lastSavedPath = EditorUtility.OpenFilePanel("Load Character Sheet", Character.PATH, "asset");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            else if (!path.Contains(Application.dataPath))
            {
                return;
            }
            path = path.Replace(Application.dataPath, "Assets");
            sheet = AssetDatabase.LoadAssetAtPath(path, typeof(Character)) as Character;
        }

        //Loads a specific Character sheet
        public void Load(string path)
        {
            sheet = null;
            lastSavedPath = path;
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            sheet = AssetDatabase.LoadAssetAtPath(path, typeof(Character)) as Character;
        }

        //Tries to Save the Character sheet at the last given path
        void Save()
        {
            if (string.IsNullOrEmpty(lastSavedPath))
            {
                SaveAs();
                return;
            }
            sheet.timestamp = System.DateTime.Now.ToString("yyyy/MM/dd | HH:mm");
            string tempPath = lastSavedPath.Replace(Application.dataPath, "Assets");
            if (AssetDatabase.LoadAssetAtPath<Character>(tempPath) == sheet)
            {
                EditorUtility.SetDirty(sheet);
                return;
            }
            AssetDatabase.CreateAsset(sheet, lastSavedPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        //Saves the Character sheet at the position selected via the Save File Panel
        void SaveAs()
        {
            string path = lastSavedPath = EditorUtility.SaveFilePanelInProject("Save Character Sheet", "CHR_" + sheet.forename + sheet.surname, "asset", "", Character.PATH);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            sheet.timestamp = System.DateTime.Now.ToString("yyyy/MM/dd | HH:mm");
            string tempPath = lastSavedPath.Replace(Application.dataPath, "Assets");
            if (AssetDatabase.LoadAssetAtPath<Character>(tempPath) == sheet)
            {
                EditorUtility.SetDirty(sheet);
                return;
            }
            else if (AssetDatabase.LoadAssetAtPath<Character>(tempPath) != null)
            {
                AssetDatabase.DeleteAsset(tempPath);
            }
            AssetDatabase.CreateAsset(sheet, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #endregion

        #region Edit Menu Methods

        /// <summary>
        /// Draw the Edit DropDown Menu
        /// </summary>
        /// <param name="rect">The position for the menu</param>
        void DrawEditMenu(Rect rect)
        {
            GenericMenu menu = new GenericMenu();

            if (sheet)
            {
                menu.AddItem(new GUIContent("Randomize"), false, null);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Randomize"));
            }

            menu.DropDown(rect);
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Header

        //Draws the Header for the Character sheet and send the draw command to all sub drawers of the Character sheet
        void CharacterSheet(Rect rect)
        {
            //Draw the Sheet header
            GUILayout.BeginArea(new Rect(16f, 16f, rect.width - 32f, 32f), styleBox);
            float halfWidth = (rect.width - 56f) / 2f;
            GUI.Label(new Rect(8f, 8f, halfWidth, 16f), "Character Sheet", styleHeader);
            GUI.Label(new Rect(16f + halfWidth, 8f, halfWidth, 16f), sheet.timestamp, styleTimestamp);
            GUILayout.EndArea();

            //Draw Sheet body
            Profile(new Rect(16f, 56f, rect.width - 32f, 96f));
            ExtraSettings(new Rect(16f, 160f, rect.width - 32f, 64f));
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Character Settings Methods

        /// <summary>
        /// Draws the Profile part of the Character Editor
        /// </summary>
        /// <param name="rect">The position for the Profile Section</param>
        void Profile(Rect rect)
        {
            GUILayout.BeginArea(rect, styleBox);

            GUI.Box(new Rect(8f, 8f, 80f, 80f), "", styleBox);
            if (GUI.Button(new Rect(8f, 8f, 80f, 80f), "Profile", styleCenterLabel))
            {
                EditorGUIUtility.ShowObjectPicker<Texture2D>(sheet.profile, false, "", 10);
            }
            if (profile != null && sheet.profile == null)
            {
                GUI.DrawTexture(new Rect(9f, 9f, 78f, 78f), profile, ScaleMode.StretchToFill);
            }
            else if (sheet.profile != null)
            {
                GUI.DrawTexture(new Rect(9f, 9f, 78f, 78f), sheet.profile, ScaleMode.StretchToFill);
            }

            GUILayout.BeginArea(new Rect(96f, 8f, rect.width - 104f, 120f));

            FillingTextField(new Rect(0f, 0f, (rect.width - 112f) / 2f, 16f), ref sheet.forename, "Forename");
            FillingTextField(new Rect((rect.width - 116f) / 2f + 10f, 0f, (rect.width - 112f) / 2f, 16f), ref sheet.surname, "Surname");

            FillingIntField(new Rect(0f, 32f, (rect.width - 112f) * 0.25f, 16f), ref sheet.age, "Age");
            sheet.gender = (Character.Genders)EditorGUI.EnumPopup(new Rect((rect.width - 116f) * 0.25f + 10f, 31f, (rect.width - 112f) * 0.75f, 16f), sheet.gender);

            FillingTextField(new Rect(0f, 64f, (rect.width - 112f), 16f), ref sheet.profession, "Profession");

            GUILayout.EndArea();

            GUILayout.EndArea();
        }

        #endregion

        #region Extra Settings Methods

        /// <summary>
        /// Draws the Extra Settings part of the Character Editor
        /// </summary>
        /// <param name="rect">The position for the ExtraSettings Section</param>
        void ExtraSettings(Rect rect)
        {
            GUILayout.BeginArea(rect, styleBox);

            sheet.textColor = EditorGUI.ColorField(new Rect(8f, 8f, rect.width - 16f, 16f), "Text Color", sheet.textColor);
            sheet.isPlayer = EditorGUI.Toggle(new Rect(8f, 40f, rect.width - 16f, 16f), "Is Player", sheet.isPlayer);

            GUILayout.EndArea();
        }

        #endregion

        //-----------------------------------------------------------------------------------------

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

        /// <summary>
        /// A Int Field with a placeholder option
        /// </summary>
        /// <param name="rect">The Rect for the field</param>
        /// <param name="input">The int which should be edited</param>
        /// <param name="placeholder">The placeholder for the field</param>
        void FillingIntField(Rect rect, ref int input, string placeholder)
        {
            input = Mathf.Max(0, EditorGUI.IntField(rect, input, (int)input != 0 ? styleTextField : styleTextFieldNoText));
            if (input == 0)
            {
                GUI.Label(rect, placeholder, styleTextFieldEmpty);
            }
        }

        #endregion

    }
}