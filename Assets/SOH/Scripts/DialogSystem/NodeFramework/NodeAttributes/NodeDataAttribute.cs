using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NodeDataAttribute : Attribute
    {

        #region Attribute Data

        //The name of the node in the system
        public readonly string nodeName;
        //The description of the node in the system
        public readonly string nodeDescription;

        //The min size of the node
        public readonly Vector2 nodeSize;

        //Is this node resizeable
        public readonly bool resizeable;

        //The color of the node in the editor
        public readonly Color nodeColor;

        #endregion

        #region Attribute Methods

        /// <summary>
        /// Registers a Node as a placable Node in the editor
        /// </summary>
        /// <param name="name">The name of the node in the system</param>
        /// <param name="description">The description of the node in the system</param>
        /// <param name="width">The min width of the node</param>
        /// <param name="height">The min height of the node</param>
        /// <param name="red">The red amount of the color of the node in the editor</param>
        /// <param name="green">The green amount of the color of the node in the editor</param>
        /// <param name="blue">The blue amount of the color of the node in the editor</param>
        public NodeDataAttribute(string name, string description, float width, float height, bool resizeable = false, float red = 1f, float green = 1f, float blue = 1f)
        {
            nodeName = name;
            nodeDescription = description;
            nodeSize = new Vector2(width, height);
            this.resizeable = resizeable;
            nodeColor = new Color(red, green, blue);
        }

        #endregion
    }
}