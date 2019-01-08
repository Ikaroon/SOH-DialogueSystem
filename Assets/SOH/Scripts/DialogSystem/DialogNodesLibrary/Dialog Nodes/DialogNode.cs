using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Dialog", "A Node which refers to another Dialog Canvas", 144f, 64f, false, 0.1f, 0.5f, 0.9f)]
    public class DialogNode : Node
    {
        #region Handles

        [NodeHandle(0, ConnectionType.Input, y: 0f)]
        public NodeConnection input;

        [NodeHandle(0, ConnectionType.Output, y: 0f)]
        public NodeConnection output;

        #endregion

        #region Content

        public DialogCanvas dialog;

        #endregion

        #region Display Data

        private Node currentNode;

        #endregion

        #region Update Methods

        public override bool IsAuto()
        {
            return false;
        }

        public override Node PrepareNode()
        {
            if (dialog)
            {
                currentNode = dialog.startNode.PrepareNode();
            }
            if (currentNode)
            {
                currentNode = currentNode.PrepareNode();
                return this;
            }
            if (output)
            {
                return output.to.PrepareNode();
            }
            return null;
        }

        public override Node UpdateNode()
        {
            if (currentNode)
            {
                currentNode = currentNode.UpdateNode();
                return this;
            }
            if (output)
            {
                return output.to.PrepareNode();
            }
            return null;
        }

        public override Node LateUpdateNode()
        {
            if (currentNode)
            {
                currentNode = currentNode.LateUpdateNode();
                return this;
            }
            if (output)
            {
                return output.to.PrepareNode();
            }
            return null;
        }

        #endregion

        #region Display Methods

        public override void DisplayNode(Rect rect)
        {
            if (currentNode)
            {
                currentNode.DisplayNode(rect);
            }
        }

        #endregion

    }
}