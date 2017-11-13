using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    public class NodeSaveOperator : Editor
    {
        
        #region Save

        public static string Save(ref DialogCanvas canvas, string path = "")
        {
            //Copy Canvas
            DialogCanvas savedCanvas;

            string npath = path.Replace(Application.dataPath, "Assets");
            DialogCanvas loadedCanvas = AssetDatabase.LoadAssetAtPath<DialogCanvas>(npath);
            if (loadedCanvas)
            {
                savedCanvas = loadedCanvas;

                for (int n = savedCanvas.nodes.Count - 1; n >= 0; n--)
                {
                    DestroyImmediate(savedCanvas.nodes[n], true);
                }

                for (int c = savedCanvas.connections.Count - 1; c >= 0; c--)
                {
                    DestroyImmediate(savedCanvas.connections[c], true);
                }

                DestroyImmediate(savedCanvas.startNode, true);

                savedCanvas.nodes = new List<Node>(new Node[canvas.nodes.Count]);
                savedCanvas.connections = new List<NodeConnection>(new NodeConnection[canvas.connections.Count]);

                savedCanvas.canvasName = canvas.canvasName;
                savedCanvas.canvasDescription = canvas.canvasDescription;

            } else
            {
                savedCanvas = Object.Instantiate(canvas) as DialogCanvas;
            }

            savedCanvas.canvasTimestamp = System.DateTime.Now.ToString("yyyy / MM / dd - HH:mm: ss");

            //Copy all Nodes
            for (int n = canvas.nodes.Count - 1; n >= 0; n--)
            {
                if (canvas.nodes[n] == null)
                {
                    savedCanvas.nodes.RemoveAt(n);
                    continue;
                }
                savedCanvas.nodes[n] = (Object.Instantiate(canvas.nodes[n]) as Node);
                savedCanvas.nodes[n].name = canvas.nodes[n].name;
            }

            //Copy Start Node
            savedCanvas.startNode = (Object.Instantiate(canvas.startNode) as StartNode);
            savedCanvas.startNode.name = canvas.startNode.name;

            //Copy all Connections
            for (int c = 0; c < canvas.connections.Count; c++)
            {
                savedCanvas.connections[c] = (Object.Instantiate(canvas.connections[c]) as NodeConnection);
                savedCanvas.connections[c].name = canvas.connections[c].name;
            }

            //Rebind Connections to Nodes
            for (int c = 0; c < canvas.connections.Count; c++)
            {
                for (int subNodeID = 0; subNodeID < canvas.connections[c].froms.Count; subNodeID++)
                {
                    if (canvas.connections[c].froms[subNodeID] == canvas.startNode)
                    {
                        savedCanvas.connections[c].froms[subNodeID] = savedCanvas.startNode;
                    }
                    else
                    {
                        savedCanvas.connections[c].froms[subNodeID] = savedCanvas.nodes[canvas.nodes.IndexOf(canvas.connections[c].froms[subNodeID])];
                    }
                }

                if (canvas.connections[c].to == canvas.startNode)
                {
                    savedCanvas.connections[c].to = savedCanvas.startNode;
                }
                else
                {
                    savedCanvas.connections[c].to = savedCanvas.nodes[canvas.nodes.IndexOf(canvas.connections[c].to)];
                }
            }

            //Rebind Nodes to Connections
            for (int c = 0; c < savedCanvas.connections.Count; c++)
            {
                for (int subNodeID = 0; subNodeID < savedCanvas.connections[c].froms.Count; subNodeID++)
                {
                    List<NodeHandlePackage> fromsPackage = NodeOperator.GetConnections(savedCanvas.connections[c].froms[subNodeID], ConnectionType.Output);
                    for (int fp = 0; fp < fromsPackage.Count; fp++)
                    {
                        if (fromsPackage[fp].handle.ID == savedCanvas.connections[c].fromAttributes[subNodeID].ID)
                        {
                            fromsPackage[fp].info.SetValue(savedCanvas.connections[c].froms[subNodeID], savedCanvas.connections[c]);
                        }
                    }
                }
                List<NodeHandlePackage> tosPackage = NodeOperator.GetConnections(savedCanvas.connections[c].to, ConnectionType.Input);
                for (int fp = 0; fp < tosPackage.Count; fp++)
                {
                    if (tosPackage[fp].handle.ID == savedCanvas.connections[c].toAttribute.ID)
                    {
                        tosPackage[fp].info.SetValue(savedCanvas.connections[c].to, savedCanvas.connections[c]);
                    }
                }
            }

            //Rebind Nodes to DefinedPoints
            List<string> keys = new List<string>(canvas.definedPoints.Keys);
            for (int k = 0; k < keys.Count; k++)
            {
                int nodeID = canvas.nodes.IndexOf(canvas.definedPoints[keys[k]]);
                savedCanvas.definedPoints[keys[k]] = savedCanvas.nodes[nodeID];
            }

            //Get Saving Path
            string newPath = path == "" ? EditorUtility.SaveFilePanelInProject("Save Node Canvas", "Node Canvas", "asset", "", "Assets/") : path;

            //Send copied files and canvas to saving
            return SaveCanvas(savedCanvas, newPath);
        }

        public static string SaveCanvas(DialogCanvas canvas, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }
            if (AssetDatabase.GetAssetPath(canvas) == "")
            {
                AssetDatabase.CreateAsset(canvas, path);
            }
            SaveAllSubData(new ScriptableObject[] { canvas.startNode }, canvas);
            SaveAllSubData(canvas.nodes.ToArray(), canvas);
            SaveAllSubData(canvas.connections.ToArray(), canvas);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return path;
        }

        public static string SaveAllSubData(ScriptableObject[] subAssets, ScriptableObject originalAsset)
        {
            int lFails = 0;
            foreach (ScriptableObject asset in subAssets)
            {
                lFails += SaveSubData(asset, originalAsset);
            }
            return "Failed to save SubAssets: " + lFails;
        }

        public static int SaveSubData(ScriptableObject subAsset, ScriptableObject originalAsset)
        {
            if (subAsset && originalAsset)
            {
                AssetDatabase.AddObjectToAsset(subAsset, originalAsset);
                subAsset.hideFlags = HideFlags.HideInHierarchy;
                return 0;
            }
            return 1;
        }

        #endregion

        #region Load
        
        public static string Load(ref DialogCanvas canvas)
        {
            string newPath = EditorUtility.OpenFilePanel("Load Node Canvas", "Assets/", "asset");
            if (string.IsNullOrEmpty(newPath))
            {
                return "";
            }
            else if (!newPath.Contains(Application.dataPath))
            {
                return "";
            }
            return LoadCanvas(ref canvas, newPath);
        }

        public static string LoadCanvas(ref DialogCanvas canvas, string path)
        {
            path = path.Replace(Application.dataPath, "Assets");
            DialogCanvas loadedCanvas = AssetDatabase.LoadAssetAtPath<DialogCanvas>(path);

            if (!loadedCanvas)
            {
                return "";
            }

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

            //Copy Canvas
            canvas = Object.Instantiate(loadedCanvas) as DialogCanvas;

            //Copy all Nodes
            for (int n = canvas.nodes.Count - 1; n >= 0 ; n--)
            {
                if (canvas.nodes[n] == null)
                {
                    canvas.nodes.RemoveAt(n);
                    continue;
                }
                canvas.nodes[n] = Object.Instantiate(loadedCanvas.nodes[n]) as Node;
                canvas.nodes[n].name = loadedCanvas.nodes[n].name;
            }

            //Copy Start Node
            canvas.startNode = (Object.Instantiate(loadedCanvas.startNode) as StartNode);
            canvas.startNode.name = loadedCanvas.startNode.name;

            //Copy all Connections
            for (int c = 0; c < canvas.connections.Count; c++)
            {
                canvas.connections[c] = (Object.Instantiate(loadedCanvas.connections[c]) as NodeConnection);
                canvas.connections[c].name = loadedCanvas.connections[c].name;
            }

            //Rebind Connections to Nodes
            for (int c = 0; c < loadedCanvas.connections.Count; c++)
            {
                for (int subNodeID = 0; subNodeID < loadedCanvas.connections[c].froms.Count; subNodeID++)
                {
                    if (loadedCanvas.connections[c].froms[subNodeID] == loadedCanvas.startNode)
                    {
                        canvas.connections[c].froms[subNodeID] = canvas.startNode;
                    }
                    else
                    {
                        canvas.connections[c].froms[subNodeID] = canvas.nodes[loadedCanvas.nodes.IndexOf(loadedCanvas.connections[c].froms[subNodeID])];
                    }
                }

                if (loadedCanvas.connections[c].to == loadedCanvas.startNode)
                {
                    canvas.connections[c].to = canvas.startNode;
                }
                else
                {
                    canvas.connections[c].to = canvas.nodes[loadedCanvas.nodes.IndexOf(loadedCanvas.connections[c].to)];
                }
            }

            //Rebind Nodes to Connections
            for (int c = 0; c < canvas.connections.Count; c++)
            {
                for (int subNodeID = 0; subNodeID < canvas.connections[c].froms.Count; subNodeID++)
                {
                    List<NodeHandlePackage> fromsPackage = NodeOperator.GetConnections(canvas.connections[c].froms[subNodeID], ConnectionType.Output);
                    for (int fp = 0; fp < fromsPackage.Count; fp++)
                    {
                        if (fromsPackage[fp].handle.ID == canvas.connections[c].fromAttributes[subNodeID].ID)
                        {
                            fromsPackage[fp].info.SetValue(canvas.connections[c].froms[subNodeID], canvas.connections[c]);
                        }
                    }
                }
                List<NodeHandlePackage> tosPackage = NodeOperator.GetConnections(canvas.connections[c].to, ConnectionType.Input);
                for (int fp = 0; fp < tosPackage.Count; fp++)
                {
                    if (tosPackage[fp].handle.ID == canvas.connections[c].toAttribute.ID)
                    {
                        tosPackage[fp].info.SetValue(canvas.connections[c].to, canvas.connections[c]);
                    }
                }
            }

            //Rebind Nodes to DefinedPoints
            List<string> keys = new List<string>(loadedCanvas.definedPoints.Keys);
            for (int k = 0; k < keys.Count; k++)
            {
                int nodeID = loadedCanvas.nodes.IndexOf(loadedCanvas.definedPoints[keys[k]]);
                canvas.definedPoints[keys[k]] = canvas.nodes[nodeID];
            }

            return path;
        }

        #endregion

        #region Export

        public static string Export(ref DialogCanvas canvas, string path = "")
        {
            //Copy Canvas
            DialogCanvas savedCanvas = Object.Instantiate(canvas) as DialogCanvas;

            //Copy all Nodes
            for (int n = 0; n < canvas.nodes.Count; n++)
            {
                savedCanvas.nodes[n] = (Object.Instantiate(canvas.nodes[n]) as Node);
                savedCanvas.nodes[n].name = canvas.nodes[n].name;
            }

            //Copy Start Node
            savedCanvas.startNode = (Object.Instantiate(canvas.startNode) as StartNode);
            savedCanvas.startNode.name = canvas.startNode.name;

            //Copy all Connections
            for (int c = 0; c < canvas.connections.Count; c++)
            {
                savedCanvas.connections[c] = (Object.Instantiate(canvas.connections[c]) as NodeConnection);
                savedCanvas.connections[c].name = canvas.connections[c].name;
            }

            //Rebind Connections to Nodes
            for (int c = 0; c < canvas.connections.Count; c++)
            {
                for (int subNodeID = 0; subNodeID < canvas.connections[c].froms.Count; subNodeID++)
                {
                    if (canvas.connections[c].froms[subNodeID] == canvas.startNode)
                    {
                        savedCanvas.connections[c].froms[subNodeID] = savedCanvas.startNode;
                    }
                    else
                    {
                        savedCanvas.connections[c].froms[subNodeID] = savedCanvas.nodes[canvas.nodes.IndexOf(canvas.connections[c].froms[subNodeID])];
                    }
                }

                if (canvas.connections[c].to == canvas.startNode)
                {
                    savedCanvas.connections[c].to = savedCanvas.startNode;
                }
                else
                {
                    savedCanvas.connections[c].to = savedCanvas.nodes[canvas.nodes.IndexOf(canvas.connections[c].to)];
                }
            }

            //Rebind Nodes to Connections
            for (int c = 0; c < savedCanvas.connections.Count; c++)
            {
                for (int subNodeID = 0; subNodeID < savedCanvas.connections[c].froms.Count; subNodeID++)
                {
                    List<NodeHandlePackage> fromsPackage = NodeOperator.GetConnections(savedCanvas.connections[c].froms[subNodeID], ConnectionType.Output);
                    for (int fp = 0; fp < fromsPackage.Count; fp++)
                    {
                        if (fromsPackage[fp].handle.ID == savedCanvas.connections[c].fromAttributes[subNodeID].ID)
                        {
                            fromsPackage[fp].info.SetValue(savedCanvas.connections[c].froms[subNodeID], savedCanvas.connections[c]);
                        }
                    }
                }
                List<NodeHandlePackage> tosPackage = NodeOperator.GetConnections(savedCanvas.connections[c].to, ConnectionType.Input);
                for (int fp = 0; fp < tosPackage.Count; fp++)
                {
                    if (tosPackage[fp].handle.ID == savedCanvas.connections[c].toAttribute.ID)
                    {
                        tosPackage[fp].info.SetValue(savedCanvas.connections[c].to, savedCanvas.connections[c]);
                    }
                }
            }

            //Get Saving Path
            string newPath = path == "" ? EditorUtility.SaveFilePanelInProject("Export Node Canvas", "Node Canvas", "json", "", "Assets/") : path;

            //Send copied files and canvas to saving
            return ExportCanvas(savedCanvas, newPath);
        }

        public static string ExportCanvas(DialogCanvas canvas, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }
            string textFile = JsonUtility.ToJson(canvas);
            string absPath = Application.dataPath + "/" + path.Replace("Assets/", "");
            if (File.Exists(absPath))
            {
                File.Delete(absPath);
            }
            using (FileStream fs = File.Create(absPath))
            {
                System.Byte[] info = new System.Text.UTF8Encoding(true).GetBytes(textFile);
                fs.Write(info, 0, info.Length);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return path;
        }

        #endregion

    }
}