using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    public class NodeHandlePackage
    {
        #region Package Data

        //A NodeHandle to be transfered together with the FieldInfo
        public NodeHandleAttribute handle;

        //A FieldInfo to be transfered together with the NodeHandle
        public FieldInfo info;

        #endregion

        #region Package Methods

        /// <summary>
        /// Creates a NodeHandlePackage to transfer a FieldInfo with the according NodeHandleAttribute
        /// </summary>
        /// <param name="handle">A NodeHandle to be transfered together with the FieldInfo</param>
        /// <param name="info">A FieldInfo to be transfered together with the NodeHandle</param>
        public NodeHandlePackage(NodeHandleAttribute handle, FieldInfo info)
        {
            this.handle = handle;
            this.info = info;
        }

        #endregion
    }
}