using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    public class Node : ScriptableObject
    {
        //--------------------------------\\
        //-----------< EDITOR >-----------\\
        //--------------------------------\\

        #if UNITY_EDITOR

        #region Node Editor Data

        //The position of the node. This will be deleted on build time
        public Vector2 position;
        //The size of the node. This will be deleted on build time
        public Vector2 size;

        #endregion

        #region Node Editor Methods

        /// <summary>
        /// Initiates the Node with a given position
        /// </summary>
        /// <param name="pos">The position this node should start at</param>
        public void InitiateNode(Vector2 pos)
        {
            position = pos;
        }

        #endregion

        #region Node Editor Static Methods

        /// <summary>
        /// Creates a Node of type T, initiates it and returns it
        /// </summary>
        /// <typeparam name="T">The Node type which should be created</typeparam>
        /// <param name="pos">The position the node should start at</param>
        /// <returns>The created Node of type T</returns>
        public static T CreateNode<T>(Vector2 pos) where T : Node
        {
            T node = ScriptableObject.CreateInstance<T>();
            node.InitiateNode(pos);
            return node;
        }

        /// <summary>
        /// Creates a Node of type nodeType, initiates it and returns it
        /// </summary>
        /// <param name="nodeType">The Node type which should be created</param>
        /// <param name="pos">The position the node should start at</param>
        /// <returns>The created Node of type nodeType</returns>
        public static Node CreateNode(System.Type nodeType, Vector2 pos)
        {
            Node node = (Node)ScriptableObject.CreateInstance(nodeType);
            node.InitiateNode(pos);
            return node;
        }

        #endregion

        #endif

        //--------------------------------\\
        //-----------< PLAYER >-----------\\
        //--------------------------------\\

        #region Node Data

        #endregion

        #region Node Methods

        #endregion

        #region Node Static Methods

        #endregion
    }
}