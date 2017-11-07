﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpyOnHuman.NodeFramework
{
    public class NodeSaveOperator
    {
        
        #region Save

        public static string Save(ref NodeCanvas canvas, string path = "")
        {
            //Copy Canvas
            NodeCanvas savedCanvas = Object.Instantiate(canvas) as NodeCanvas;

            //Copy all Nodes
            for (int n = 0; n < canvas.nodes.Count; n++)
            {
                savedCanvas.nodes[n] = (Object.Instantiate(canvas.nodes[n]) as Node);
                savedCanvas.nodes[n].name = canvas.nodes[n].name;
            }

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
                    savedCanvas.connections[c].froms[subNodeID] = canvas.connections[c].froms[subNodeID];
                }
                savedCanvas.connections[c].to = canvas.connections[c].to;
            }

            //Rebind Nodes to Connections
            for (int n = 0; n < canvas.nodes.Count; n++)
            {
                List<NodeHandlePackage> InputPackagesNode = NodeOperator.GetConnections(canvas.nodes[n], ConnectionType.Input);
                List<NodeHandlePackage> InputPackagesSaveNode = NodeOperator.GetConnections(savedCanvas.nodes[n], ConnectionType.Input);

                for (int pn = 0; pn < InputPackagesNode.Count; pn++)
                {
                    int savedNodeID = canvas.nodes.IndexOf(InputPackagesNode[pn].info.GetValue(canvas.nodes[n]) as Node);
                    InputPackagesSaveNode[pn].info.SetValue(savedCanvas.nodes[pn], savedCanvas.nodes[savedNodeID]);
                }

                List<NodeHandlePackage> OutputPackagesNode = NodeOperator.GetConnections(canvas.nodes[n], ConnectionType.Output);
                List<NodeHandlePackage> OutputPackagesSaveNode = NodeOperator.GetConnections(savedCanvas.nodes[n], ConnectionType.Output);

                for (int pn = 0; pn < OutputPackagesNode.Count; pn++)
                {
                    int savedNodeID = canvas.nodes.IndexOf(OutputPackagesNode[pn].info.GetValue(canvas.nodes[n]) as Node);
                    OutputPackagesSaveNode[pn].info.SetValue(savedCanvas.nodes[pn], savedCanvas.nodes[savedNodeID]);
                }
            }

            //Get Saving Path
            string newPath = path == "" ? EditorUtility.SaveFilePanelInProject("Save Node Canvas", "Node Canvas", "asset", "", "Assets/") : path;

            //Send copied files and canvas to saving
            return SaveCanvas(savedCanvas, newPath);
        }
        
        public static string SaveCanvas(NodeCanvas canvas, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }
            AssetDatabase.CreateAsset(canvas, path);
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
        
        public static string Load(ref NodeCanvas canvas)
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

        public static string LoadCanvas(ref NodeCanvas canvas, string path)
        {
            path = path.Replace(Application.dataPath, "Assets");
            NodeCanvas loadedCanvas = AssetDatabase.LoadAssetAtPath<NodeCanvas>(path);

            if (!loadedCanvas)
            {
                return "";
            }

            //Copy Canvas
            canvas = Object.Instantiate(loadedCanvas) as NodeCanvas;

            //Copy all Nodes
            for (int n = 0; n < canvas.nodes.Count; n++)
            {
                canvas.nodes[n] = Object.Instantiate(loadedCanvas.nodes[n]) as Node;
                canvas.nodes[n].name = loadedCanvas.nodes[n].name;
            }

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
                    canvas.connections[c].froms[subNodeID] = loadedCanvas.connections[c].froms[subNodeID];
                }
                canvas.connections[c].to = loadedCanvas.connections[c].to;
            }

            //Rebind Nodes to Connections
            for (int n = 0; n < loadedCanvas.nodes.Count; n++)
            {
                List<NodeHandlePackage> InputPackagesNode = NodeOperator.GetConnections(loadedCanvas.nodes[n], ConnectionType.Input);
                List<NodeHandlePackage> InputPackagesSaveNode = NodeOperator.GetConnections(canvas.nodes[n], ConnectionType.Input);

                for (int pn = 0; pn < InputPackagesNode.Count; pn++)
                {
                    int savedNodeID = loadedCanvas.nodes.IndexOf(InputPackagesNode[pn].info.GetValue(loadedCanvas.nodes[n]) as Node);
                    InputPackagesSaveNode[pn].info.SetValue(canvas.nodes[pn], canvas.nodes[savedNodeID]);
                }

                List<NodeHandlePackage> OutputPackagesNode = NodeOperator.GetConnections(loadedCanvas.nodes[n], ConnectionType.Output);
                List<NodeHandlePackage> OutputPackagesSaveNode = NodeOperator.GetConnections(canvas.nodes[n], ConnectionType.Output);

                for (int pn = 0; pn < OutputPackagesNode.Count; pn++)
                {
                    int savedNodeID = loadedCanvas.nodes.IndexOf(OutputPackagesNode[pn].info.GetValue(loadedCanvas.nodes[n]) as Node);
                    OutputPackagesSaveNode[pn].info.SetValue(canvas.nodes[pn], canvas.nodes[savedNodeID]);
                }
            }
            return path;
        }

        #endregion

    }
}