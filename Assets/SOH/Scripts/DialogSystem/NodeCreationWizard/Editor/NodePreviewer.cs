using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using System.IO;

using Northwind.Essentials;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    public class NodePreviewer : EditorWindow
    {

        #region Preview Data

        private static System.Type previewNodeType;
        private static NodeDataAttribute previewNodeData;

        #endregion

        #region Window Methods

        //The initialization Method for the Window
        [MenuItem("Assets/Preview Node")]
        static void Init()
        {
            NodePreviewer window = CreateInstance<NodePreviewer>();
            window.ShowUtility();
        }

        [MenuItem("Assets/Preview Node", validate = true)]
        static bool ValidateInit()
        {
            if (Selection.activeObject.GetType() == typeof(MonoScript)) {
                MonoScript script = (MonoScript)Selection.activeObject;
                if (script.GetClass().BaseType == typeof(Node))
                {
                    previewNodeType = script.GetClass();
                    return true;
                }
            }
            return false;
        }

        private void OnEnable()
        {
            this.titleContent = new GUIContent("Node Previewer");
            
            object[] attributes = previewNodeType.GetCustomAttributes(false);
            NodeDataAttribute nodeData = new NodeDataAttribute("", "", 0f, 0f);
            foreach (object attribute in attributes)
            {
                if (attribute.GetType() == typeof(NodeDataAttribute))
                {
                    nodeData = attribute as NodeDataAttribute;
                }
            }

            previewNodeData = nodeData;

            this.maxSize = this.minSize = new Vector2(nodeData.nodeSize.x + 64f, nodeData.nodeSize.y + 32f);
        }

        private void OnDestroy()
        {

        }

        private void OnDisable()
        {
            OnDestroy();
        }

        private void OnLostFocus()
        {
            this.Close();
        }

        private void Update()
        {

        }

        //The GUI call for this Window
        void OnGUI()
        {
            DrawNodePreview(previewNodeType, previewNodeData, NodeOperator.GetHandles(previewNodeType).ToArray());
        }

        #endregion
        
        #region Style Data

        private static GUIStyle nodeBGStyle, nodeHeaderStyle, handleBGStyle, handleFGstyle;

        #endregion

        #region Style Methods

        private static void InitiateNodeStyles()
        {
            nodeBGStyle = GUI.skin.GetStyle("ChannelStripBg");

            nodeHeaderStyle = new GUIStyle(GUI.skin.GetStyle("HeaderLabel"));
            nodeHeaderStyle.alignment = TextAnchor.MiddleCenter;

            handleBGStyle = new GUIStyle(GUI.skin.GetStyle("SearchCancelButtonEmpty"));

            handleFGstyle = new GUIStyle(GUI.skin.GetStyle("ColorPicker2DThumb"));
        }

        #endregion

        #region Previwer Field

        private const float nodeHandleWidth = 16f;

        public static void DrawNodePreview(Vector2 origin, System.Type nodeType, NodeDataAttribute nodeData, params NodeHandleAttribute[] nodeHandles)
        {
            GUILayout.BeginArea(new Rect(origin, nodeData.nodeSize + new Vector2(64f, 32f)));
            DrawNodePreview(nodeType, nodeData, nodeHandles);
            GUILayout.EndArea();
        }

        public static void DrawNodePreview(System.Type nodeType, NodeDataAttribute nodeData, params NodeHandleAttribute[] nodeHandles)
        {
            InitiateNodeStyles();

            Rect nodeRect = new Rect(new Vector2(16f, 16f), nodeData.nodeSize + new Vector2(nodeHandleWidth * 2f, 0f));
            GUILayout.BeginArea(nodeRect);

            // Draw Background
            Color oldColor = GUI.backgroundColor;
            GUI.backgroundColor = nodeData.nodeColor;
            GUILayout.BeginArea(new Rect(nodeHandleWidth, 0f, nodeRect.width - nodeHandleWidth * 2f, nodeRect.height), nodeBGStyle);
            GUI.backgroundColor = oldColor;

            // Draw Header
            Rect headerRect = new Rect(0f, 0f, nodeRect.width - nodeHandleWidth * 2f, 22f);
            GUI.Label(headerRect, new GUIContent(nodeData.nodeName, nodeData.nodeDescription), nodeHeaderStyle);

            // Draw Node Editor GUI
            GUILayout.BeginArea(new Rect(8f, 32f, nodeData.nodeSize.x - 16f, nodeData.nodeSize.y - 48f));
            if (nodeType != null)
            {
                DialogCanvas tempCanvas = DialogCanvas.CreateCanvas<DialogCanvas>();
                Node tempNode = Node.CreateNode(nodeType, Vector2.zero);
                tempCanvas.nodes.Add(tempNode);

                Editor editor = Editor.CreateEditor(tempNode);
                NodeInspector nodeEditor = editor as NodeInspector;
                if (nodeEditor != null)
                {
                    nodeEditor.OnDrawNodeGUI(new Rect(0f, 0f, nodeData.nodeSize.x - 16f, nodeData.nodeSize.y - 48f), tempCanvas);
                }
                else
                {
                    editor.OnInspectorGUI();
                }
                DestroyImmediate(editor);
                DestroyImmediate(tempNode);
                DestroyImmediate(tempCanvas);
            }
            else
            {
                GUI.Label(new Rect(0f, 0f, nodeData.nodeSize.x - 16f, nodeData.nodeSize.y - 48f), "No GUI given", nodeHeaderStyle);
            }
            GUILayout.EndArea();

            GUILayout.EndArea();

            // Draw Handles
            DrawHandles(nodeHandles, Northwind.Essentials.VectorMath.Step(nodeData.nodeSize, 16f), ConnectionType.Input);
            DrawHandles(nodeHandles, Northwind.Essentials.VectorMath.Step(nodeData.nodeSize, 16f), ConnectionType.Output);

            GUILayout.EndArea();
        }

        private static void DrawHandles(NodeHandleAttribute[] connections, Vector2 size, ConnectionType type)
        {
            List<NodeHandleAttribute> handles = new List<NodeHandleAttribute>();

            for (int c = 0; c < connections.Length; c++)
            {
                if (connections[c].handleType == type)
                {
                    handles.Add(connections[c]);
                }
            }

            Vector2 anchorPos = Vector2.zero;
            switch (type)
            {
                case ConnectionType.Input: anchorPos = new Vector2(nodeHandleWidth, 0f); break;
                case ConnectionType.Output: anchorPos = new Vector2(nodeHandleWidth + size.x, 0f); break;
            }

            foreach (NodeHandleAttribute handle in handles)
            {
                Rect rect = new Rect(anchorPos + handle.HandlePosition(size), new Vector2(nodeHandleWidth, nodeHandleWidth));

                //Change Direction of Field
                Matrix4x4 guiOriginalMatrix = GUI.matrix;
                GUIUtility.ScaleAroundPivot(new Vector2(handle.handleType == ConnectionType.Output ? 1f : -1f, 1f), anchorPos + handle.HandlePosition(size));

                //Begin the handle drawing
                GUILayout.BeginArea(rect, handleBGStyle);

                //Draw the anchor in the middle
                GUI.Label(new Rect(4f, 5f, 8f, 8f), new GUIContent("", handle.handleTooltip), handleFGstyle);
                GUILayout.EndArea();

                GUI.matrix = guiOriginalMatrix;
            }
        }

        #endregion

    }
}