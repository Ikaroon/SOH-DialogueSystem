using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    public class DialogEditor : EditorWindow
    {
        #region Constant Path Data

        private const string BG_IMAGE_PATH = "Assets/_DATA/Resources/DialogSystem/Images/Grid_EditorBG.png";
        public const string NODE_LIB_PATH = "Assets/Scripts/DialogSystem/DialogNodesLibrary";

        #endregion

        #region Editor Content

        public DialogCanvas canvas = null;
        public string oldPath = "";

        #endregion

        #region Editor Task Data

        public enum NodeEditorTask { None, ResizeSidebar, MoveNodeField };
        public NodeEditorTask currentTask = NodeEditorTask.None;

        #endregion

        #region Editor Visual Data

        private float sidebarWidth = 320f;
        private Vector2 nodeFieldScroll = Vector2.zero;
        private Vector2 anchorPoint = Vector2.zero;

        #endregion

        #region Style Data

        private GUIStyle nodeFieldBGStyle;
        Texture2D background;

        #endregion

        #region Style Methods

        private void InitializeStyles()
        {
            if (nodeFieldBGStyle != null)
            {
                return;
            }

            background = AssetDatabase.LoadAssetAtPath<Texture2D>(BG_IMAGE_PATH);

            nodeFieldBGStyle = new GUIStyle();

            nodeFieldBGStyle.onNormal.background = background;
            nodeFieldBGStyle.onHover.background = background;
            nodeFieldBGStyle.onFocused.background = background;
            nodeFieldBGStyle.onActive.background = background;
        }

        #endregion

        #region Window Methods

        //The initialization Method for the Window
        [MenuItem("SpyOnHuman/Dialog Editor")]
        static void Init()
        {
            DialogEditor window = (DialogEditor)EditorWindow.GetWindow(typeof(DialogEditor));
            window.Show();
        }

        private void OnEnable()
        {
            this.titleContent = new GUIContent("Dialog Editor");
            this.minSize = new Vector2(768f, 320f);
        }

        private void Update()
        {
            //Updating for performing Tasks
            DatabaseDrawer.UpdateDrawer();
        }

        //The GUI call for this Window
        void OnGUI()
        {
            //Initialize Styles
            InitializeStyles();

            //Display Toolbar
            ToolbarDrawer.DrawToolbar(new Rect(0f, 0f, this.position.width, 18f), this);

            //-----------------------------------------------------------------------------------------

            //Begin Bottom Part of Window
            GUILayout.BeginArea(new Rect(0f, 18f, this.position.width, this.position.height - 18f));

            //Display Node Editor Frame
            GUILayout.BeginArea(new Rect(sidebarWidth - 1f, 0f, this.position.width - (sidebarWidth - 1f), this.position.height - 16f));
            DrawNodeField(new Vector2((this.position.width - (sidebarWidth - 1f)) / 2f, (this.position.height - 16f) / 2f));
            GUILayout.EndArea();

            //Resize Sidebar and Display
            SidebarDrawer.DrawSidebar(new Rect(-1f, -1f, sidebarWidth, this.position.height - 16f), this, ref sidebarWidth, 240f, this.position.width / 2f);
            
            GUILayout.EndArea();

            //-----------------------------------------------------------------------------------------

            //Reset Events
            if (currentTask == NodeEditorTask.ResizeSidebar && Event.current.isMouse && Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                currentTask = NodeEditorTask.None;
            }
            if (currentTask == NodeEditorTask.MoveNodeField && Event.current.isMouse && Event.current.type == EventType.MouseUp && Event.current.button == 2)
            {
                currentTask = NodeEditorTask.None;
            }
        }

        #endregion

        #region Basic Node Field Drawer

        private void DrawNodeField(Vector2 center)
        {
            EditorGUI.BeginDisabledGroup(!canvas);

            Rect canvasRect = new Rect(-6000f + center.x + nodeFieldScroll.x, -6000f + center.y + nodeFieldScroll.y, 12000f, 12000f);

            //TODO: Scaling

            bool mouseInside = new Rect(Vector2.zero, center * 2f).Contains(Event.current.mousePosition);
            
            GUILayout.BeginArea(canvasRect);


            GUI.DrawTextureWithTexCoords(new Rect(0f, 0f, 12000f, 12000f), background, new Rect(0f, 0f, 12000f * background.texelSize.x * 2f, 12000f * background.texelSize.y * 2f));

            //----------------

            if (currentTask == DialogEditor.NodeEditorTask.None && Event.current.isMouse && Event.current.type == EventType.MouseDown && Event.current.button == 2 && mouseInside)
            {
                anchorPoint = Event.current.mousePosition;
                currentTask = DialogEditor.NodeEditorTask.MoveNodeField;
            }

            if (currentTask == DialogEditor.NodeEditorTask.MoveNodeField && Event.current.isMouse && Event.current.type == EventType.MouseDrag && Event.current.button == 2)
            {
                Vector2 moveVector = anchorPoint - Event.current.mousePosition;
                nodeFieldScroll = new Vector2(nodeFieldScroll.x - moveVector.x, nodeFieldScroll.y - moveVector.y);
                Repaint();
            }

            //----------

            if (canvas)
            {

                for (int n = 0; n < canvas.nodes.Count; n++)
                {
                    if (NodeDrawer.IsVisible(canvasRect, new Rect(canvasRect.position * -1f, center * 2f), canvas.nodes[n]))
                    {
                        bool test = false;
                        NodeDrawer.DrawNode(canvasRect, canvas.nodes[n], ref test, this);
                    }
                }

                //if (NodeDrawer.IsVisible(canvasRect, new Rect(canvasRect.position * -1f, center * 2f), canvas.nodes[n]))
                {
                    bool test = false;
                    NodeDrawer.DrawConnections(canvasRect, canvas.connections, ref test);
                }

                if (canvas && currentTask == DialogEditor.NodeEditorTask.None && Event.current.isMouse && Event.current.type == EventType.MouseDown && Event.current.button == 1 && mouseInside)
                {
                    DrawCreateNodeMenu(Event.current.mousePosition - new Vector2(6000f, 6000f));
                }

                Repaint();
                //TODO: Node Field GUI
            }
            
            GUILayout.EndArea();

            EditorGUI.EndDisabledGroup();

        }

        #endregion

        #region Create Node Menu Methods

        private void DrawCreateNodeMenu(Vector2 createPos)
        {
            GenericMenu menu = new GenericMenu();

            AddNodesToMenu(ref menu, createPos);

            menu.ShowAsContext();
        }

        public void AddNodesToMenu(ref GenericMenu menu, Vector2 createPos)
        {
            Dictionary<string, System.Type> nodes = NodeOperator.CollectNodeTypes(NODE_LIB_PATH);

            foreach (KeyValuePair<string, System.Type> pair in nodes)
            {
                menu.AddItem(new GUIContent(pair.Key), false, CreateNode, new TypePosPack(pair.Value, createPos));
            }
        }

        public void AddDisabledNodesToMenu(ref GenericMenu menu)
        {
            Dictionary<string, System.Type> nodes = NodeOperator.CollectNodeTypes(NODE_LIB_PATH);

            foreach (KeyValuePair<string, System.Type> pair in nodes)
            {
                menu.AddDisabledItem(new GUIContent(pair.Key));
            }
        }

        private void CreateNode(object obj)
        {
            TypePosPack pair = obj as TypePosPack;
            canvas.nodes.Add(Node.CreateNode(pair.type, pair.pos));
        }

        private class TypePosPack {

            public System.Type type;
            public Vector2 pos;

            public TypePosPack(System.Type type, Vector2 pos)
            {
                this.type = type;
                this.pos = pos;
            }

        }

        #endregion

    }
}