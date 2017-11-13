using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Include Point", "Jumps to a defined point in the canvas and will continue.", 128f, 64f)]
    public class IncludePointNode : Node
    {
        [NodeHandle(0, ConnectionType.Input, 0f)]
        public NodeConnection input;

        [NodeHandle(0, ConnectionType.Output, 0f, true, "When the point ends with null then it will continue from here.")]
        public NodeConnection output;

        public string pointKey = "";
        private Node currentNode;

        public override bool IsAuto()
        {
            return false;
        }

        public override Node PrepareNode()
        {
            if (DialogInterpreter.current.interpretedDialog.definedPoints.ContainsKey(pointKey))
            {
                currentNode = DialogInterpreter.current.interpretedDialog.definedPoints[pointKey];
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

        public override void DisplayNode(Rect rect)
        {
            if (currentNode)
            {
                currentNode.DisplayNode(rect);
            }
        }
    }
}