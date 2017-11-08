using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem.CharacterSystem
{
    public class CharacterEditWindow : EditorWindow
    {

        #region Backend Data

        public Texture2D ico, profile;
        private string lastSavedPath;

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Style Data

        private static GUIStyle styleBox, styleHeader, styleTimestamp, styleCenterLabel, styleToolbar, styleToolbarButton, styleTextField, styleTextFieldNoText, styleTextFieldEmpty;

        #endregion

        #region Stlye Methods

        private static void InitStyles()
        {
            styleBox = new GUIStyle(GUI.skin.GetStyle("GroupBox"));

            styleHeader = new GUIStyle(GUI.skin.GetStyle("HeaderLabel"));
            styleHeader.alignment = TextAnchor.MiddleLeft;

            styleTimestamp = new GUIStyle(GUI.skin.GetStyle("HeaderLabel"));
            styleTimestamp.alignment = TextAnchor.MiddleRight;

            styleCenterLabel = new GUIStyle(GUI.skin.GetStyle("HeaderLabel"));
            styleCenterLabel.alignment = TextAnchor.MiddleCenter;

            styleToolbar = new GUIStyle(GUI.skin.GetStyle("Toolbar"));
            styleToolbar.fixedHeight = 0f;

            styleToolbarButton = new GUIStyle(GUI.skin.GetStyle("toolbarbutton"));
            styleToolbarButton.fixedHeight = 0f;

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

        private Character sheet;

        private static Vector2 scrollView = Vector2.zero;

        private int removedStorySheet = -1;

        #endregion

        #region Window Functions

        [MenuItem("SpyOnHuman/Character Editor")]
        static void Init()
        {
            CharacterEditWindow window = (CharacterEditWindow)EditorWindow.GetWindow(typeof(CharacterEditWindow));
            window.Show();
            window.minSize = new Vector2(400f, 200f);
        }

        void OnEnable()
        {
            lastSavedPath = "";
            this.titleContent = new GUIContent("Char Editor", ico);
        }

        void Update()
        {
            if (removedStorySheet >= 0 && sheet != null && removedStorySheet < sheet.storySheets.Count)
            {
                sheet.storySheets.RemoveAt(removedStorySheet);
                removedStorySheet = -1;
            }
        }

        void OnGUI()
        {
            if (styleBox == null)
            {
                InitStyles();
            }

            Toolbar(new Rect(0f, 0f, position.width, 20f));


            if (sheet != null)
            {
                scrollView = GUI.BeginScrollView(new Rect(0f, 20f, position.width, position.height - 20f), scrollView, new Rect(8f, 8f, position.width - 16f, 368f + 40f * sheet.storySheets.Count), false, true);
                CharacterSheet(new Rect(16f, 16f, position.width, position.height));
                GUI.EndScrollView();
            }

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

        void Toolbar(Rect rect)
        {
            GUILayout.BeginArea(rect, styleToolbar);

            if (GUI.Button(new Rect(8f, 0f, 100f, rect.height), "Data", styleToolbarButton))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("New"), false, New);
                menu.AddItem(new GUIContent("Load"), false, Load);
                menu.AddItem(new GUIContent("Save"), false, Save);
                menu.AddItem(new GUIContent("Save As"), false, SaveAs);
                menu.ShowAsContext();
            }

            if (GUI.Button(new Rect(108f, 0f, 100f, rect.height), "Edit", styleToolbarButton))
            {

            }

            GUILayout.EndArea();
        }

        #endregion

        #region Data Menu Methods

        void New()
        {
            sheet = (Character)ScriptableObject.CreateInstance<Character>();
            lastSavedPath = "";
        }

        void Load()
        {
            string path = lastSavedPath = EditorUtility.OpenFilePanel("Load Character Sheet", "Assets/", "asset");
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

        void SaveAs()
        {
            string path = lastSavedPath = EditorUtility.SaveFilePanelInProject("Save Character Sheet", "Char_" + sheet.forename + "_" + sheet.surname, "asset", "", "Assets/");
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

        //-----------------------------------------------------------------------------------------

        #region Header

        void CharacterSheet(Rect rect)
        {

            GUILayout.BeginArea(new Rect(16f, 16f, rect.width - 32f, 32f), styleBox);
            float halfWidth = (rect.width - 56f) / 2f;
            GUI.Label(new Rect(8f, 8f, halfWidth, 16f), "Character Sheet", styleHeader);
            GUI.Label(new Rect(16f + halfWidth, 8f, halfWidth, 16f), sheet.timestamp, styleTimestamp);
            GUILayout.EndArea();

            Profile(new Rect(16f, 56f, rect.width - 32f, 96f));
            Modifier(new Rect(16f, 160f, rect.width - 32f, 64f));
            ExtraSettings(new Rect(16f, 232f, rect.width - 32f, 96f));
            StorySheets(new Rect(16f, 336f, rect.width - 32f, 32f + 40f * sheet.storySheets.Count));
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Character Settings Methods

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

            GUILayout.EndArea();

            GUILayout.EndArea();
        }

        void Modifier(Rect rect)
        {
            GUILayout.BeginArea(rect, styleBox);

            float halfWidth = (rect.width - 32f) / 2f;

            GUI.Box(new Rect(0f, 0f, rect.width / 2f, rect.height), "", styleBox);
            GUI.Box(new Rect(rect.width / 2f, 0f, rect.width / 2f, rect.height), "", styleBox);

            GUI.Label(new Rect(8f, 8f, halfWidth, 16f), "Trust Stability", styleCenterLabel);
            sheet.trustStability = EditorGUI.Slider(new Rect(8f, 40f, halfWidth, 16f), sheet.trustStability * 100f, 0f, 100f) / 100f;

            GUI.Label(new Rect(24f + halfWidth, 8f, halfWidth, 16f), "Heart Modifier", styleCenterLabel);
            sheet.heartModifier = EditorGUI.Slider(new Rect(24f + halfWidth, 40f, halfWidth, 16f), sheet.heartModifier, 0.5f, 2f);
            
            GUILayout.EndArea();
        }

        #endregion

        #region Extra Settings Methods

        void ExtraSettings(Rect rect)
        {
            GUILayout.BeginArea(rect, styleBox);

            sheet.textColor = EditorGUI.ColorField(new Rect(8f, 8f, rect.width - 16f, 16f), "Text Color", sheet.textColor);
            sheet.playerPrefab = (GameObject)EditorGUI.ObjectField(new Rect(8f, 40f, rect.width - 16f, 16f), "Player Prefab", sheet.playerPrefab, typeof(GameObject), false);
            sheet.infoSheet = (InfoSheet)EditorGUI.ObjectField(new Rect(8f, 72f, rect.width - 16f, 16f), "Info Sheet", sheet.infoSheet, typeof(InfoSheet), false);

            GUILayout.EndArea();
        }

        #endregion

        #region Story Sheets Methods

        void StorySheets(Rect rect)
        {
            GUILayout.BeginArea(rect, styleBox);
            GUI.Label(new Rect(8f, 8f, rect.width - 16f, 16f), "Story Sheets");
            if (GUI.Button(new Rect(rect.width - 56f, 8f, 48f, 16f), "Add"))
            {
                sheet.storySheets.Add(null);
            }
            for (int s = 0; s < sheet.storySheets.Count; s++)
            {
                StorySheet(new Rect(8f, 32f + 40f * s, rect.width - 16f, 32f), s);
            }
            GUILayout.EndArea();
        }

        void StorySheet(Rect rect, int s)
        {
            GUILayout.BeginArea(rect, styleBox);
            sheet.storySheets[s] = (DialogCanvas)EditorGUI.ObjectField(new Rect(8f, 8f, rect.width - 64f, 16f), sheet.storySheets[s] == null ? "Story Sheet" : sheet.storySheets[s].canvasName, sheet.storySheets[s], typeof(DialogCanvas), false);
            if (GUI.Button(new Rect(rect.width - 48f, 8f, 40f, 16f), "-"))
            {
                removedStorySheet = s;
            }
            GUILayout.EndArea();
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Extra Fields

        void FillingTextField(Rect rect, ref string input, string placeholder)
        {
            input = GUI.TextField(rect, input, styleTextField);
            if (input == "")
            {
                GUI.Label(rect, placeholder, styleTextFieldEmpty);
            }
        }

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