﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    public static class NodeOperator
    {
        /// <summary>
        /// Collects all NodeHandles in the given node
        /// </summary>
        /// <param name="node">The Node which should expose it's handles</param>
        /// <param name="type">The Type of handle searched</param>
        /// <returns>A List of all collected NodeHandlePackes</returns>
        public static List<NodeHandlePackage> GetConnections(Node node, ConnectionType type)
        {
            List<NodeHandlePackage> fields = new List<NodeHandlePackage>();

            FieldInfo[] connectionFields = node.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < connectionFields.Length; i++)
            {
                NodeHandleAttribute attribute = Attribute.GetCustomAttribute(connectionFields[i], typeof(NodeHandleAttribute)) as NodeHandleAttribute;

                if (attribute != null && attribute.handleType == type)
                {
                    if (connectionFields.GetType() == typeof(NodeConnection))
                    {
                        fields.Add(new NodeHandlePackage(attribute, connectionFields[i]));
                    }
                }
            }

            return fields;
        }

        /// <summary>
        /// Collects all Nodes under a specific root folder and returns them in a Dictionary
        /// </summary>
        /// <param name="root">The root folder of the search query</param>
        /// <returns>The relative path and the Node Type</returns>
        public static Dictionary<string, System.Type> CollectNodeTypes(string root)
        {
            Dictionary<string, System.Type> nodeTypes = new Dictionary<string, Type>();

            string[] assets = AssetDatabase.FindAssets("t:MonoScript", new string[] { root });
            for (int a = 0; a < assets.Length; a++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[a]);
                if (AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(MonoScript))
                {
                    MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                    object[] attributes = script.GetClass().GetCustomAttributes(false);
                    bool attributeContained = false;
                    foreach (object attribute in attributes)
                    {
                        if (attribute.GetType() == typeof(NodeDataAttribute))
                        {
                            attributeContained = true;
                        }
                    }
                    if (script.GetClass().BaseType == typeof(Node) && attributeContained)
                    {
                        string relativePath = GenerateHomogeneousMenu(root, path, script.GetClass().Name);
                        nodeTypes.Add(relativePath, script.GetClass());
                    }
                }
            }

            return nodeTypes;
        }

        public static string GenerateHomogeneousMenu(string root, string path, string className)
        {
            string relativePath = path.Replace(root + "/", "").Replace(".cs", "");
            string[] parts = relativePath.Split('/');
            parts[parts.Length - 1] = parts[parts.Length - 1].Replace(className, className.Replace("Node", ""));
            relativePath = parts[0];
            for (int p = 1; p < parts.Length; p++)
            {
                relativePath += "/" + parts[p];
            }
            return relativePath;
        }
    }
}