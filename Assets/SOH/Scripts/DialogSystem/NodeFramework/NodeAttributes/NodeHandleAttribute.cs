using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Northwind.Essentials;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class NodeHandleAttribute : Attribute
    {

        #region Attribute Data

        //The unique ID for this handle.
        public readonly int ID;

        //The type of the connection
        public readonly ConnectionType handleType;

        //The local handle position in the system
        public readonly float height;

        //The value if y should be used as fixed value
        public readonly bool isFixed;

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
        public NodeHandleAttribute(int ID, ConnectionType type, float y = -1f, bool fixedPosition = true, string tooltip = "")
        {
            this.ID = ID;
            handleType = type;
            height = y;
            handleTooltip = tooltip;
            isFixed = fixedPosition;
        }

        public Vector2 HandlePosition(Vector2 size)
        {
            return VectorMath.Step(new Vector2(0f, 32f + height * (isFixed ? 1f : (size.y - 16f - 32f))), 16f);
        }

        #endregion

    }
}