using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;

using Northwind.Essentials;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Jump To Dialog", "A Node which refers to another Dialog Canvas", 144f, 64f, false, 0.1f, 0.5f, 0.9f)]
    public class JumpToDialogNode : Node
    {

        #region Handles

        [NodeHandle(0, ConnectionType.Input, y: 0f)]
        public NodeConnection input;

        #endregion

        #region Content

        public DialogCanvas dialog;

        #endregion

        #region Update Methods

        public override bool IsAuto()
        {
            return true;
        }

        public override Node PrepareNode()
        {
            if (dialog != null)
            {
                return dialog.startNode.PrepareNode();
            }
            return null;
        }

        #endregion

    }
}
