using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    public static class NodeDrawer
    {
        #region Style Data

        private static GUIStyle nodeBGStyle, headerStyle, handleBGStyle, handleFGstyle;

        #endregion

        #region Style Methods

        private static void InitiateStyles()
        {
            if (nodeBGStyle != null)
            {
                return;
            }

            nodeBGStyle = GUI.skin.GetStyle("ChannelStripBg");

            headerStyle = new GUIStyle(GUI.skin.GetStyle("HeaderLabel"));
            headerStyle.alignment = TextAnchor.MiddleCenter;

            handleBGStyle = new GUIStyle(GUI.skin.GetStyle("SearchCancelButtonEmpty"));

            handleFGstyle = new GUIStyle(GUI.skin.GetStyle("ColorPicker2DThumb"));
        }

        #endregion

        #region Task Data

        private enum NodeDrawerTask { None, MoveNode, ResizeUL, ResizeUR, ResizeDR, ResizeDL, Connect };
        private static NodeDrawerTask currentTask = NodeDrawerTask.None;

        private static Node currentNode;
        private static Vector2 anchorPoint;
        private static NodeHandlePackage handleInfo;

        #endregion

        #region Nouse Methods

        private static bool CheckMovement(Node node, Rect rect)
        {
            if (Event.current.isMouse && rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                currentTask = NodeDrawerTask.MoveNode;
                currentNode = node;
                anchorPoint = Event.current.mousePosition;
                return true;
            }

            if (Event.current.isMouse && Event.current.type == EventType.MouseDrag && currentNode == node)
            {
                if (currentTask == NodeDrawerTask.MoveNode)
                {
                    node.position = new Vector2(node.position.x + (Event.current.mousePosition.x - anchorPoint.x), node.position.y + (Event.current.mousePosition.y - anchorPoint.y));
                    return true;
                }
            }

            return false;
        }

        private static bool CheckMovementStop(Node node)
        {
            if (currentTask == NodeDrawerTask.MoveNode && currentNode == node && Event.current.isMouse && Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                currentTask = NodeDrawerTask.None;
                currentNode = null;
                anchorPoint = Vector2.zero;
                return true;
            }

            return false;
        }

        #endregion

        #region Node Drawer Methods

        public static bool IsVisible(Rect canvasRect, Rect rect, Node node)
        {
            if (currentNode == node)
            {
                return true;
            }

            bool inside = false;

            for (int x = -1; x < 2; x+= 2)
            {
                for (int y = -1; y < 2; y+= 2)
                {
                    if (rect.Contains(new Vector2(  node.position.x + (node.size.x + 36f) * 0.5f * x,
                                                    node.position.y + node.size.y * 0.5f * y) +
                                                    canvasRect.size * 0.5f)) {
                        inside = true;
                    }
                }
            }
            return inside;
        }

        public static void DrawNode(Rect rect, Node node, ref bool eventUsed, DialogEditor editor)
        {
            InitiateStyles();

            object[] attributes = node.GetType().GetCustomAttributes(false);
            NodeDataAttribute nodeData = new NodeDataAttribute("", "", 0f, 0f);
            foreach (object attribute in attributes)
            {
                if (attribute.GetType() == typeof(NodeDataAttribute))
                {
                    nodeData = attribute as NodeDataAttribute;
                }
            }
            DrawNodeInternal(rect, node, nodeData, ref eventUsed, editor);
        }

        private static void DrawNodeInternal(Rect rect, Node node, NodeDataAttribute nodeData, ref bool eventUsed, DialogEditor editor)
        {
            Rect nodeRect = new Rect(rect.size / 2f + node.position - node.size / 2f + new Vector2(-18f, 0f), node.size + new Vector2(36f, 0f));
            GUILayout.BeginArea(nodeRect);

            GUILayout.BeginArea(new Rect(18f, 0f, nodeRect.width - 36f, nodeRect.height), nodeBGStyle);
            Rect headerRect = new Rect(0f, 0f, nodeRect.width - 36f, 22f);
            GUI.Label(headerRect, new GUIContent(nodeData.nodeName), headerStyle);

            CheckMovement(node, headerRect);

            GUILayout.EndArea();

            DrawHandles(node, new Vector2(nodeRect.width - 36f, nodeRect.height), ConnectionType.Input, ref eventUsed, editor);
            DrawHandles(node, new Vector2(nodeRect.width - 36f, nodeRect.height), ConnectionType.Output, ref eventUsed, editor);

            GUILayout.EndArea();

            CheckMovementStop(node);
        }

        private static void DrawHandles(Node node, Vector2 size, ConnectionType type, ref bool eventUsed, DialogEditor editor)
        {
            List<NodeHandlePackage> handles = NodeOperator.GetConnections(node, type);

            Vector2 anchorPos = Vector2.zero;
            switch (type)
            {
                case ConnectionType.Input: anchorPos = new Vector2(18f, 0f); break;
                case ConnectionType.Output: anchorPos = new Vector2(18f + size.x, 0f); break;
            }

            foreach (NodeHandlePackage handle in handles)
            {
                MouseEventPackage mouseEventPackage = DrawHandle(type, anchorPos + handle.handle.handlePosition, handle.info.GetValue(node) != null);
                if (currentTask == NodeDrawerTask.None && mouseEventPackage.eventType == EventType.mouseDown && mouseEventPackage.mouseButton == MouseButton.Left)
                {
                    currentTask = NodeDrawerTask.Connect;
                    anchorPoint = node.position;
                    currentNode = node;
                    handleInfo = handle;
                }

                if (currentNode != node && currentTask == NodeDrawerTask.Connect && mouseEventPackage.eventType == EventType.mouseUp && mouseEventPackage.mouseButton == MouseButton.Left)
                {
                    if (handle.handle.handleType != handleInfo.handle.handleType)
                    {
                        switch (handleInfo.handle.handleType)
                        {
                            case ConnectionType.Output:

                                for (int c = 0; c < editor.canvas.connections.Count; c++)
                                {
                                    if (editor.canvas.connections[c].froms.Contains(currentNode))
                                    {
                                        editor.canvas.connections[c].DiscardFromConnection(currentNode);
                                    }
                                }

                                if ((handle.info.GetValue(node) as NodeConnection) == null)
                                {
                                    NodeConnection connection = NodeConnection.CreateConnection(currentNode, handleInfo.handle.handlePosition,
                                        node, handle.handle.handlePosition);
                                    handle.info.SetValue(node, connection);
                                    handleInfo.info.SetValue(currentNode, connection);
                                    editor.canvas.connections.Add(connection);
                                } else
                                {
                                    for (int c = 0; c < editor.canvas.connections.Count; c++)
                                    {
                                        Debug.Log(editor.canvas.connections[c].to);
                                        Debug.Log(node);
                                        if (editor.canvas.connections[c].to == node)
                                        {
                                            Debug.Log("Got");
                                            editor.canvas.connections[c].AddFrom(currentNode, handleInfo.handle.handlePosition);
                                        }
                                    }
                                }
                                break;
                            case ConnectionType.Input:

                                for (int c = 0; c < editor.canvas.connections.Count; c++)
                                {
                                    if (editor.canvas.connections[c].to == currentNode)
                                    {
                                        editor.canvas.connections[c].DiscardConnection();
                                    }
                                }

                                for (int c = 0; c < editor.canvas.connections.Count; c++)
                                {
                                    if (editor.canvas.connections[c].froms.Contains(node))
                                    {
                                        editor.canvas.connections[c].DiscardFromConnection(node);
                                    }
                                }

                                if ((handle.info.GetValue(node) as NodeConnection) == null)
                                {
                                    NodeConnection connection = NodeConnection.CreateConnection(node, handle.handle.handlePosition,
                                        currentNode, handleInfo.handle.handlePosition);
                                    handle.info.SetValue(node, connection);
                                    handleInfo.info.SetValue(currentNode, connection);
                                    editor.canvas.connections.Add(connection);
                                }
                                break;
                        }
                    }

                    currentTask = NodeDrawerTask.None;
                    anchorPoint = Vector2.zero;
                    currentNode = null;
                    handleInfo = null;
                }
            }
        }
        
        public static MouseEventPackage DrawHandle(ConnectionType type, Vector2 pos, bool filled)
        {
            MouseEventPackage mouseEvent = new MouseEventPackage(EventType.Ignore, MouseButton.None);
            Rect rect = new Rect(pos, new Vector2(16f, 16f));

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

        #region Connection Drawer Methods

        public static void DrawConnections(Rect rect, List<NodeConnection> connections, ref bool eventUsed)
        {
            for(int c = connections.Count - 1; c >= 0; c--)
            {
                if (connections[c].to == null)
                {
                    connections.Remove(connections[c]);
                }
                DrawConnection(rect, connections[c], ref eventUsed);
            }

            DrawCurrentConnection(rect);
        }

        private static void DrawConnection(Rect rect, NodeConnection connection, ref bool eventUsed)
        {
            List<Vector2> posFroms = new List<Vector2>();

            for (int n = 0; n < connection.froms.Count; n++)
            {
                Vector2 newPos = connection.froms[n].position + new Vector2(connection.froms[n].size.x / 2f, -connection.froms[n].size.y / 2f);
                newPos += connection.fromPositions[n];
                newPos += new Vector2(8f, 8f) + rect.size / 2f;
                posFroms.Add(newPos);
            }

            Vector2 toPos = rect.size / 2f;
            toPos += connection.to.position + new Vector2(-connection.to.size.x / 2f, -connection.to.size.y / 2f);
            toPos += connection.toPosition;
            toPos += new Vector2(-8f, 8f);

            for (int c = 0; c < posFroms.Count; c++)
            {
                Handles.DrawBezier(posFroms[c], toPos, posFroms[c], toPos, Color.white, null, 3f);
            }
        }

        private static void DrawCurrentConnection(Rect rect)
        {
            if (currentTask == NodeDrawerTask.Connect)
            {
                Vector2 anchor = rect.size / 2f + anchorPoint - new Vector2(0f, currentNode.size.y * 0.5f);
                anchor += new Vector2(((int)handleInfo.handle.handleType * 2 - 1) * (currentNode.size.x * 0.5f), 0f);
                anchor += handleInfo.handle.handlePosition;
                anchor += new Vector2(((int)handleInfo.handle.handleType * 2 - 1) * 8f, 8f);
                Handles.DrawBezier(anchor, Event.current.mousePosition, anchor, Event.current.mousePosition, Color.white, null, 6f);
            }
        }

        #endregion
    }
}