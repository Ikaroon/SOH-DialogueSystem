using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Northwind.Essentials;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    public abstract class Node : ScriptableObject
    {
        //--------------------------------\\
        //-----------< EDITOR >-----------\\
        //--------------------------------\\

        #if UNITY_EDITOR

        #region Node Editor Data

        [HideInInspector]
        //The position of the node. This will be deleted on build time
        public Vector2 position;

        [HideInInspector]
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

            object[] attributes = this.GetType().GetCustomAttributes(false);
            NodeDataAttribute nodeData = new NodeDataAttribute("", "", 0f, 0f);
            foreach (object attribute in attributes)
            {
                if (attribute.GetType() == typeof(NodeDataAttribute))
                {
                    nodeData = attribute as NodeDataAttribute;
                }
            }

            size = nodeData.nodeSize;
        }

        public virtual void OnDelete(DialogCanvas canvas)
        {

        }

        public virtual void PrepareNode(DialogCanvas canvas, string canvasRoot, string contentRoot)
        {

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

        public virtual bool IsAuto()
        {
            return false;
        }

        #endregion

        #region Node Methods

        public virtual Node PrepareNode()
        {
            return this;
        }

        public virtual Node UpdateNode()
        {
            return this;
        }

        public virtual Node LateUpdateNode()
        {
            return this;
        }

        public virtual void DisplayNode(Rect rect)
        {

        }

        #endregion

        #region Node Static Methods

        #endregion
    }
}