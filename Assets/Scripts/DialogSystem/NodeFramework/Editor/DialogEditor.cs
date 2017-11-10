using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using Northwind.Essentials;

using SpyOnHuman.DialogSystem.LanguageSystem;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    public class DialogEditor : EditorWindow
    {
        #region Constant Path Data
        
        private const string NODE_LIB_PATH = "Assets/Scripts/DialogSystem/DialogNodesLibrary";

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Editor Content

        public DialogCanvas canvas = null;
        public string oldPath = "";

        #endregion

        #region Editor Visual Data

        private Vector2 nodeFieldScroll = Vector2.zero;

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Style Data

        private bool stylesLoaded = false;

        #endregion

        #region Style Methods

        private void InitializeStyleData()
        {
            if (stylesLoaded)
            {
                return;
            }

            InitializeToolbarStyles();
            InitializeSidebarStyles();
            InitiateNodeStyles();
            InitiateHandleStyles();

            stylesLoaded = true;
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Task System Data

        private enum TaskType { None = 0,                   // No task given

                                ResizeSidebar = 1,          // Resize the Sidebar

                                MoveNodeField = 2,          // Move the entire NodeField

                                MoveNode = 3,               // Move a single Node

                                ResizeNodeUpperLeft = 4,    // Resize a single Node on the upper left corner
                                ResizeNodeUpperRight = 5,   // Resize a single Node on the upper right corner
                                ResizeNodeBottomRight = 6,  // Resize a single Node on the bottom right corner
                                ResizeNodeBottomLeft = 7,   // Resize a single Node on the bottom left corner

                                ConnectAnOutput = 8,        // An Output was selected and waits for the connection
                                ConnectAnInput = 9,         // An Input was selected and waits for the connection
                                CreateConnectionOutput = 10,// An Output was selected and an Input afterwards -> prepared for connection
                                CreateConnectionInput = 11, // An Input was selected and an Output afterwards -> prepared for connection

                                DeleteNode = 12,            // A Node was marked for delete and waits for the deletion now
                                DeleteConnection = 13,      // A Connection was marked for delete and waits for the deletion now
                                DeleteHandle = 14           // A Handle was marked for delete and waits for the deletion now

                                };

        private TaskType task = TaskType.None;

        private object taskObject;

        private bool mouseActionUsed = false;

        #endregion

        #region Task System Methods

        private bool MouseActionInRect(Rect rect, EventType eventType, MouseButton button)
        {
            Event current = Event.current;

            return (!mouseActionUsed &&
                current.isMouse &&
                current.button == (int)button &&
                rect.Contains(current.mousePosition) &&
                current.type == eventType) ;
        }

        private bool MouseAction(EventType eventType, MouseButton button)
        {
            Event current = Event.current;
            
            return (!mouseActionUsed && 
                current.isMouse &&
                current.button == (int)button &&
                current.type == eventType);
        }

        private void RegisterTask(TaskType taskType, object taskObj)
        {
            if (task == TaskType.None)
            {
                task = taskType;
                taskObject = taskObj;
            }
        }

        private System.Action OnResetTask, OnCompleteTask;

        #endregion

        //-----------------------------------------------------------------------------------------

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

        private void OnLostFocus()
        {
            task = TaskType.None;
        }

        private void Update()
        {
            //Updating for performing Tasks
            DatabaseDrawer.UpdateDrawer();
            nodeFieldScroll = VectorMath.Round(nodeFieldScroll);

            if (OnCompleteTask != null)
            {
                OnCompleteTask.Invoke();
            }
        }

        //The GUI call for this Window
        void OnGUI()
        {
            string oldFocus = GUI.GetNameOfFocusedControl();

            //Initialize Styles
            InitializeStyleData();

            //Display Toolbar
            DrawToolbar(new Rect(0f, 0f, this.position.width, 18f));

            //-----------------------------------------------------------------------------------------

            //Begin Bottom Part of Window
            Rect bottomRect = new Rect(new Rect(0f, 18f, this.position.width, this.position.height - 18f));
            GUILayout.BeginArea(bottomRect);

            //Display Node Editor Frame
            DrawNodeField(new Rect(sidebarWidth - 1f, 0f, bottomRect.width - (sidebarWidth - 1f), bottomRect.height));

            //Resize Sidebar and Display
            DrawSidebar(new Rect(-1f, -1f, sidebarWidth, bottomRect.height));
            
            GUILayout.EndArea();

            //-----------------------------------------------------------------------------------------

            //Reset Events
            
            //End the Group of the Editor to check all events even outside of the window
            GUI.EndGroup();
            HandleEverEvent();
            if (OnResetTask != null)
            {
                OnResetTask.Invoke();
            }
            //Begin the Window again
            GUI.BeginGroup(new Rect(0, 21, Screen.width, Screen.height));

            if (mouseActionUsed)
            {
                GUI.SetNextControlName("");
                GUI.FocusControl("");
            }
            mouseActionUsed = false;

            if (oldFocus == GUI.GetNameOfFocusedControl()
                && MouseActionInRect(new Rect(Vector2.zero, position.size), EventType.mouseDown, MouseButton.Left))
            {
                GUI.SetNextControlName("");
                GUI.FocusControl("");
                Repaint();
            }

            //Generel Events
        }

        #endregion

        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //----------------------------------< DRAWERS METHODS >----------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        // Here are all Drawer Methods which are used to draw the editor

        //---------------------------------------------------------------------------------------\\
        //---------------------------------< NODEFIELD METHODS >---------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        #region Node Field Drawer

        private void DrawNodeField(Rect rect)
        {
            GUILayout.BeginArea(rect);
            
            //Calculate Views
            Vector2 center = VectorMath.Round(rect.size / 2f);
            Rect displayRect = new Rect(Vector2.zero, center * 2f);


            //Calculate the offset for the node placement
            Vector2 positionOffset = new Vector2(-1f + displayRect.width / 2f + nodeFieldScroll.x,
                                        1f + displayRect.height / 2f + nodeFieldScroll.y);

            DrawGrid(displayRect.size, positionOffset, 16f, new Color(0.1f, 0.1f, 0.1f, 0.2f), new Color(0f, 0.7f, 0f, 0.4f));

            DrawCanvas(displayRect, positionOffset);

            GUILayout.EndArea();
        }

        private void DrawGrid(Vector2 size, Vector2 offset, float gridSize, Color gridColor, Color centerColor)
        {
            Color oldColor = Handles.color;
            Handles.color = gridColor;

            Vector2 offsetAddition = new Vector2(offset.x % gridSize, offset.y % gridSize);
            for (int x = 0; x < (size.x / gridSize); x++)
            {
                Handles.DrawAAPolyLine(4f,  new Vector3(x * gridSize + offsetAddition.x, 0f, 0f),
                                            new Vector3(x * gridSize + offsetAddition.x, size.y, 0f));
            }
            for (int y = 0; y < (size.y / gridSize); y++)
            {
                Handles.DrawAAPolyLine(4f,  new Vector3(0f, y * gridSize + offsetAddition.y, 0f),
                                            new Vector3(size.x, y * gridSize + offsetAddition.y, 0f));
            }

            Handles.color = centerColor;

            Handles.DrawAAPolyLine(6f,  new Vector3(offset.x, 0f, 0f),
                                        new Vector3(offset.x, size.y, 0f));
            Handles.DrawAAPolyLine(6f, new Vector3(0f, offset.y, 0f),
                                        new Vector3(size.x, offset.y, 0f));

            DrawOffscreenArrows(size, offset);

            Handles.color = oldColor;
        }

        private void DrawOffscreenArrows(Vector2 size, Vector2 offset)
        {
            //Horizontal Arrows
            if (offset.x < 0f)
            {
                Handles.DrawAAPolyLine(4f, new Vector3(16f, offset.y + 16f, 0f), new Vector3(0f, offset.y, 0f), new Vector3(16f, offset.y - 16f, 0f));
            }
            if (offset.x > size.x)
            {
                Handles.DrawAAPolyLine(4f, new Vector3(size.x - 16f, offset.y + 16f, 0f), new Vector3(size.x, offset.y, 0f), new Vector3(size.x - 16f, offset.y - 16f, 0f));
            }

            //Vertical Arrows
            if (offset.y < 0f)
            {
                Handles.DrawAAPolyLine(4f, new Vector3(offset.x - 16f, 16f, 0f), new Vector3(offset.x, 0f, 0f), new Vector3(offset.x + 16f, 16f, 0f));
            }
            if (offset.y > size.y)
            {
                Handles.DrawAAPolyLine(4f, new Vector3(offset.x + 16f, size.y - 16f, 0f), new Vector3(offset.x, size.y, 0f), new Vector3(offset.x - 16f, size.y - 16f, 0f));
            }
        }

        private void DrawCanvas(Rect displayRect, Vector2 positionOffset)
        {
            //If no canvas is available to draw then abort
            if (!canvas)
            {
                return;
            }

            //Iterate over all Nodes and draw them
            for (int n = 0; n < canvas.nodes.Count; n++)
            {
                if (IsVisible(positionOffset, displayRect, canvas.nodes[n]))
                {
                    DrawNode(positionOffset, canvas.nodes[n]);
                }

                DrawConnections(positionOffset, displayRect, canvas.connections);
            }

            //Events
            CreateNodeEvent(displayRect);
            MoveFieldEvent(displayRect);
        }

        #endregion

        #region Node Field Events

        private void CreateNodeEvent(Rect displayRect)
        {
            if (task == TaskType.None && MouseActionInRect(displayRect, EventType.MouseDown, MouseButton.Right))
            {
                DrawCreateNodeMenu(displayRect);
                mouseActionUsed = true; //Disable all other mouse events from now on!
            }
        }

        private void MoveFieldEvent(Rect displayRect)
        {
            Rect calculationRect = new Rect(-1f + displayRect.width / 2f + nodeFieldScroll.x,
                                            1f + displayRect.height / 2f + nodeFieldScroll.y,
                                            2f, 2f);

            if (MouseActionInRect(displayRect, EventType.mouseDown, MouseButton.Middle))// && mouseIsInside)
            {
                GUI.BeginGroup(calculationRect);
                RegisterTask(TaskType.MoveNodeField, Event.current.mousePosition);
                GUI.EndGroup();

                OnResetTask = ResetNodeFieldEvents;
                mouseActionUsed = true; //Disable all other mouse events from now on!
            }
            else if (task == TaskType.MoveNodeField && MouseAction(EventType.MouseDrag, MouseButton.Middle))
            {
                GUI.BeginGroup(calculationRect);
                Vector2 moveVector = ((Vector2)taskObject) - Event.current.mousePosition;
                nodeFieldScroll = new Vector2(nodeFieldScroll.x - moveVector.x, nodeFieldScroll.y - moveVector.y);
                GUI.EndGroup();

                mouseActionUsed = true; //Disable all other mouse events from now on!
                Repaint();
            }
        }

        private void ResetNodeFieldEvents()
        {
            if (task == TaskType.MoveNodeField && MouseAction(EventType.MouseUp, MouseButton.Middle))
            {
                task = TaskType.None;
                mouseActionUsed = true; //Disable all other mouse events from now on!
            }
        }

        #endregion

        #region Create Node Menu Methods

        private void DrawCreateNodeMenu(Rect displayRect)
        {
            Rect calculationRect = new Rect(-1f + displayRect.width / 2f + nodeFieldScroll.x,
                                            1f + displayRect.height / 2f + nodeFieldScroll.y,
                                            2f, 2f);

            GUI.BeginGroup(calculationRect);
            Vector2 createPos = VectorMath.Step(Event.current.mousePosition, 1f);
            GUI.EndGroup();

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



        //---------------------------------------------------------------------------------------\\
        //----------------------------------< TOOLBAR METHODS >----------------------------------\\ 
        //---------------------------------------------------------------------------------------\\
        
        #region Style Data

        private GUIStyle toolbarStyle, toolbarButtonStyle, toolbarDropdownStyle, toolbarTextfieldStyle;

        #endregion

        #region Style Methods

        private void InitializeToolbarStyles()
        {
            toolbarStyle = GUI.skin.GetStyle("Toolbar");

            toolbarButtonStyle = GUI.skin.GetStyle("toolbarbutton");

            toolbarDropdownStyle = GUI.skin.GetStyle("ToolbarDropDown");

            toolbarTextfieldStyle = GUI.skin.GetStyle("ToolbarTextField");
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Toolbar Drawer

        private void DrawToolbar(Rect rect)
        {
            //Menus Toolbar
            GUILayout.BeginArea(new Rect(rect.x, rect.y, rect.width - 128f, 18f), toolbarStyle);
            GUILayout.BeginHorizontal();

            //Data Menu Button
            if (GUILayout.Button(new GUIContent("Data"), toolbarButtonStyle, GUILayout.Width(100f)))
            {
                ToolbarDataMenu(new Rect(7f, 18f, 0f, 0f));
            }

            //Edit Menu Button
            if (GUILayout.Button(new GUIContent("Edit"), toolbarButtonStyle, GUILayout.Width(100f)))
            {
                ToolbarEditMenu(new Rect(107f, 18f, 0f, 0f));
            }

            //Add Menu Button
            if (GUILayout.Button(new GUIContent("Add"), toolbarButtonStyle, GUILayout.Width(100f)))
            {
                ToolbarAddMenu(new Rect(207f, 18f, 0f, 0f));
            }

            EditorGUI.BeginDisabledGroup(!canvas);

            EditorGUILayout.Space();

            if (canvas)
            {
                EditorGUILayout.LabelField(new GUIContent("Name"), GUILayout.Width(50f));
                canvas.canvasName = EditorGUILayout.TextField(canvas.canvasName, toolbarTextfieldStyle, GUILayout.MaxWidth(200f));

                EditorGUILayout.LabelField(new GUIContent("Description"), GUILayout.Width(80f));
                canvas.canvasDescription = EditorGUILayout.TextField(canvas.canvasDescription, toolbarTextfieldStyle, GUILayout.MaxWidth(300f));
            }
            else
            {
                EditorGUILayout.LabelField(new GUIContent("Name"), GUILayout.Width(50f));
                EditorGUILayout.TextField("", toolbarTextfieldStyle, GUILayout.MaxWidth(200f));

                EditorGUILayout.LabelField(new GUIContent("Description"), GUILayout.Width(80f));
                EditorGUILayout.TextField("", toolbarTextfieldStyle, GUILayout.MaxWidth(300f));
            }

            EditorGUILayout.Space();

            EditorGUI.EndDisabledGroup();

            //Add new Menus when needed

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            //-----------------------------------------------------------------------------------------

            //Language Toolbar
            GUILayout.BeginArea(new Rect(rect.x + rect.width - 128f, rect.y, 128f, 18f), toolbarStyle);

            if (GUI.Button(new Rect(0f, 0f, 120f, 18f), new GUIContent("[" + LangSys.activeLang + "] " + LangSys.DATA[LangSys.activeLang].fullname), toolbarDropdownStyle))
            {
                ToolbarLanguageMenu(new Rect(0f, 18f, 0f, 0f));
            }

            GUILayout.EndArea();
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Data Menu

        private void ToolbarDataMenu(Rect rect)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("New"), false, New);
            menu.AddItem(new GUIContent("Load"), false, Load);

            if (canvas)
            {
                menu.AddItem(new GUIContent("Save"), false, Save);
                menu.AddItem(new GUIContent("Save As"), false, SaveAs);
                menu.AddItem(new GUIContent("Export JSON"), false, ExportJSON);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Save"));
                menu.AddDisabledItem(new GUIContent("Save As"));
                menu.AddDisabledItem(new GUIContent("Export JSON"));
            }

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Settings"), false, Settings);
            menu.DropDown(rect);
        }

        #endregion

        #region Data Menu Methods

        private void New()
        {
            canvas = DialogCanvas.CreateCanvas<DialogCanvas>();
            oldPath = "";
        }

        private void Load()
        {
            oldPath = NodeSaveOperator.Load(ref canvas);
        }

        private void Save()
        {
            oldPath = NodeSaveOperator.Save(ref canvas, oldPath);
        }

        private void SaveAs()
        {
            oldPath = NodeSaveOperator.Save(ref canvas, oldPath);
        }

        private void ExportJSON()
        {
            NodeSaveOperator.Export(ref canvas, "");
        }

        private static void Settings()
        {
            //TODO: Open Settings Window
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Edit Menu

        private void ToolbarEditMenu(Rect rect)
        {
            GenericMenu menu = new GenericMenu();

            if (canvas)
            {
                menu.AddItem(new GUIContent("Smart Align"), false, SmartAlign);
                menu.AddItem(new GUIContent("Improve"), false, Improve);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Smart Align"));
                menu.AddDisabledItem(new GUIContent("Improve"));
            }

            menu.DropDown(rect);
        }

        #endregion

        #region Edit Menu Methods

        private void SmartAlign()
        {
            //TODO: Smart Align Tool
        }

        private void Improve()
        {
            //TODO: Improve Tool
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Add Menu

        private void ToolbarAddMenu(Rect rect)
        {
            GenericMenu menu = new GenericMenu();

            if (canvas)
            {
                AddNodesToMenu(ref menu, Vector2.zero);
            }
            else
            {
                AddDisabledNodesToMenu(ref menu);
            }

            menu.DropDown(rect);
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Language Menu

        private void ToolbarLanguageMenu(Rect rect)
        {
            GenericMenu menu = new GenericMenu();

            foreach (KeyValuePair<string, Language> pair in LangSys.DATA.languages)
            {
                menu.AddItem(new GUIContent("[" + pair.Key + "] " + pair.Value.fullname), false, ChangeLanguage, pair.Key);
            }

            menu.DropDown(rect);
        }

        #endregion

        #region Language Menu Methods

        private void ChangeLanguage(object obj)
        {
            LangSys.activeLang = obj as string;
            mouseActionUsed = true;
        }

        #endregion



        //---------------------------------------------------------------------------------------\\
        //----------------------------------< SIDEBAR METHODS >----------------------------------\\ 
        //---------------------------------------------------------------------------------------\\
        
        #region Style Data

        private GUIStyle groupBoxStyle;

        #endregion

        #region Style Methods

        private void InitializeSidebarStyles()
        {
            groupBoxStyle = GUI.skin.GetStyle("GroupBox");
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Sidebar Data

        private float sidebarWidth = 320f;
        private Vector2 sidebarWidthconstraints = new Vector2(300f, 500f);

        #endregion

        #region Sidebar Drawer

        public void DrawSidebar(Rect rect)
        {
            //Resize
            rect = new Rect(rect.x, rect.y, sidebarWidth, rect.height);

            //-----------------------------------------------------------------------------------------
            // GUI Drawing

            //Sidebar
            GUILayout.BeginArea(rect, groupBoxStyle);

            //Draw Content
            DatabaseDrawer.DrawDatabase(new Rect(0f, 0f, rect.width - 4f, rect.height), false);

            //-----------------------------------------------------------------------------------------
            // Events

            ToolbarEvents(rect);

            //-----------------------------------------------------------------------------------------

            GUILayout.EndArea();
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Toolbar Events

        private void ToolbarEvents(Rect rect)
        {
            //Mouse Zone
            Rect resizeRect = new Rect(sidebarWidth - 4f, 0f, 4f, rect.height);
            EditorGUIUtility.AddCursorRect(resizeRect, MouseCursor.ResizeHorizontal);

            //Resize Events
            if (MouseActionInRect(resizeRect, EventType.MouseDown, MouseButton.Left))
            {
                RegisterTask(TaskType.ResizeSidebar, (float)(Event.current.mousePosition.x - sidebarWidth));
                OnResetTask = ResetToolbarEvents;
                mouseActionUsed = true; //Disable all other mouse events from now on!
            }
            else if (task == TaskType.ResizeSidebar && MouseAction(EventType.MouseDrag, MouseButton.Left))
            {
                sidebarWidth = Mathf.Clamp(Event.current.mousePosition.x - ((float)taskObject),
                    sidebarWidthconstraints.x, sidebarWidthconstraints.y);
                mouseActionUsed = true; //Disable all other mouse events from now on!
                Repaint();
            }
            sidebarWidth = Mathf.Clamp(sidebarWidth, sidebarWidthconstraints.x, sidebarWidthconstraints.y);
        }

        private void ResetToolbarEvents()
        {
            if (task == TaskType.ResizeSidebar && MouseAction(EventType.MouseUp, MouseButton.Left))
            {
                task = TaskType.None;
                mouseActionUsed = true; //Disable all other mouse events from now on!
            }
        }

        #endregion



        //---------------------------------------------------------------------------------------\\
        //-----------------------------------< NODES METHODS >-----------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        #region Style Data

        private static GUIStyle nodeBGStyle, nodeHeaderStyle;

        #endregion

        #region Style Methods

        private static void InitiateNodeStyles()
        {
            nodeBGStyle = GUI.skin.GetStyle("ChannelStripBg");

            nodeHeaderStyle = new GUIStyle(GUI.skin.GetStyle("HeaderLabel"));
            nodeHeaderStyle.alignment = TextAnchor.MiddleCenter;
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Node Helper Classes

        private class NodeEventPackage {

            public Node node;
            public Vector2 anchor;

            public NodeHandleAttribute attribute;
            public FieldInfo info;

        }

        #endregion

        #region Node Helper Methods

        private bool IsNodeBlocked(Node node)
        {
            switch (task)
            {
                case TaskType.MoveNode:                 return ((NodeEventPackage)taskObject).node == node;

                case TaskType.ResizeNodeBottomLeft:     return ((NodeEventPackage)taskObject).node == node;
                case TaskType.ResizeNodeBottomRight:    return ((NodeEventPackage)taskObject).node == node;
                case TaskType.ResizeNodeUpperLeft:      return ((NodeEventPackage)taskObject).node == node;
                case TaskType.ResizeNodeUpperRight:     return ((NodeEventPackage)taskObject).node == node;

                case TaskType.ConnectAnInput:           return ((NodeEventPackage)taskObject).node == node;
                case TaskType.ConnectAnOutput:          return ((NodeEventPackage)taskObject).node == node;
            }
            return false;
        }

        public bool IsVisible(Vector2 offset, Rect viewRect, Node node)
        {
            if (IsNodeBlocked(node))
            {
                return true;
            }

            Rect nodeRect = new Rect(offset + node.position - node.size / 2f, node.size);
            
            return viewRect.Overlaps(nodeRect);
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Node Visual Data

        private float nodeHandleWidth = 18f;

        #endregion

        #region Node Drawer

        public void DrawNode(Vector2 offset, Node node)
        {
            object[] attributes = node.GetType().GetCustomAttributes(false);
            NodeDataAttribute nodeData = new NodeDataAttribute("", "", 0f, 0f);
            foreach (object attribute in attributes)
            {
                if (attribute.GetType() == typeof(NodeDataAttribute))
                {
                    nodeData = attribute as NodeDataAttribute;
                }
            }

            DrawNodeInternal(offset, node, nodeData);
        }

        private void DrawNodeInternal(Vector2 offset, Node node, NodeDataAttribute nodeData)
        {
            Rect nodeRect = new Rect(offset + node.position - node.size / 2f + new Vector2(-nodeHandleWidth, 0f), node.size + new Vector2(nodeHandleWidth * 2f, 0f));
            GUILayout.BeginArea(nodeRect);

            GUILayout.BeginArea(new Rect(nodeHandleWidth, 0f, nodeRect.width - nodeHandleWidth * 2f, nodeRect.height), nodeBGStyle);
            Rect headerRect = new Rect(0f, 0f, nodeRect.width - nodeHandleWidth * 2f, 22f);
            GUI.Label(headerRect, new GUIContent(nodeData.nodeName), nodeHeaderStyle);

            NodeContextEvent(node, headerRect);
            NodeMoveEvent(node, headerRect);

            //Draw content of node
            GUILayout.BeginArea(new Rect(8f, 30f, node.size.x - 16f, node.size.y - 38f));
            Editor editor = Editor.CreateEditor(node);
            INodeInspector nodeEditor = (INodeInspector)editor;
            if (nodeEditor != null)
            {
                nodeEditor.OnDrawNodeGUI(new Rect(0f, 0f, node.size.x - 16f, node.size.y - 38f));
            }
            else
            {
                editor.OnInspectorGUI();
            }
            GUILayout.EndArea();


            GUILayout.EndArea();

            DrawHandles(node, new Vector2(nodeRect.width - nodeHandleWidth * 2f, nodeRect.height), ConnectionType.Input);
            DrawHandles(node, new Vector2(nodeRect.width - nodeHandleWidth * 2f, nodeRect.height), ConnectionType.Output);

            GUILayout.EndArea();
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Node Context Menu

        private void NodeContextMenu(Node node)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Delete"), false, DeleteNode, node);

            menu.ShowAsContext();
        }

        #endregion

        #region Node Context Menu Methods

        private void DeleteNode(object obj)
        {
            RegisterTask(TaskType.DeleteNode, (Node)obj);
            OnResetTask = CompleteDeleteNodeTask;
        }

        private void CompleteDeleteNodeTask()
        {
            if (task == TaskType.DeleteNode)
            {
                Node node = (Node)taskObject;

                //Remove left Connections
                List<NodeHandlePackage> inputHandles = NodeOperator.GetConnections(node, ConnectionType.Input);

                for (int ih = 0; ih < inputHandles.Count; ih++)
                {
                    int connection = GetCurrentConnection(node, inputHandles[ih].handle);

                    if (connection != -1)
                    {
                        canvas.connections[connection].DiscardConnection();
                        canvas.connections.RemoveAt(connection);
                    }
                }

                List<NodeHandlePackage> outputHandles = NodeOperator.GetConnections(node, ConnectionType.Output);

                for (int oh = 0; oh < outputHandles.Count; oh++)
                {
                    int connection = GetCurrentConnection(node, outputHandles[oh].handle);

                    if (connection != -1)
                    {
                        if (canvas.connections[connection].DiscardFromConnection(node, outputHandles[oh].handle.handlePosition))
                        {
                            canvas.connections.RemoveAt(connection);
                        }
                    }
                }

                //Remove Node

                canvas.nodes.Remove(node);
                task = TaskType.None;
                Repaint();
            }
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Node Events

        private void NodeContextEvent(Node node, Rect rect)
        {
            if (MouseActionInRect(rect, EventType.mouseDown, MouseButton.Right))
            {
                NodeContextMenu(node);
                mouseActionUsed = true; //Disable all other mouse events from now on!
            }
        }

        private void NodeMoveEvent(Node node, Rect rect)
        {
            if (MouseActionInRect(rect, EventType.mouseDown, MouseButton.Left))
            {
                NodeEventPackage eventPackage = new NodeEventPackage();
                eventPackage.node = node;
                eventPackage.anchor = Event.current.mousePosition;
                RegisterTask(TaskType.MoveNode, eventPackage);
                OnResetTask = ResetNodeMoveEvent;
                mouseActionUsed = true; //Disable all other mouse events from now on!
            }
            else if (task == TaskType.MoveNode && MouseAction(EventType.mouseDrag, MouseButton.Left) && ((NodeEventPackage)taskObject).node == node)
            {
                Vector2 anchorPoint = ((NodeEventPackage)taskObject).anchor;
                node.position = new Vector2(node.position.x + (Event.current.mousePosition.x - anchorPoint.x),
                                            node.position.y + (Event.current.mousePosition.y - anchorPoint.y));
                node.position = VectorMath.Step(node.position, 16f);
                mouseActionUsed = true; //Disable all other mouse events from now on!
                Repaint();
            }
        }

        private void ResetNodeMoveEvent()
        {
            if (task == TaskType.MoveNode && MouseAction(EventType.MouseUp, MouseButton.Left))
            {
                task = TaskType.None;
                mouseActionUsed = true; //Disable all other mouse events from now on!
            }
        }

        #endregion



        //---------------------------------------------------------------------------------------\\
        //----------------------------------< HANDLES METHODS >----------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        #region Style Data

        private static GUIStyle handleBGStyle, handleFGstyle;

        #endregion

        #region Style Methods

        private static void InitiateHandleStyles()
        {
            handleBGStyle = new GUIStyle(GUI.skin.GetStyle("SearchCancelButtonEmpty"));

            handleFGstyle = new GUIStyle(GUI.skin.GetStyle("ColorPicker2DThumb"));
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Handle Drawer

        private void DrawHandles(Node node, Vector2 size, ConnectionType type)
        {
            List<NodeHandlePackage> handles = NodeOperator.GetConnections(node, type);

            Vector2 anchorPos = Vector2.zero;
            switch (type)
            {
                case ConnectionType.Input: anchorPos = new Vector2(nodeHandleWidth, 0f); break;
                case ConnectionType.Output: anchorPos = new Vector2(nodeHandleWidth + size.x, 0f); break;
            }

            foreach (NodeHandlePackage handle in handles)
            {
                MouseEventPackage mouseEventPackage = DrawHandle(type, anchorPos + handle.handle.handlePosition, handle.info.GetValue(node) != null);

                //Handle Events
                HandleContextEvent(mouseEventPackage, node, handle.handle, handle.info);
                HandleEventConnect(mouseEventPackage, node, handle.handle, handle.info);
            }
        }

        public MouseEventPackage DrawHandle(ConnectionType type, Vector2 pos, bool filled)
        {
            MouseEventPackage mouseEvent = new MouseEventPackage(EventType.Ignore, MouseButton.None);
            Rect rect = new Rect(pos, new Vector2(nodeHandleWidth, nodeHandleWidth));

            //Change Direction of Field
            Matrix4x4 guiOriginalMatrix = GUI.matrix;
            GUIUtility.ScaleAroundPivot(new Vector2(type == ConnectionType.Output ? 1f : -1f, 1f), pos);

            //Begin the handle drawing
            GUILayout.BeginArea(rect, handleBGStyle);

            //Draw the anchor in the middle
            GUI.Label(new Rect(4f, 5f, 8f, 8f), new GUIContent(), handleFGstyle);
            if (filled)
            {
                //Fill the anchor if filled
                Handles.DrawSolidDisc(new Vector3(8f, 9f, 0f), Vector3.forward, 2f);
            }

            //Check for Mouse event:
            Rect mouseRect = new Rect(0f, 0f, rect.width, rect.height);
            if (Event.current.isMouse && mouseRect.Contains(Event.current.mousePosition))
            {
                mouseEvent.eventType = Event.current.type;
                mouseEvent.mouseButton = (MouseButton)Event.current.button;
            }
            GUILayout.EndArea();

            GUI.matrix = guiOriginalMatrix;

            return mouseEvent;
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Handle Context Menu

        private void HandleContextMenu(Node node, NodeHandleAttribute attribute, FieldInfo info)
        {
            NodeEventPackage eventPackage = new NodeEventPackage();
            eventPackage.node = node;
            eventPackage.attribute = attribute;
            eventPackage.info = info;

            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete Connection"), false, ClearHandle, eventPackage);
            menu.ShowAsContext();
        }

        #endregion

        #region Handle Context Menu Methods

        private void ClearHandle(object obj)
        {
            NodeEventPackage eventPackage = (NodeEventPackage)obj;
            RegisterTask(TaskType.DeleteHandle, eventPackage);
            OnResetTask = CompleteClearHandle;
        }

        private void CompleteClearHandle()
        {
            if (task == TaskType.DeleteHandle)
            {
                NodeEventPackage eventPackage = (NodeEventPackage)taskObject;
                int connection = GetCurrentConnection(eventPackage.node, eventPackage.attribute);
                if (eventPackage.attribute.handleType == ConnectionType.Input)
                {
                    if (connection != -1)
                    {
                        canvas.connections[connection].DiscardConnection();
                        canvas.connections.RemoveAt(connection);
                    }
                }
                else if (eventPackage.attribute.handleType == ConnectionType.Output)
                {
                    if (connection != -1)
                    {
                        if (canvas.connections[connection].DiscardFromConnection(eventPackage.node, eventPackage.attribute.handlePosition))
                        {
                            canvas.connections.RemoveAt(connection);
                        }
                    }
                }
                task = TaskType.None;
                Repaint();
            }
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Handle Events

        private void HandleContextEvent(MouseEventPackage inputPackage, Node node, NodeHandleAttribute attribute, FieldInfo info)
        {
            if (task == TaskType.None && inputPackage.eventType == EventType.mouseDown && inputPackage.mouseButton == MouseButton.Right)
            {
                HandleContextMenu(node, attribute, info);
                mouseActionUsed = true; //Disable all other mouse events from now on!
            }
        }
        
        private void HandleEventConnect(MouseEventPackage inputPackage, Node node, NodeHandleAttribute attribute, FieldInfo info)
        {
            if (task == TaskType.None && inputPackage.eventType == EventType.mouseDown && inputPackage.mouseButton == MouseButton.Left)
            {
                NodeEventPackage eventPackage = new NodeEventPackage();
                eventPackage.node = node;
                eventPackage.anchor = Event.current.mousePosition;
                eventPackage.attribute = attribute;
                eventPackage.info = info;

                switch (attribute.handleType)
                {
                    case ConnectionType.Input:
                        {
                            RegisterTask(TaskType.ConnectAnInput, eventPackage);
                        }
                        break;
                    case ConnectionType.Output:
                        {
                            RegisterTask(TaskType.ConnectAnOutput, eventPackage);
                        }
                        break;
                }

                OnResetTask = ResetHandleConnect;
                mouseActionUsed = true; //Disable all other mouse events from now on!
            }
            else if (inputPackage.eventType == EventType.mouseUp && inputPackage.mouseButton == MouseButton.Left)
            {
                switch (task)
                {
                    case TaskType.ConnectAnInput:
                        if (attribute.handleType == ConnectionType.Output && node != ((NodeEventPackage)taskObject).node)
                        {
                            task = TaskType.None;
                            mouseActionUsed = true;

                            ConnectionChangePackage changePack = new ConnectionChangePackage();

                            changePack.package = ((NodeEventPackage)taskObject);
                            changePack.node = node;
                            changePack.attribute = attribute;
                            changePack.info = info;

                            RegisterTask(TaskType.CreateConnectionInput, changePack);
                            OnResetTask = ApplyConnectInput;
                        }
                        break;
                    case TaskType.ConnectAnOutput:
                        if (attribute.handleType == ConnectionType.Input && node != ((NodeEventPackage)taskObject).node)
                        {
                             task = TaskType.None;
                            mouseActionUsed = true;

                            ConnectionChangePackage changePack = new ConnectionChangePackage();

                            changePack.package = ((NodeEventPackage)taskObject);
                            changePack.node = node;
                            changePack.attribute = attribute;
                            changePack.info = info;

                            RegisterTask(TaskType.CreateConnectionOutput, changePack);
                            OnResetTask = ApplyConnectOutput;
                        }
                        break;
                }
            }
        }

        private void HandleEverEvent()
        {
            if (task == TaskType.ConnectAnInput || task == TaskType.ConnectAnOutput)
            {
                if (MouseAction(EventType.mouseDrag, MouseButton.Left))
                {
                    Repaint();
                }
            }
        }

        private void ResetHandleConnect()
        {
            if ((task == TaskType.ConnectAnInput || task == TaskType.ConnectAnOutput) && MouseAction(EventType.mouseUp, MouseButton.Left))
            {
                task = TaskType.None;
                Repaint();
            }
        }

        private void ApplyConnectInput()
        {
            ConnectionChangePackage changePackage = ((ConnectionChangePackage)taskObject);
            ConnectInputToOutput(changePackage.package, changePackage.node, changePackage.attribute, changePackage.info);
            task = TaskType.None;
            Repaint();
        }

        private void ApplyConnectOutput()
        {
            ConnectionChangePackage changePackage = ((ConnectionChangePackage)taskObject);
            ConnectOutputToInput(changePackage.package, changePackage.node, changePackage.attribute, changePackage.info);
            task = TaskType.None;
            Repaint();
        }

        #endregion



        //---------------------------------------------------------------------------------------\\
        //--------------------------------< CONNECTIONS METHODS >--------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        #region Connection Helper Classes

        private class ConnectionChangePackage
        {
            public NodeEventPackage package;
            public Node node;
            public NodeHandleAttribute attribute;
            public FieldInfo info;
        }

        private class ConnectionIDPackage
        {
            public int connection;
            public int fromID;

            public ConnectionIDPackage(int connection, int fromID)
            {
                this.connection = connection;
                this.fromID = fromID;
            }
        }

        #endregion

        #region Connection Helper Methods

        private int GetCurrentConnection(Node node, NodeHandleAttribute attribute)
        {
            for (int c = 0; c < canvas.connections.Count; c++)
            {
                if (canvas.connections[c].Contains(node, attribute.handlePosition, attribute.handleType))
                {
                    return c;
                }
            }
            return -1;
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Connection Drawer

        public void DrawConnections(Vector2 offset, Rect rect, List<NodeConnection> connections)
        {
            for (int c = connections.Count - 1; c >= 0; c--)
            {
                if (connections[c].to == null)
                {
                    connections.Remove(connections[c]);
                }
                DrawConnection(offset, rect, connections[c]);
            }

            DrawCurrentConnection(offset, rect);
        }

        private void DrawConnection(Vector2 offset, Rect rect, NodeConnection connection)
        {
            List<Vector2> posFroms = new List<Vector2>();

            for (int n = 0; n < connection.froms.Count; n++)
            {
                Vector2 newPos = offset + connection.froms[n].position + new Vector2(connection.froms[n].size.x / 2f, -connection.froms[n].size.y / 2f);
                newPos += connection.fromPositions[n];
                newPos += new Vector2(9f, 9f);
                posFroms.Add(newPos);
            }

            Vector2 toPos = offset;
            toPos += connection.to.position + new Vector2(-connection.to.size.x / 2f, -connection.to.size.y / 2f);
            toPos += connection.toPosition;
            toPos += new Vector2(-9f, 9f);

            for (int c = 0; c < posFroms.Count; c++)
            {
                ConnectionContextEvent(canvas.connections.IndexOf(connection), c, posFroms[c], toPos);
                Handles.DrawBezier(posFroms[c], toPos, posFroms[c], toPos, Color.white, null, 3f);
            }
        }

        private void DrawCurrentConnection(Vector2 offset, Rect rect)
        {
            if (task == TaskType.ConnectAnInput || task == TaskType.ConnectAnOutput)
            {
                NodeEventPackage eventPackage = (NodeEventPackage)taskObject;

                Vector2 anchor = offset + eventPackage.node.position - new Vector2(0f, eventPackage.node.size.y * 0.5f);
                anchor += new Vector2(((int)eventPackage.attribute.handleType * 2 - 1) * (eventPackage.node.size.x * 0.5f), 0f);
                anchor += eventPackage.attribute.handlePosition;
                anchor += new Vector2(((int)eventPackage.attribute.handleType * 2 - 1) * 9f, 9f); 
                Handles.DrawAAPolyLine(4f, anchor, Event.current.mousePosition);
            }
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Connection Context Menu

        private void ConnectionContextMenu(int connection, int fromID)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete Connection"), false, DeleteOneConnection, new ConnectionIDPackage(connection, fromID));
            menu.ShowAsContext();
        }

        #endregion

        #region Connection Context Menu Methods

        private void DeleteOneConnection(object obj)
        {
            ConnectionIDPackage idPackage = (ConnectionIDPackage)obj;
            RegisterTask(TaskType.DeleteConnection, idPackage);
            OnResetTask = CompleteDeleteOneConnection;
        }

        private void CompleteDeleteOneConnection()
        {
            if (task == TaskType.DeleteConnection)
            {
                ConnectionIDPackage idPackage = (ConnectionIDPackage)taskObject;
                NodeConnection connection = canvas.connections[idPackage.connection];
                
                Node node = connection.froms[idPackage.fromID];
                Vector2 handlePos = connection.fromPositions[idPackage.fromID];

                if (connection.DiscardFromConnection(node, handlePos))
                {
                    canvas.connections.Remove(connection);
                }
                task = TaskType.None;
                Repaint();
            }
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Connection Events

        private void ConnectionContextEvent(int connection, int fromID, Vector2 from, Vector2 to)
        {
            Vector2 mousePos = Event.current.mousePosition;

            Vector2 closestPoint = VectorMath.ClosestPointOnLineSegment(mousePos, from, to);
            float distance = Vector2.Distance(closestPoint, mousePos);

            if (MouseAction(EventType.mouseDown, MouseButton.Right) && distance <= 6f)
            {
                ConnectionContextMenu(connection, fromID);
                mouseActionUsed = true;
            }
        }

        private void ConnectOutputToInput(NodeEventPackage package, Node inputNode, NodeHandleAttribute inputAttribute, FieldInfo info)
        {
            int oldOutputConnection = GetCurrentConnection(package.node, package.attribute);

            if (oldOutputConnection != -1)
            {
                if (canvas.connections[oldOutputConnection].DiscardFromConnection(package.node, package.attribute.handlePosition))
                {
                    canvas.connections.RemoveAt(oldOutputConnection);
                }
            }

            int oldInputConnection = GetCurrentConnection(inputNode, inputAttribute);

            if (oldInputConnection != -1)
            {
                canvas.connections[oldInputConnection].AddFrom(package.node, package.attribute.handlePosition);
                
                package.info.SetValue(package.node, canvas.connections[oldInputConnection]);

                return;
            }

            NodeConnection newConnection = NodeConnection.CreateConnection(package.node, package.attribute.handlePosition,
                                                                            inputNode, inputAttribute.handlePosition);
            
            canvas.connections.Add(newConnection);
            info.SetValue(inputNode, newConnection);
            package.info.SetValue(package.node, newConnection);
        }

        private void ConnectInputToOutput(NodeEventPackage package, Node outputNode, NodeHandleAttribute outputAttribute, FieldInfo info)
        {
            int oldOutputConnection = GetCurrentConnection(outputNode, outputAttribute);

            if (oldOutputConnection != -1)
            {
                if (canvas.connections[oldOutputConnection].DiscardFromConnection(outputNode, outputAttribute.handlePosition))
                {
                    canvas.connections.RemoveAt(oldOutputConnection);
                }
            }

            int oldInputConnection = GetCurrentConnection(package.node, package.attribute);

            if (oldInputConnection != -1)
            {
                canvas.connections[oldInputConnection].AddFrom(outputNode, outputAttribute.handlePosition);

                info.SetValue(outputNode, canvas.connections[oldInputConnection]);

                return;
            }

            NodeConnection newConnection = NodeConnection.CreateConnection(outputNode, outputAttribute.handlePosition,
                                                                            package.node, package.attribute.handlePosition);

            canvas.connections.Add(newConnection);
            info.SetValue(outputNode, newConnection);
            package.info.SetValue(package.node, newConnection);
        }

        #endregion



        //---------------------------------------------------------------------------------------\\
        //----------------------------------< GLOBAL METHODS >-----------------------------------\\ 
        //---------------------------------------------------------------------------------------\\
        
    }
}