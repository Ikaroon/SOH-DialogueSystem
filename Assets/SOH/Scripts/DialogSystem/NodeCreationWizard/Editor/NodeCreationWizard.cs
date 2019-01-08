using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using System.IO;

using Northwind.Essentials;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    public class NodeHandleData
    {
        //NodeHandleAttribute(int ID, ConnectionType type, float y = -1f, bool fixedPosition = true, string tooltip = "")
        
        public float y;
        public bool fixedPosition;
        public string tooltip;
    }

    public class NodeHandleEditor : EditorWindow
    {

        public NodeHandleData handleData;

        public static void Initialize(string handleName, NodeHandleData data)
        {
            NodeHandleEditor window = CreateInstance<NodeHandleEditor>();
            window.handleData = data;
            window.titleContent = new GUIContent("Handle Editor: " + handleName);
            window.ShowUtility();
        }

        private void OnEnable()
        {
            this.minSize = new Vector2(256f, 64f);
            this.maxSize = new Vector2(256f, 64f);
        }

        //The GUI call for this Window
        void OnGUI()
        {
            handleData.y = EditorGUILayout.FloatField(new GUIContent("Y"), handleData.y);
            handleData.fixedPosition = EditorGUILayout.Toggle(new GUIContent("Fixed"), handleData.fixedPosition);
            handleData.tooltip = EditorGUILayout.TextField(new GUIContent("Tooltip"), handleData.tooltip);
        }

        private void OnLostFocus()
        {
            Close();
        }
    }

    public class NodeCreationWizard : EditorWindow
    {

        #region Creation Data

        private string resultPath = "Assets";
        private string nodeTemplate = "";
        private string resultTemplate = "";

        #endregion

        #region Helper Methods

        private string GetSelectedPathOrFallback()
        {
            string path = "Assets";

            foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
            return path;
        }

        private string GetNodeTemplate()
        {
            string templatePath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            templatePath = templatePath.Replace(this.GetType().ToString().Replace(this.GetType().Namespace + ".", "") + ".cs", "NodeTemplate.ntp");
            return FileOperator.LoadStringFromFile(FileOperator.GetProjectPath() + templatePath);
        }

        private string GetHandleTemplate()
        {
            string templatePath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            templatePath = templatePath.Replace(this.GetType().ToString().Replace(this.GetType().Namespace + ".", "") + ".cs", "NodeHandleTemplate.ntp");
            return FileOperator.LoadStringFromFile(FileOperator.GetProjectPath() + templatePath);
        }

        private string GetEditorTemplate()
        {
            string templatePath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            templatePath = templatePath.Replace(this.GetType().ToString().Replace(this.GetType().Namespace + ".", "") + ".cs", "NodeEditorTemplate.ntp");
            return FileOperator.LoadStringFromFile(FileOperator.GetProjectPath() + templatePath);
        }

        #endregion

        #region Window Methods

        //The initialization Method for the Window
        [MenuItem("Assets/Create/Node C#", priority = 0)]
        static void Init()
        {
            NodeCreationWizard window = CreateInstance<NodeCreationWizard>();
            window.ShowUtility();
        }

        private void OnEnable()
        {
            this.titleContent = new GUIContent("Node Creation Wizard");
            this.minSize = new Vector2(800f, 600f);
            this.maxSize = new Vector2(800f, 600f);

            resultTemplate = nodeTemplate = GetNodeTemplate();
            resultPath = GetSelectedPathOrFallback();

            //FileOperator.SaveToStringFile("using UnityEngine;\nusing UnityEditor;", FileOperator.GetProjectPath() + GetSelectedPathOrFallback() + "/Test.cs");
            //AssetDatabase.ImportAsset(GetSelectedPathOrFallback() + "/Test.cs");
        }

        private void OnDestroy()
        {

        }

        private void OnDisable()
        {
            OnDestroy();
        }

        private void OnLostFocus()
        {/*
            EditorApplication.Beep();
            bool close = EditorUtility.DisplayDialog("Node Creation Wizard", "Do you want to close the Node Creation Wizard, your changes will be lost!", "Yes", "Cancel");
            if (close)
            {
                Focus();
                this.Close();
            } else
            {
                Focus();
            }*/
        }

        private float EaseValue(float x)
        {
            return ((x - 1) * (x - 1)) * x + (((-(x * x)) + 1) * (1 - x));
        }

        private void Update()
        {
            timer = Mathf.Max(0f, timer - 0.01f);
            viewSize = Vector2.Lerp(oldSize, nodeSize, EaseValue(timer / blendTime));

            if (!fixedSize)
            {
                Vector2 targetVal = new Vector2(Mathf.Cos((float)EditorApplication.timeSinceStartup) * 0.5f + 0.5f, Mathf.Sin((float)EditorApplication.timeSinceStartup) * 0.5f + 0.5f) * 64f;
                targetVal = Northwind.Essentials.VectorMath.Step(targetVal, 16f);
                if (targetVal != targetAddedDemoSize)
                {
                    oldAddedDemoSize = addedDemoSize;
                    targetAddedDemoSize = targetVal;
                    addedTimer = addedBlendTime;
                }

                addedTimer = Mathf.Max(0f, addedTimer - 0.025f);
                addedDemoSize = Vector2.Lerp(oldAddedDemoSize, targetAddedDemoSize, EaseValue(addedTimer / addedBlendTime));
            }
            else
            {
                if (Vector2.zero != targetAddedDemoSize)
                {
                    oldAddedDemoSize = addedDemoSize;
                    targetAddedDemoSize = Vector2.zero;
                    addedTimer = addedBlendTime;
                }

                addedTimer = Mathf.Max(0f, addedTimer - 0.025f);
                addedDemoSize = Vector2.Lerp(oldAddedDemoSize, targetAddedDemoSize, EaseValue(addedTimer / addedBlendTime));
            }

            maxSize = minSize = new Vector2(Mathf.Max(800f, 220f + viewSize.x + Mathf.Sign(viewSize.x) * (64f + (fixedSize ? 0f : 64f)) ), 600f);
            Rect pos = position;
            pos.size = minSize;

            if (readd)
            {
                readd = false;
                tempHandles.Clear();

                for (int ih = 0; ih < inputHandles.Count; ih++)
                {
                    tempHandles.Add(new NodeHandleAttribute(ih, ConnectionType.Input, inputHandles[ih].y, inputHandles[ih].fixedPosition, inputHandles[ih].tooltip));
                }

                for (int oh = 0; oh < outputHandles.Count; oh++)
                {
                    tempHandles.Add(new NodeHandleAttribute(oh, ConnectionType.Output, outputHandles[oh].y, outputHandles[oh].fixedPosition, outputHandles[oh].tooltip));
                }
            }

            Repaint();

        }

        string nodeName = "";
        string nodeDescription = "";

        Vector2 nodeSize = new Vector2(256f, 64f);
        Vector2 viewSize = new Vector2(0f, 0f);
        Vector2 oldSize = new Vector2(0f, 0f);

        Vector2 targetAddedDemoSize = new Vector2(0f, 0f);
        Vector2 addedDemoSize = new Vector2(0f, 0f);
        Vector2 oldAddedDemoSize = new Vector2(0f, 0f);
        float addedTimer = 0f;
        float addedBlendTime = 1f;

        float timer = 0f;
        const float blendTime = 1f;

        bool fixedSize = true;

        Color nodeColor = Color.white;

        List<NodeHandleData> inputHandles = new List<NodeHandleData>();
        List<NodeHandleData> outputHandles = new List<NodeHandleData>();
        
        List<NodeHandleAttribute> tempHandles = new List<NodeHandleAttribute>();
        bool readd = false;

        //The GUI call for this Window
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10f, 10f, 200f, position.height - 20f));

            nodeName = EditorGUI.TextField(new Rect(0f, 0f, 200f, 20f), nodeName);
            nodeDescription = EditorGUI.TextArea(new Rect(0f, 30f, 200f, 60f), nodeDescription);

            EditorGUI.BeginChangeCheck();

            nodeSize.x = EditorGUI.DelayedFloatField(new Rect(0f, 100f, 95f, 20f), nodeSize.x);
            nodeSize.y = EditorGUI.DelayedFloatField(new Rect(105f, 100f, 95f, 20f), nodeSize.y);

            if (EditorGUI.EndChangeCheck() && viewSize != nodeSize)
            {
                nodeSize = Northwind.Essentials.VectorMath.Step(nodeSize, 16f);

                oldSize = viewSize;
                timer = blendTime;
            }

            fixedSize = EditorGUI.Toggle(new Rect(0f, 120f, 200f, 20f), new GUIContent("Fixed"), fixedSize);

            nodeColor = EditorGUI.ColorField(new Rect(0f, 150f, 200f, 20f), new GUIContent("Color"), nodeColor);

            EditorGUI.BeginChangeCheck();

            // Inputs and Outputs
            EditorGUI.LabelField(new Rect(0f, 180f, 100f, 20f), new GUIContent("Inputs"), GUI.skin.GetStyle("LargeButtonLeft"));

            GUILayout.BeginArea(new Rect(4f, 200f, 96f, 200f), GUI.skin.GetStyle("CN Box"));

            for (int ih = 0; ih < inputHandles.Count; ih++)
            {
                if (GUI.Button(new Rect(4f, 4f + ih * 24f, 88f, 20f), "Input " + ih))
                {
                    NodeHandleEditor.Initialize("Input " + ih, inputHandles[ih]);
                }
            }

            GUILayout.EndArea();

            if (GUI.Button(new Rect(0f, 400f, 50f, 20f), "+", GUI.skin.GetStyle("LargeButtonLeft")))
            {
                if (inputHandles.Count < 8)
                {
                    inputHandles.Add(new NodeHandleData());
                }
            }

            if (GUI.Button(new Rect(50f, 400f, 50f, 20f), "-", GUI.skin.GetStyle("LargeButtonMid")))
            {
                inputHandles.RemoveAt(inputHandles.Count - 1);
            }

            EditorGUI.LabelField(new Rect(100f, 180f, 100f, 20f), new GUIContent("Outputs"), GUI.skin.GetStyle("LargeButtonRight"));

            GUILayout.BeginArea(new Rect(100f, 200f, 96f, 200f), GUI.skin.GetStyle("CN Box"));

            for (int oh = 0; oh < outputHandles.Count; oh++)
            {
                if (GUI.Button(new Rect(4f, 4f + oh * 24f, 88f, 20f), "Output " + oh))
                {
                    NodeHandleEditor.Initialize("Output " + oh, outputHandles[oh]);
                }
            }

            GUILayout.EndArea();

            if (GUI.Button(new Rect(100f, 400f, 50f, 20f), "+", GUI.skin.GetStyle("LargeButtonMid")))
            {
                if (outputHandles.Count < 8)
                {
                    outputHandles.Add(new NodeHandleData());
                }
            }

            if (GUI.Button(new Rect(150f, 400f, 50f, 20f), "-", GUI.skin.GetStyle("LargeButtonRight")))
            {
                outputHandles.RemoveAt(outputHandles.Count - 1);
            }

            if (EditorGUI.EndChangeCheck())
            {
                readd = true;
            }


            // Creation buttons
            if (GUI.Button(new Rect(0f, position.height - 40f, 95f, 20f), "Create"))
            {
                string template = GetNodeTemplate();

                // Node Data
                template = template.Replace("#NODE_NAME#", nodeName);
                template = template.Replace("#NODE_DESCRIPTION#", nodeDescription);
                template = template.Replace("#NODE_WIDTH#", nodeSize.x + "f");
                template = template.Replace("#NODE_HEIGHT#", nodeSize.y + "f");
                template = template.Replace("#NODE_FIXED#", fixedSize ? "true" : "false");
                template = template.Replace("#NODE_COLOR#", nodeColor.r + "f, " + nodeColor.g + "f, " + nodeColor.b + "f");

                // Class Data
                template = template.Replace("#NODE_CLASSNAME#", nodeName.Replace(" ", ""));

                // Input Handles
                if (inputHandles.Count <= 0)
                {
                    template = template.Replace("#NODE_INPUT_HANDLES#", "// No Inputs");
                }
                else
                {
                    string handles = "";

                    for (int ih = 0; ih < inputHandles.Count; ih++)
                    {
                        string handleTemplate = (ih > 0 ? "\t\t" : "") + GetHandleTemplate();

                        handleTemplate = handleTemplate.Replace("#HANDLE_ID#", ih + "");
                        handleTemplate = handleTemplate.Replace("#HANDLE_CONNECTION#", "ConnectionType.Input");
                        handleTemplate = handleTemplate.Replace("#HANDLE_Y#", inputHandles[ih].y + "f");
                        handleTemplate = handleTemplate.Replace("#HANDLE_FIXED#", inputHandles[ih].fixedPosition ? "true" : "false");
                        handleTemplate = handleTemplate.Replace("#HANDLE_TOOLTIP#", inputHandles[ih].tooltip);

                        handleTemplate = handleTemplate.Replace("#HANDLE_CONNECTION_SMALL#", "input");

                        handles += handleTemplate + "\n\n";
                    }

                    template = template.Replace("#NODE_INPUT_HANDLES#", handles);
                }

                // Output Handles
                if (outputHandles.Count <= 0)
                {
                    template = template.Replace("#NODE_OUTPUT_HANDLES#", "// No Outputs");
                }
                else
                {
                    string handles = "";

                    for (int oh = 0; oh < outputHandles.Count; oh++)
                    {
                        string handleTemplate = (oh > 0 ? "\t\t" : "") + GetHandleTemplate();

                        handleTemplate = handleTemplate.Replace("#HANDLE_ID#", oh + "");
                        handleTemplate = handleTemplate.Replace("#HANDLE_CONNECTION#", "ConnectionType.Output");
                        handleTemplate = handleTemplate.Replace("#HANDLE_Y#", outputHandles[oh].y + "f");
                        handleTemplate = handleTemplate.Replace("#HANDLE_FIXED#", outputHandles[oh].fixedPosition ? "true" : "false");
                        handleTemplate = handleTemplate.Replace("#HANDLE_TOOLTIP#", outputHandles[oh].tooltip);

                        handleTemplate = handleTemplate.Replace("#HANDLE_CONNECTION_SMALL#", "output");

                        handles += handleTemplate + "\n\n";
                    }

                    template = template.Replace("#NODE_OUTPUT_HANDLES#", handles);
                }

                FileOperator.SaveToStringFile(template, FileOperator.GetProjectPath() + GetSelectedPathOrFallback() + "/" + nodeName.Replace(" ", "") + "Node.cs");

                template = GetEditorTemplate();

                template = template.Replace("#NODE_CLASS#", nodeName.Replace(" ", "") + "Node");

                FileOperator.SaveToStringFile(template, FileOperator.GetProjectPath() + GetSelectedPathOrFallback() + "/Editor/" + nodeName.Replace(" ", "") + "NodeEditor.cs");

                AssetDatabase.Refresh();

                Close();
            }

            if (GUI.Button(new Rect(105f, position.height - 40f, 95f, 20f), "Cancel"))
            {
                Close();
            }

            GUILayout.EndArea();

            //GUILayout.Label(GetSelectedPathOrFallback());

            GUILayout.BeginArea(new Rect(220f, 0f, position.width - 220f, position.height), GUI.skin.GetStyle("GroupBox"));
            
            NodePreviewer.DrawNodePreview(new Vector2((position.width - 220f) / 2f - (viewSize.x + addedDemoSize.x + 64f) / 2f, position.height / 2f - (viewSize.y + addedDemoSize.y + 32f) / 2f), null, new NodeDataAttribute(nodeName, nodeDescription, viewSize.x + addedDemoSize.x, viewSize.y + addedDemoSize.y, red: nodeColor.r, green: nodeColor.g, blue: nodeColor.b),
                tempHandles.ToArray());

            GUILayout.EndArea();
        }

        #endregion

    }
}