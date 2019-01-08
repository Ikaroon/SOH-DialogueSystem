using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Northwind.Essentials;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    public class DialogCanvas : ScriptableObject
    {
        //--------------------------------\\
        //-----------< EDITOR >-----------\\
        //--------------------------------\\

        #if UNITY_EDITOR

        #region Canvas Editor Data

        //The description of the canvas to show people the purpose of the canvas.
        public string canvasDescription;

        //The timestamp of the canvas to enable autosave.
        public string canvasTimestamp;

        #endregion

        #region Canvas Editor Methods

        /// <summary>
        /// Initiates the Canvas with a given name and description
        /// </summary>
        /// <param name="name">The name of the canvas used to call it from the system</param>
        /// <param name="description">The description of the canvas to show people the purpose of the canvas</param>
        public void InitiateCanvas(string name, string description)
        {
            canvasName = name;
            canvasDescription = description;
            startNode = StartNode.CreateNode<StartNode>(Vector2.zero);
            //The saving of the first timestamp: aka "Creation Timestamp"
            canvasTimestamp = System.DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss"); //TODO: Correct Timestamp
        }

        public void PrepareCanvas(string canvasRoot, string contentRoot)
        {
            startNode.PrepareNode(this, canvasRoot, contentRoot);
            foreach (Node node in nodes)
            {
                node.PrepareNode(this, canvasRoot, contentRoot);
            }
        }

        #endregion

        #region Canvas Editor Static Methods

        /// <summary>
        /// Creates a Canvas of type T, initiates it and returns it
        /// </summary>
        /// <typeparam name="T">The Canvas type which should be created</typeparam>
        /// <param name="name">The name used for the Canvas</param>
        /// <param name="description">The description used for the Canvas</param>
        /// <returns>The created Canvas of type T</returns>
        public static T CreateCanvas<T>(string name = "", string description = "") where T : DialogCanvas
        {
            T canvas = ScriptableObject.CreateInstance<T>();
            canvas.InitiateCanvas(name, description);
            return canvas;
        }

        #endregion

        #endif

        //--------------------------------\\
        //-----------< PLAYER >-----------\\
        //--------------------------------\\

        #region Canvas Data

        //The name of the canvas in file instead of filename : This name will be used to call it.
        public string canvasName;

        //A list of all created Nodes referenced in this Canvas
        public List<Node> nodes = new List<Node>();

        //The Start Node
        public StartNode startNode;

        //A list of all created Connections referenced in some Nodes of this Canvas
        public List<NodeConnection> connections = new List<NodeConnection>();

        //A Reference to all defined Points
        public DefinedPointDictionary definedPoints = new DefinedPointDictionary();

        //The type of this canvas
        public enum CanvasType { Playable, Nested };
        public CanvasType canvasType = CanvasType.Playable;

        #endregion

        #region Canvas Methods

        #endregion

        #region Canvas Static Methods

        #endregion
    }

    /// <summary>
    /// A Dictionary which is sorted and serializable
    /// </summary>
    [System.Serializable]
    public class DefinedPointDictionary : SerializableSortedDictionary<string, Node>
    {
        //A Node packing dictionary
    }
}