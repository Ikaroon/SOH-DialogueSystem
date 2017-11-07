using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpyOnHuman.NodeFramework
{
    public class NodeEditor : EditorWindow
    {
        #region Editor Content

        public NodeCanvas canvas = null;
        public string oldPath = "";

        #endregion

        #region Editor Task Data

        public enum NodeEditorTask { None, ResizeSidebar };
        public NodeEditorTask currentTask = NodeEditorTask.None;

        #endregion

        #region Editor Visual Data

        private float sidebarWidth = 320f;

        #endregion

        #region Window Methods

        //The initialization Method for the Window
        [MenuItem("SpyOnHuman/Dialog Editor")]
        static void Init()
        {
            NodeEditor window = (NodeEditor)EditorWindow.GetWindow(typeof(NodeEditor));
            window.Show();
        }

        //The GUI call for this Window
        void OnGUI()
        {
            //Display Toolbar
            ToolbarDrawer.DrawToolbar(new Rect(0f, 0f, this.position.width, 18f), this);

            //Begin Bottom Part of Window
            GUILayout.BeginArea(new Rect(0f, 18f, this.position.width, this.position.height - 18f));

            //Resize Sidebar and Display
            SidebarDrawer.DrawSidebar(new Rect(-1f, -1f, sidebarWidth, this.position.height - 16f), this, ref sidebarWidth, 240f, this.position.width / 2f);

            //Display Node Editor Frame
            //Rect nodeField = new Rect(sidebarWidth - 1f, 0f, this.position.width - (sidebarWidth - 1f), this.position.height - 16f);
            //TODO: Node Field GUI

            GUILayout.EndArea();

            //-----------------------------------------------------------------------------------------

            //Reset Events
            if (currentTask == NodeEditorTask.ResizeSidebar && Event.current.isMouse && Event.current.type == EventType.MouseUp)
            {
                currentTask = NodeEditorTask.None;
            }
        }

        #endregion

    }
}