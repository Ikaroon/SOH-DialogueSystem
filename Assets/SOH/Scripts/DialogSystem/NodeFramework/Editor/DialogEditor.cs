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
        
        private const string NODE_LIB_PATH = "Assets/SOH/Scripts/DialogSystem/DialogNodesLibrary";

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

        #region Extra Features

        //Feature Data for Switch Node
        int switchNodeID = 0;

        //Node lastNode = null;

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
            //Initialize Toolbar Styles is disabled in lightweight version
            InitializeNodeFieldStyles();
            InitiateNodeStyles();
            InitiateHandleStyles();

            stylesLoaded = true;
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Task System Data

        private enum TaskType { None,                       // No task given

                                ResizeSidebar,              // Resize the Sidebar

                                MoveNodeField,              // Move the entire NodeField

                                MoveNode,                   // Move a single Node
                                MovingNode,                 // Moving a single Node

                                ResizeNodeUpperLeft,        // Resize a single Node on the upper left corner
                                ResizeNodeUpperRight,       // Resize a single Node on the upper right corner
                                ResizeNodeBottomRight,      // Resize a single Node on the bottom right corner
                                ResizeNodeBottomLeft,       // Resize a single Node on the bottom left corner

                                ResizeNodeUp,               // Resize a single Node on the upper side
                                ResizeNodeRight,            // Resize a single Node on the right side
                                ResizeNodeBottom,           // Resize a single Node on the bottom side
                                ResizeNodeLeft,             // Resize a single Node on the left side

                                ConnectAnOutput,            // An Output was selected and waits for the connection
                                ConnectAnInput,             // An Input was selected and waits for the connection
                                CreateConnectionOutput,     // An Output was selected and an Input afterwards -> prepared for connection
                                CreateConnectionInput,      // An Input was selected and an Output afterwards -> prepared for connection

                                DeleteNode,                 // A Node was marked for delete and waits for the deletion now
                                FianlizeDeleteNode,         // The System finalizes the deletion of the node
                                DeleteConnection,           // A Connection was marked for delete and waits for the deletion now
                                FinalizeDeleteConnection,   // The System finalizes the deletion of the connection
                                ClearHandle,               // A Handle was marked for delete and waits for the deletion now
                                FinalizeClearHandle,       // The System finalizes the deletion of the handle's content

                                New,                        // The System creates a new canvas
                                FinalizeNew,                // The System finalizes the new creation
                                Load,                       // The System loads a canvas
                                FinalizeLoad,               // The System finalizes the loading
                                Save,                       // The System saves the canvas
                                FinalizeSave,               // The System finalizes the saving

                                MoveToCenter,
                                MoveToSwitchNode,
                                MoveToLastNode

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

        private bool RegisterTask(TaskType taskType, object taskObj)
        {
            if (task == TaskType.None)
            {
                task = taskType;
                taskObject = taskObj;
                return true;
            } else
            {
                return false;
            }
        }

        private System.Action OnResetTask, OnCompleteTask;

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Window Methods

        //The initialization Method for the Window
        [MenuItem("SpyOnHuman/Dialog Editor", priority = 0)]
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

        private void OnDestroy()
        {
            // If no canvas was initialized then skip destrcution
            if (canvas == null)
            {
                System.GC.Collect();
                return;
            }

            //Destroy all temporary nodes
            for (int n = canvas.nodes.Count - 1; n >= 0 ; n--)
            {
                DestroyImmediate(canvas.nodes[n]);
                canvas.nodes.RemoveAt(n);
            }

            //Destroy all temporary connections
            for (int c = canvas.connections.Count - 1; c >= 0; c--)
            {
                DestroyImmediate(canvas.connections[c]);
                canvas.connections.RemoveAt(c);
            }

            //Destroy Start Node
            DestroyImmediate(canvas.startNode);
            canvas.startNode = null;

            //Destroy the temporary canvas
            DestroyImmediate(canvas);
            canvas = null;

            System.GC.Collect();
        }

        private void OnDisable()
        {
            OnDestroy();
        }

        private void OnLostFocus()
        {
            task = TaskType.None;
        }

        private void Update()
        {
            nodeFieldScroll = VectorMath.Round(nodeFieldScroll);

            if (OnCompleteTask != null)
            {
                OnCompleteTask.Invoke();
            }
            
            //Upgrade Task from MoveNode to MovingNode to highlight the used Node
            if (task == TaskType.MoveNode)
            {
                task = TaskType.MovingNode;
            }
        }

        //The GUI call for this Window
        void OnGUI()
        {
            string oldFocus = GUI.GetNameOfFocusedControl();

            //Initialize Styles
            InitializeStyleData();

            //Display Toolbar
            DrawToolbar(new Rect(0f, 0f, position.width, 18f));

            //-----------------------------------------------------------------------------------------

            //Begin Bottom Part of Window
            Rect bottomRect = new Rect(new Rect(0f, 18f, this.position.width, this.position.height - 18f));
            GUILayout.BeginArea(bottomRect);

            //Display Node Editor Frame
            DrawNodeField(new Rect(-1f, 0f, bottomRect.width + 1f, bottomRect.height));

            //Resize Sidebar and Display
            //Disabled in lightweight version
            
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
                && MouseActionInRect(new Rect(Vector2.zero, position.size), EventType.MouseDown, MouseButton.Left))
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

        #region Style Data

        private GUIStyle nodeFieldToolInactive, nodeFieldToolActive;

        #endregion

        #region Style Methods

        private void InitializeNodeFieldStyles()
        {
            nodeFieldToolInactive = new GUIStyle(EditorStyles.miniButton);
            nodeFieldToolInactive.onFocused = nodeFieldToolInactive.normal;
            nodeFieldToolInactive.onActive = nodeFieldToolInactive.normal;

            nodeFieldToolActive = new GUIStyle(EditorStyles.miniButton);
            nodeFieldToolActive.normal = nodeFieldToolActive.onFocused;
            nodeFieldToolActive.normal = nodeFieldToolActive.onActive;
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Node Field Drawer

        private void DrawNodeField(Rect rect)
        {
            GUILayout.BeginArea(rect);

            ExtraFeatures();

            //Calculate Views
            Vector2 center = VectorMath.Round(rect.size / 2f);
            Rect displayRect = new Rect(Vector2.zero, center * 2f);
            
            //Calculate the offset for the node placement
            Vector2 positionOffset = new Vector2(-1f + displayRect.width / 2f + nodeFieldScroll.x,
                                        1f + displayRect.height / 2f + nodeFieldScroll.y);

            DrawGrid(displayRect.size, positionOffset, 16f, new Color(0.1f, 0.1f, 0.1f, 0.2f), new Color(0f, 0.7f, 0f, 0.4f));

            DrawCanvas(displayRect, positionOffset);

            ExtraFeaturesOverlay();

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

            if (task == TaskType.New || task == TaskType.FinalizeNew || task == TaskType.Load || task == TaskType.FinalizeLoad || task == TaskType.Save || task == TaskType.FinalizeSave)
            {
                return;
            }

            //If no canvas is available to draw then abort
            if (!canvas)
            {
                return;
            }

            //Draw the Start Node
            if (IsVisible(positionOffset, displayRect, canvas.startNode))
            {
                DrawNode(positionOffset, canvas.startNode);
            }

            //Iterate over all Nodes and draw them
            for (int n = 0; n < canvas.nodes.Count; n++)
            {
                if (canvas.nodes[n] == null)
                {
                    continue;
                }

                if (!(task == TaskType.MovingNode && ((NodeEventPackage)taskObject).node == canvas.nodes[n]))
                {
                    if (IsVisible(positionOffset, displayRect, canvas.nodes[n]))
                    {
                        DrawNode(positionOffset, canvas.nodes[n]);
                    }
                }
            }

            DrawConnections(positionOffset, displayRect);//, canvas.connections);

            if (task == TaskType.MovingNode)
            {
                DrawNode(positionOffset, ((NodeEventPackage)taskObject).node);
            }

            //Events
            CreateNodeEvent(displayRect);
            MoveFieldEvent(displayRect);
        }

        #endregion

        //-----------------------------------------------------------------------------------------

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

            Node node = Node.CreateNode(pair.type, pair.pos);
            node.position = SnapToGrid(node.position, node.size);

            canvas.nodes.Add(node);
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

        //-----------------------------------------------------------------------------------------

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

            if (MouseActionInRect(displayRect, EventType.MouseDown, MouseButton.Middle))// && mouseIsInside)
            {
                GUI.BeginGroup(calculationRect);
                bool taskGot = RegisterTask(TaskType.MoveNodeField, Event.current.mousePosition);
                GUI.EndGroup();

                if (taskGot)
                {
                    OnResetTask = ResetNodeFieldEvents;
                    mouseActionUsed = true; //Disable all other mouse events from now on!
                }
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

        #region Node Field Extra Events

        private void ExtraFeatures()
        {
            if (MouseActionInRect(new Rect(8f, 8f, 100f, 18f), EventType.MouseDown, MouseButton.Left))
            {
                if (RegisterTask(TaskType.MoveToCenter, Vector2.zero))
                {
                    OnResetTask = MoveToCenter;
                    mouseActionUsed = true;
                }
            }

            if (MouseActionInRect(new Rect(116f, 8f, 100f, 18f), EventType.MouseDown, MouseButton.Left))
            {
                if (RegisterTask(TaskType.MoveToSwitchNode, Vector2.zero))
                {
                    OnResetTask = MoveToSwitchNode;
                    mouseActionUsed = true;
                }
            }
        }

        private void ExtraFeaturesOverlay()
        {
            //TODO: Pressed Animation

            GUI.Box(new Rect(8f, 8f, 100f, 18f), new GUIContent("Go to Center"),
                task == TaskType.MoveToCenter ? nodeFieldToolActive : nodeFieldToolInactive);

            GUI.Box(new Rect(116f, 8f, 100f, 18f), new GUIContent("Switch Node"),
                task == TaskType.MoveToSwitchNode ? nodeFieldToolActive : nodeFieldToolInactive);
        }

        private void MoveToSwitchNode()
        {
            if (task == TaskType.MoveToSwitchNode)
            {
                if (canvas && canvas.nodes.Count > 0f)
                {
                    switchNodeID = (switchNodeID + 1) % canvas.nodes.Count;
                    nodeFieldScroll = -canvas.nodes[switchNodeID].position;
                }
                task = TaskType.None;
            }
        }

        private void MoveToCenter()
        {
            if (task == TaskType.MoveToCenter)
            {
                nodeFieldScroll = Vector2.zero;
                Repaint();
                task = TaskType.None;
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
                menu.AddItem(new GUIContent("Export JSON"), false, null);
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Save"));
                menu.AddDisabledItem(new GUIContent("Save As"));
                menu.AddDisabledItem(new GUIContent("Export JSON"));
            }

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Settings"), false, null);
            menu.DropDown(rect);
        }

        #endregion

        #region Data Menu Methods

        private void New()
        {
            if (RegisterTask(TaskType.New, null))
            {
                OnResetTask = CompleteNew;
                OnCompleteTask = FinalizeNew;
            }
        }

        private void CompleteNew()
        {
            if (task == TaskType.New)
            {
                // If no canvas was initialized then skip destrcution
                if (canvas != null)
                {
                    //Destroy all temporary nodes
                    for (int n = canvas.nodes.Count - 1; n >= 0; n--)
                    {
                        DestroyImmediate(canvas.nodes[n]);
                        canvas.nodes.RemoveAt(n);
                    }

                    //Destroy all temporary connections
                    for (int c = canvas.connections.Count - 1; c >= 0; c--)
                    {
                        DestroyImmediate(canvas.connections[c]);
                        canvas.connections.RemoveAt(c);
                    }

                    //Destroy Start Node
                    DestroyImmediate(canvas.startNode);
                    canvas.startNode = null;

                    //Destroy the temporary canvas
                    DestroyImmediate(canvas);
                }

                canvas = DialogCanvas.CreateCanvas<DialogCanvas>();
                oldPath = "";

                task = TaskType.FinalizeNew;
                OnResetTask = null;
                Repaint();
            }
        }

        private void FinalizeNew()
        {
            if (task == TaskType.FinalizeNew)
            {
                task = TaskType.None;
                OnCompleteTask = null;
                Repaint();
            }
        }

        private void Load()
        {
            if (RegisterTask(TaskType.Load, null))
            {
                OnResetTask = CompleteLoad;
                OnCompleteTask = FinalizeLoad;
            }
        }

        private void CompleteLoad()
        {
            if (task == TaskType.Load)
            {
                oldPath = NodeSaveOperator.Load(ref canvas);
                task = TaskType.FinalizeLoad;
                OnResetTask = null;
                Repaint();
            }
        }

        private void FinalizeLoad()
        {
            if (task == TaskType.FinalizeLoad)
            {
                task = TaskType.None;
                OnCompleteTask = null;
                Repaint();
            }
        }

        private void Save()
        {
            if (RegisterTask(TaskType.Save, null))
            {
                OnResetTask = CompleteSave;
                OnCompleteTask = FinalizeSave;
            }
        }

        private void SaveAs()
        {
            if (RegisterTask(TaskType.Save, null))
            {
                OnResetTask = CompleteSave;
                OnCompleteTask = FinalizeSave;
            }
        }

        private void CompleteSave()
        {
            if (task == TaskType.Save)
            {
                oldPath = NodeSaveOperator.Save(ref canvas, oldPath);
                task = TaskType.FinalizeSave;
                OnResetTask = null;
                Repaint();
            }
        }

        private void FinalizeSave()
        {
            if (task == TaskType.FinalizeSave)
            {
                task = TaskType.None;
                OnCompleteTask = null;
                Repaint();
            }
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Edit Menu

        private void ToolbarEditMenu(Rect rect)
        {
            GenericMenu menu = new GenericMenu();

            if (canvas)
            {
                menu.AddItem(new GUIContent("Smart Align"), false, null);
                menu.AddItem(new GUIContent("Improve"), false, null);
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

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Add Menu

        private void ToolbarAddMenu(Rect rect)
        {
            GenericMenu menu = new GenericMenu();

            if (canvas)
            {
                AddNodesToMenu(ref menu, -nodeFieldScroll);
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

            public NodeDataAttribute data;

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

            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = nodeData.nodeColor;
            GUILayout.BeginArea(new Rect(nodeHandleWidth, 0f, nodeRect.width - nodeHandleWidth * 2f, nodeRect.height), nodeBGStyle);
            GUI.backgroundColor = oldColor;

            Rect headerRect = new Rect(0f, 0f, nodeRect.width - nodeHandleWidth * 2f, 22f);
            if (!((task == TaskType.MoveNode || task == TaskType.MovingNode) && ((NodeEventPackage)taskObject).node == node))
            {
                GUI.Label(headerRect, new GUIContent(nodeData.nodeName, nodeData.nodeDescription), nodeHeaderStyle);
            }
            else
            {
                GUI.Label(headerRect, new GUIContent(nodeData.nodeName), nodeHeaderStyle);
            }

            ResizeNodeSideEvent(node, nodeData, new Rect(0f, 0f, nodeRect.width - nodeHandleWidth * 2f, nodeRect.height));
            ResizeNodeCornerEvent(node, nodeData, new Rect(0f, 0f, nodeRect.width - nodeHandleWidth * 2f, nodeRect.height));
            NodeContextEvent(node, headerRect);
            NodeMoveEvent(node, headerRect);

            //Draw content of node
            GUILayout.BeginArea(new Rect(8f, 32f, node.size.x - 16f, node.size.y - 48f));
            Editor editor = Editor.CreateEditor(node);
            NodeInspector nodeEditor = editor as NodeInspector;
            if (nodeEditor != null)
            {
                nodeEditor.OnDrawNodeGUI(new Rect(0f, 0f, node.size.x - 16f, node.size.y - 48f), canvas);
            }
            else
            {
                editor.OnInspectorGUI();
            }
            DestroyImmediate(editor);
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

            if (node.GetType() != typeof(StartNode))
            {
                menu.AddItem(new GUIContent("Delete"), false, DeleteNode, node);
            }

            menu.ShowAsContext();
        }

        #endregion

        #region Node Context Menu Methods

        private void DeleteNode(object obj)
        {
            if (RegisterTask(TaskType.DeleteNode, (Node)obj))
            {
                OnCompleteTask = FinalizeDeleteNodeTask;
                OnResetTask = StartDeleteNodeTask;
            }
        }

        private void StartDeleteNodeTask()
        {
            if (task == TaskType.DeleteNode)
            {
                task = TaskType.FianlizeDeleteNode;
                OnResetTask = null;
            }
        }

        private void FinalizeDeleteNodeTask()
        {
            if (task == TaskType.FianlizeDeleteNode)
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
                        DestroyImmediate(canvas.connections[connection]);
                        canvas.connections.RemoveAt(connection);
                    }
                }

                List<NodeHandlePackage> outputHandles = NodeOperator.GetConnections(node, ConnectionType.Output);

                for (int oh = 0; oh < outputHandles.Count; oh++)
                {
                    int connection = GetCurrentConnection(node, outputHandles[oh].handle);

                    if (connection != -1)
                    {
                        if (canvas.connections[connection].DiscardFromConnection(node, outputHandles[oh].handle.ID))
                        {
                            DestroyImmediate(canvas.connections[connection]);
                            canvas.connections.RemoveAt(connection);
                        }
                    }
                }

                //Remove Node

                node.OnDelete(canvas);
                int nodeID = canvas.nodes.IndexOf(node);
                DestroyImmediate(node);
                canvas.nodes.RemoveAt(nodeID);
                task = TaskType.None;
                OnCompleteTask = null;
                Repaint();
            }
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Node Events

        private void NodeContextEvent(Node node, Rect rect)
        {
            if (MouseActionInRect(rect, EventType.MouseDown, MouseButton.Right))
            {
                NodeContextMenu(node);
                mouseActionUsed = true; //Disable all other mouse events from now on!
            }
        }

        private void NodeMoveEvent(Node node, Rect rect)
        {
            if (MouseActionInRect(rect, EventType.MouseDown, MouseButton.Left))
            {
                NodeEventPackage eventPackage = new NodeEventPackage();
                eventPackage.node = node;
                eventPackage.anchor = Event.current.mousePosition;
                if (RegisterTask(TaskType.MoveNode, eventPackage))
                {
                    OnResetTask = ResetNodeMoveEvent;
                    mouseActionUsed = true; //Disable all other mouse events from now on!
                }
            }
            else if ((task == TaskType.MoveNode || task == TaskType.MovingNode) && MouseAction(EventType.MouseDrag, MouseButton.Left) && ((NodeEventPackage)taskObject).node == node)
            {
                Vector2 anchorPoint = ((NodeEventPackage)taskObject).anchor;
                node.position = new Vector2(node.position.x + (Event.current.mousePosition.x - anchorPoint.x),
                                            node.position.y + (Event.current.mousePosition.y - anchorPoint.y));
                node.position = SnapToGrid(node.position, node.size);
                mouseActionUsed = true; //Disable all other mouse events from now on!
                Repaint();
            }
        }

        private void ResetNodeMoveEvent()
        {
            if ((task == TaskType.MoveNode || task == TaskType.MovingNode) && MouseAction(EventType.MouseUp, MouseButton.Left))
            {
                task = TaskType.None;
                mouseActionUsed = true; //Disable all other mouse events from now on!
            }
        }

        private void ResizeNodeCornerEvent(Node node, NodeDataAttribute attribute, Rect rect)
        {
            if (!attribute.resizeable)
            {
                return;
            }

            Rect upperLeft = new Rect(0f, 0f, 8f, 8f);
            Rect upperRight = new Rect(rect.width - 8f, 0f, 8f, 8f);
            Rect bottomRight = new Rect(rect.width - 8f, rect.height - 8f, 8f, 8f);
            Rect bottomLeft = new Rect(0f, rect.height - 8f, 8f, 8f);

            EditorGUIUtility.AddCursorRect(upperLeft, MouseCursor.ResizeUpLeft);
            EditorGUIUtility.AddCursorRect(upperRight, MouseCursor.ResizeUpRight);
            EditorGUIUtility.AddCursorRect(bottomRight, MouseCursor.ResizeUpLeft);
            EditorGUIUtility.AddCursorRect(bottomLeft, MouseCursor.ResizeUpRight);

            if (MouseActionInRect(rect, EventType.MouseDown, MouseButton.Left))
            {
                NodeEventPackage eventPackage = new NodeEventPackage();
                eventPackage.anchor = Event.current.mousePosition;
                eventPackage.node = node;
                eventPackage.data = attribute;
                if (upperLeft.Contains(Event.current.mousePosition)) //Resize Upper Left
                {
                    if (RegisterTask(TaskType.ResizeNodeUpperLeft, eventPackage))
                    {
                        OnResetTask = ResetNodeResizeTask;
                        mouseActionUsed = true;
                    }
                }
                else if (upperRight.Contains(Event.current.mousePosition)) //Resize Upper Right
                {
                    if (RegisterTask(TaskType.ResizeNodeUpperRight, eventPackage))
                    {
                        OnResetTask = ResetNodeResizeTask;
                        mouseActionUsed = true;
                    }
                }
                else if(bottomRight.Contains(Event.current.mousePosition)) //Resize Bottom Right
                {
                    if (RegisterTask(TaskType.ResizeNodeBottomRight, eventPackage))
                    {
                        OnResetTask = ResetNodeResizeTask;
                        mouseActionUsed = true;
                    }
                } else if (bottomLeft.Contains(Event.current.mousePosition)) //Resize Bottom Left
                {
                    if (RegisterTask(TaskType.ResizeNodeBottomLeft, eventPackage))
                    {
                        OnResetTask = ResetNodeResizeTask;
                        mouseActionUsed = true;
                    }
                }
            } else if (MouseAction(EventType.MouseDrag, MouseButton.Left))
            {
                switch(task)
                {
                    case TaskType.ResizeNodeUpperLeft:
                        {
                            NodeEventPackage eventPackage = (NodeEventPackage)taskObject;
                            if (eventPackage.node == node)
                            {
                                Vector2 change = Event.current.mousePosition - eventPackage.anchor;
                                change = VectorMath.Step(change, 16f);

                                change = VectorMath.Max(attribute.nodeSize, eventPackage.node.size - change) - eventPackage.node.size;

                                eventPackage.node.size = eventPackage.node.size + change;
                                eventPackage.node.position = eventPackage.node.position - change / 2f;

                                if (change.y != 0f)
                                {
                                    RemapHandles(node);
                                }

                                Repaint();

                                mouseActionUsed = true;
                            }
                        }
                        break;
                    case TaskType.ResizeNodeUpperRight:
                        {
                            NodeEventPackage eventPackage = (NodeEventPackage)taskObject;
                            if (eventPackage.node == node)
                            {
                                Vector2 change = Event.current.mousePosition - eventPackage.anchor;
                                change = VectorMath.Step(change, 16f);

                                change = VectorMath.Max(attribute.nodeSize, eventPackage.node.size + new Vector2(change.x, -change.y)) - eventPackage.node.size;

                                eventPackage.anchor.x += change.x;
                                taskObject = eventPackage;

                                eventPackage.node.size = eventPackage.node.size + new Vector2(change.x, change.y);
                                eventPackage.node.position = eventPackage.node.position + new Vector2(change.x, -change.y) / 2f;

                                if (change.y != 0f)
                                {
                                    RemapHandles(node);
                                }

                                Repaint();

                                mouseActionUsed = true;
                            }
                        }
                        break;
                    case TaskType.ResizeNodeBottomRight:
                        {
                            NodeEventPackage eventPackage = (NodeEventPackage)taskObject;
                            if (eventPackage.node == node)
                            {
                                Vector2 change = Event.current.mousePosition - eventPackage.anchor;
                                change = VectorMath.Step(change, 16f);

                                change = VectorMath.Max(attribute.nodeSize, eventPackage.node.size + change) - eventPackage.node.size;

                                eventPackage.anchor += change;
                                taskObject = eventPackage;

                                eventPackage.node.size = eventPackage.node.size + change;
                                eventPackage.node.position = eventPackage.node.position + change / 2f;

                                if (change.y != 0f)
                                {
                                    RemapHandles(node);
                                }

                                Repaint();

                                mouseActionUsed = true;
                            }
                        }
                        break;
                    case TaskType.ResizeNodeBottomLeft:
                        {
                            NodeEventPackage eventPackage = (NodeEventPackage)taskObject;
                            if (eventPackage.node == node)
                            {
                                Vector2 change = Event.current.mousePosition - eventPackage.anchor;
                                change = VectorMath.Step(change, 16f);

                                change = VectorMath.Max(attribute.nodeSize, eventPackage.node.size + new Vector2(-change.x, change.y)) - eventPackage.node.size;

                                eventPackage.anchor.y += change.y;
                                taskObject = eventPackage;

                                eventPackage.node.size = eventPackage.node.size + new Vector2(change.x, change.y);
                                eventPackage.node.position = eventPackage.node.position + new Vector2(-change.x, change.y) / 2f;

                                if (change.y != 0f)
                                {
                                    RemapHandles(node);
                                }

                                Repaint();

                                mouseActionUsed = true;
                            }
                        }
                        break;
                }
            }
        }

        private void ResizeNodeSideEvent(Node node, NodeDataAttribute attribute, Rect rect)
        {
            if (!attribute.resizeable)
            {
                return;
            }

            Rect up = new Rect(8f, 0f, rect.width - 16f, 8f);
            Rect right = new Rect(rect.width - 8f, 8f, 8f, rect.height - 16f);
            Rect bottom = new Rect(8f, rect.height - 8f, rect.width - 16f, 8f);
            Rect left = new Rect(0f, 8f, 8f, rect.height - 16f);

            EditorGUIUtility.AddCursorRect(up, MouseCursor.ResizeVertical);
            EditorGUIUtility.AddCursorRect(right, MouseCursor.ResizeHorizontal);
            EditorGUIUtility.AddCursorRect(bottom, MouseCursor.ResizeVertical);
            EditorGUIUtility.AddCursorRect(left, MouseCursor.ResizeHorizontal);

            if (MouseActionInRect(rect, EventType.MouseDown, MouseButton.Left))
            {
                NodeEventPackage eventPackage = new NodeEventPackage();
                eventPackage.anchor = Event.current.mousePosition;
                eventPackage.node = node;
                eventPackage.data = attribute;
                if (up.Contains(Event.current.mousePosition)) //Resize Up
                {
                    if (RegisterTask(TaskType.ResizeNodeUp, eventPackage))
                    {
                        OnResetTask = ResetNodeResizeTask;
                        mouseActionUsed = true;
                    }
                }
                else if (right.Contains(Event.current.mousePosition)) //Resize Right
                {
                    if (RegisterTask(TaskType.ResizeNodeRight, eventPackage))
                    {
                        OnResetTask = ResetNodeResizeTask;
                        mouseActionUsed = true;
                    }
                }
                else if (bottom.Contains(Event.current.mousePosition)) //Resize Bottom
                {
                    if (RegisterTask(TaskType.ResizeNodeBottom, eventPackage))
                    {
                        OnResetTask = ResetNodeResizeTask;
                        mouseActionUsed = true;
                    }
                }
                else if (left.Contains(Event.current.mousePosition)) //Resize Left
                {
                    if (RegisterTask(TaskType.ResizeNodeLeft, eventPackage))
                    {
                        OnResetTask = ResetNodeResizeTask;
                        mouseActionUsed = true;
                    }
                }
            }
            else if (MouseAction(EventType.MouseDrag, MouseButton.Left))
            {
                switch (task)
                {
                    case TaskType.ResizeNodeUp:
                        {
                            NodeEventPackage eventPackage = (NodeEventPackage)taskObject;
                            if (eventPackage.node == node)
                            {
                                Vector2 change = Event.current.mousePosition - eventPackage.anchor;
                                change = VectorMath.Step(change, 16f);
                                change.x *= 0f;

                                change = VectorMath.Max(attribute.nodeSize, eventPackage.node.size - change) - eventPackage.node.size;

                                eventPackage.node.size = eventPackage.node.size + change;
                                eventPackage.node.position = eventPackage.node.position - change / 2f;

                                if (change.y != 0f)
                                {
                                    RemapHandles(node);
                                }

                                Repaint();

                                mouseActionUsed = true;
                            }
                        }
                        break;
                    case TaskType.ResizeNodeRight:
                        {
                            NodeEventPackage eventPackage = (NodeEventPackage)taskObject;
                            if (eventPackage.node == node)
                            {
                                Vector2 change = Event.current.mousePosition - eventPackage.anchor;
                                change = VectorMath.Step(change, 16f);

                                change.y *= 0f;

                                change = VectorMath.Max(attribute.nodeSize, eventPackage.node.size + change) - eventPackage.node.size;

                                eventPackage.anchor.x += change.x;
                                taskObject = eventPackage;

                                eventPackage.node.size = eventPackage.node.size + change;
                                eventPackage.node.position = eventPackage.node.position + change / 2f;

                                Repaint();

                                mouseActionUsed = true;
                            }
                        }
                        break;
                    case TaskType.ResizeNodeBottom:
                        {
                            NodeEventPackage eventPackage = (NodeEventPackage)taskObject;
                            if (eventPackage.node == node)
                            {
                                Vector2 change = Event.current.mousePosition - eventPackage.anchor;
                                change = VectorMath.Step(change, 16f);
                                change.x *= 0f;

                                change = VectorMath.Max(attribute.nodeSize, eventPackage.node.size + change) - eventPackage.node.size;

                                eventPackage.anchor += change;
                                taskObject = eventPackage;

                                eventPackage.node.size = eventPackage.node.size + change;
                                eventPackage.node.position = eventPackage.node.position + change / 2f;

                                if (change.y != 0f)
                                {
                                    RemapHandles(node);
                                }

                                Repaint();

                                mouseActionUsed = true;
                            }
                        }
                        break;
                    case TaskType.ResizeNodeLeft:
                        {
                            NodeEventPackage eventPackage = (NodeEventPackage)taskObject;
                            if (eventPackage.node == node)
                            {
                                Vector2 change = Event.current.mousePosition - eventPackage.anchor;
                                change = VectorMath.Step(change, 16f);
                                change.y *= 0f;

                                change = VectorMath.Max(attribute.nodeSize, eventPackage.node.size + -change) - eventPackage.node.size;

                                eventPackage.node.size = eventPackage.node.size + change;
                                eventPackage.node.position = eventPackage.node.position - change / 2f;

                                Repaint();

                                mouseActionUsed = true;
                            }
                        }
                        break;
                }
            }
        }

        private void ResetNodeResizeTask()
        {
            if (((task == TaskType.ResizeNodeUpperLeft || task == TaskType.ResizeNodeUpperRight || task == TaskType.ResizeNodeBottomRight || task == TaskType.ResizeNodeBottomLeft) 
                || (task == TaskType.ResizeNodeUp || task == TaskType.ResizeNodeRight || task == TaskType.ResizeNodeBottom || task == TaskType.ResizeNodeLeft))
                && MouseAction(EventType.MouseUp, MouseButton.Left))
            {
                task = TaskType.None;
                Repaint();
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
            handleFGstyle.fixedHeight = 0f;
            handleFGstyle.fixedWidth = 0f;
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
                MouseEventPackage mouseEventPackage = DrawHandle(handle.handle, anchorPos + handle.handle.HandlePosition(size), handle.info.GetValue(node) != null);

                //Handle Events
                HandleContextEvent(mouseEventPackage, node, handle.handle, handle.info);
                HandleEventConnect(mouseEventPackage, node, handle.handle, handle.info);
            }
        }

        public MouseEventPackage DrawHandle(NodeHandleAttribute handle, Vector2 pos, bool filled)
        {
            MouseEventPackage mouseEvent = new MouseEventPackage(EventType.Ignore, MouseButton.None);
            Rect rect = new Rect(pos, new Vector2(nodeHandleWidth, nodeHandleWidth));

            //Change Direction of Field
            Matrix4x4 guiOriginalMatrix = GUI.matrix;
            GUIUtility.ScaleAroundPivot(new Vector2(handle.handleType == ConnectionType.Output ? 1f : -1f, 1f), pos);

            //Begin the handle drawing
            GUILayout.BeginArea(rect, handleBGStyle);

            //Draw the anchor in the middle
            GUI.Label(new Rect(4f, 5f, 8f, 8f), new GUIContent("", handle.handleTooltip), handleFGstyle);
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
            if (RegisterTask(TaskType.ClearHandle, eventPackage))
            {
                OnResetTask = StartClearHandleTask;
                OnCompleteTask = FinalizeClearHandleTask;
            }
        }

        private void StartClearHandleTask()
        {
            if (task == TaskType.ClearHandle)
            {
                task = TaskType.FinalizeClearHandle;
                OnResetTask = null;
            }
        }

        private void FinalizeClearHandleTask()
        {
            if (task == TaskType.FinalizeClearHandle)
            {
                NodeEventPackage eventPackage = (NodeEventPackage)taskObject;
                int connection = GetCurrentConnection(eventPackage.node, eventPackage.attribute);
                if (eventPackage.attribute.handleType == ConnectionType.Input)
                {
                    if (connection != -1)
                    {
                        canvas.connections[connection].DiscardConnection();
                        DestroyImmediate(canvas.connections[connection]);
                        canvas.connections.RemoveAt(connection);
                    }
                }
                else if (eventPackage.attribute.handleType == ConnectionType.Output)
                {
                    if (connection != -1)
                    {
                        if (canvas.connections[connection].DiscardFromConnection(eventPackage.node, eventPackage.attribute.ID))
                        {
                            DestroyImmediate(canvas.connections[connection]);
                            canvas.connections.RemoveAt(connection);
                        }
                    }
                }
                task = TaskType.None;
                OnCompleteTask = null;
                Repaint();
            }
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Handle Events

        private void HandleContextEvent(MouseEventPackage inputPackage, Node node, NodeHandleAttribute attribute, FieldInfo info)
        {
            if (task == TaskType.None && inputPackage.eventType == EventType.MouseDown && inputPackage.mouseButton == MouseButton.Right)
            {
                HandleContextMenu(node, attribute, info);
                mouseActionUsed = true; //Disable all other mouse events from now on!
            }
        }
        
        private void HandleEventConnect(MouseEventPackage inputPackage, Node node, NodeHandleAttribute attribute, FieldInfo info)
        {
            if (task == TaskType.None && inputPackage.eventType == EventType.MouseDown && inputPackage.mouseButton == MouseButton.Left)
            {
                NodeEventPackage eventPackage = new NodeEventPackage();
                eventPackage.node = node;
                eventPackage.anchor = Event.current.mousePosition;
                eventPackage.attribute = attribute;
                eventPackage.info = info;

                bool taskReceived = false;

                switch (attribute.handleType)
                {
                    case ConnectionType.Input:
                        {
                            taskReceived = RegisterTask(TaskType.ConnectAnInput, eventPackage);
                        }
                        break;
                    case ConnectionType.Output:
                        {
                            taskReceived = RegisterTask(TaskType.ConnectAnOutput, eventPackage);
                        }
                        break;
                }

                if (taskReceived) {
                    OnResetTask = ResetHandleConnect;
                    mouseActionUsed = true; //Disable all other mouse events from now on!
                }
            }
            else if (inputPackage.eventType == EventType.MouseUp && inputPackage.mouseButton == MouseButton.Left)
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

                            if (RegisterTask(TaskType.CreateConnectionInput, changePack))
                            {
                                OnResetTask = ApplyConnectInput;
                            }
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

                            if (RegisterTask(TaskType.CreateConnectionOutput, changePack))
                            {
                                OnResetTask = ApplyConnectOutput;
                            }
                        }
                        break;
                }
            }
        }

        private void HandleEverEvent()
        {
            if (task == TaskType.ConnectAnInput || task == TaskType.ConnectAnOutput)
            {
                if (MouseAction(EventType.MouseDrag, MouseButton.Left))
                {
                    Repaint();
                }
            }
        }

        private void ResetHandleConnect()
        {
            if ((task == TaskType.ConnectAnInput || task == TaskType.ConnectAnOutput) && MouseAction(EventType.MouseUp, MouseButton.Left))
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
                if (canvas.connections[c].Contains(node, attribute.ID, attribute.handleType))
                {
                    return c;
                }
            }
            return -1;
        }

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Connection Drawer

        public void DrawConnections(Vector2 offset, Rect rect)//, List<NodeConnection> connections)
        {
            for (int c = canvas.connections.Count - 1; c >= 0; c--)
            {
                if (canvas.connections[c].to == null)
                {
                    canvas.connections.Remove(canvas.connections[c]);
                }
                DrawConnection(offset, rect, canvas.connections[c]);
            }

            DrawCurrentConnection(offset, rect);
        }

        private void DrawConnection(Vector2 offset, Rect rect, NodeConnection connection)
        {
            List<Vector2> posFroms = new List<Vector2>();

            for (int n = 0; n < connection.froms.Count; n++)
            {
                Vector2 newPos = offset + connection.froms[n].position + new Vector2(connection.froms[n].size.x / 2f, -connection.froms[n].size.y / 2f);
                newPos += connection.fromAttributes[n].position;
                newPos += new Vector2(9f, 9f);
                posFroms.Add(newPos);
            }
            
            Vector2 toPos = offset;
            toPos += connection.to.position + new Vector2(-connection.to.size.x / 2f, -connection.to.size.y / 2f);
            toPos += connection.toAttribute.position;
            toPos += new Vector2(-9f, 9f);


            for (int c = 0; c < posFroms.Count; c++)
            {
                if (rect.Contains(posFroms[c]) || rect.Contains(toPos))
                {
                    ConnectionContextEvent(canvas.connections.IndexOf(connection), c, posFroms[c], toPos);
                    Handles.DrawBezier(posFroms[c], toPos, posFroms[c], toPos, Color.white, null, 3f);
                }
            }
        }

        private void DrawCurrentConnection(Vector2 offset, Rect rect)
        {
            if (task == TaskType.ConnectAnInput || task == TaskType.ConnectAnOutput)
            {
                NodeEventPackage eventPackage = (NodeEventPackage)taskObject;

                Vector2 anchor = offset + eventPackage.node.position - new Vector2(0f, eventPackage.node.size.y * 0.5f);
                anchor += new Vector2(((int)eventPackage.attribute.handleType * 2 - 1) * (eventPackage.node.size.x * 0.5f), 0f);
                anchor += eventPackage.attribute.HandlePosition(eventPackage.node.size);
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
            if (RegisterTask(TaskType.DeleteConnection, idPackage))
            {
                OnResetTask = StartDeleteOneConnection;
                OnCompleteTask = FinalizeDeleteOneConnection;
            }
        }

        private void StartDeleteOneConnection()
        {
            if (task == TaskType.DeleteConnection)
            {
                task = TaskType.FinalizeDeleteConnection;
                OnResetTask = null;
            }
        }

        private void FinalizeDeleteOneConnection()
        {
            if (task == TaskType.FinalizeDeleteConnection)
            {
                ConnectionIDPackage idPackage = (ConnectionIDPackage)taskObject;
                NodeConnection connection = canvas.connections[idPackage.connection];
                
                Node node = connection.froms[idPackage.fromID];

                if (connection.DiscardFromConnection(node, connection.fromAttributes[idPackage.fromID].ID))
                {
                    int connectionID = canvas.connections.IndexOf(connection);
                    DestroyImmediate(connection);
                    canvas.connections.RemoveAt(connectionID);
                }
                task = TaskType.None;
                OnCompleteTask = null;
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

            if (MouseAction(EventType.MouseDown, MouseButton.Right) && distance <= 6f)
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
                if (canvas.connections[oldOutputConnection].DiscardFromConnection(package.node, package.attribute.ID))
                {
                    DestroyImmediate(canvas.connections[oldOutputConnection]);
                    canvas.connections.RemoveAt(oldOutputConnection);
                }
            }

            int oldInputConnection = GetCurrentConnection(inputNode, inputAttribute);

            if (oldInputConnection != -1)
            {
                canvas.connections[oldInputConnection].AddFrom(package.node, new NodeHandleID(package.attribute.ID, package.attribute.HandlePosition(package.node.size)));
                
                package.info.SetValue(package.node, canvas.connections[oldInputConnection]);

                return;
            }

            //Debug.Log(inputAttribute.ID);
            NodeConnection newConnection = NodeConnection.CreateConnection(package.node, new NodeHandleID(package.attribute.ID, package.attribute.HandlePosition(package.node.size)),
                                                                            inputNode, new NodeHandleID(inputAttribute.ID, inputAttribute.HandlePosition(inputNode.size)));
            
            canvas.connections.Add(newConnection);
            info.SetValue(inputNode, newConnection);
            package.info.SetValue(package.node, newConnection);
        }

        private void ConnectInputToOutput(NodeEventPackage package, Node outputNode, NodeHandleAttribute outputAttribute, FieldInfo info)
        {
            int oldOutputConnection = GetCurrentConnection(outputNode, outputAttribute);

            if (oldOutputConnection != -1)
            {
                if (canvas.connections[oldOutputConnection].DiscardFromConnection(outputNode, outputAttribute.ID))
                {
                    DestroyImmediate(canvas.connections[oldOutputConnection]);
                    canvas.connections.RemoveAt(oldOutputConnection);
                }
            }

            int oldInputConnection = GetCurrentConnection(package.node, package.attribute);

            if (oldInputConnection != -1)
            {
                canvas.connections[oldInputConnection].AddFrom(outputNode, new NodeHandleID(outputAttribute.ID, outputAttribute.HandlePosition(outputNode.size)));

                info.SetValue(outputNode, canvas.connections[oldInputConnection]);

                return;
            }

            //Debug.Log(package.attribute.ID);
            NodeConnection newConnection = NodeConnection.CreateConnection(outputNode, new NodeHandleID(outputAttribute.ID, outputAttribute.HandlePosition(outputNode.size)),
                                                                            package.node, new NodeHandleID(package.attribute.ID, package.attribute.HandlePosition(package.node.size)));

            canvas.connections.Add(newConnection);
            info.SetValue(outputNode, newConnection);
            package.info.SetValue(package.node, newConnection);
        }

        #endregion



        //---------------------------------------------------------------------------------------\\
        //----------------------------------< GLOBAL METHODS >-----------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        #region Global Methods

        private Vector2 SnapToGrid(Vector2 position, Vector2 size)
        {
            float halfX = size.x / 2f;
            float halfY = size.y / 2f;

            Vector2 add = new Vector2(8f * (halfX % 16f != 0f ? 1f : 0f), 8f * (halfY % 16f != 0f ? 1f : 0f));

            position = VectorMath.Step(position, 16f) + add;

            return position;
        }

        private void RemapHandles(Node node)
        {
            List<NodeHandlePackage> packages = NodeOperator.GetConnections(node, ConnectionType.Output);

            foreach (NodeHandlePackage package in packages)
            {
                int connection = GetCurrentConnection(node, package.handle);
                if (connection != -1)
                {
                    canvas.connections[connection].UpdateFrom(node, package.handle.ID, package.handle.HandlePosition(node.size));
                }
            }

            packages = NodeOperator.GetConnections(node, ConnectionType.Input);

            foreach (NodeHandlePackage package in packages)
            {
                int connection = GetCurrentConnection(node, package.handle);
                if (connection != -1)
                {
                    canvas.connections[connection].UpdateTo(package.handle.HandlePosition(node.size));
                }
            }

        }

        #endregion

    }
}