using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SpyOnHuman.NodeFramework
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class NodeHandleAttribute : Attribute
    {
        #if UNITY_EDITOR

        #region Attribute Data
        
        //The type of the connection
        public readonly ConnectionType handleType;

        //The local handle position in the system
        public readonly Vector2 handlePosition;

        //The tooltip in the editor
        public readonly string handleTooltip;

        #endregion

        #region Attribute Methods

        /// <summary>
        /// Registers a NodeConnection as a handle in the editor
        /// </summary>
        /// <param name="type">The type of the connection</param>
        /// <param name="x">The local handle x position in the system</param>
        /// <param name="y">The local handle y position in the system</param>
        /// <param name="tooltip">The tooltip in the editor</param>
        public NodeHandleAttribute(ConnectionType type, float x, float y, string tooltip = "")
        {
            handleType = type;
            handlePosition = new Vector2(x, y);
            handleTooltip = tooltip;
        }

        #endregion

        #endif
    }
}