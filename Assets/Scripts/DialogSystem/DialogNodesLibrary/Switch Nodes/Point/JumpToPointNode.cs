using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Jump To Point", "Jumps to a defined point in the canvas.", 128f, 64f)]
    public class JumpToPointNode : Node
    {
        [NodeHandle(0, ConnectionType.Input, 0f)]
        public NodeConnection input;

        public string pointKey = "";

        public override bool IsAuto()
        {
            return true;
        }

        public override Node PrepareNode()
        {
            if (DialogInterpreter.current.interpretedDialog.definedPoints.ContainsKey(pointKey))
            {
                return DialogInterpreter.current.interpretedDialog.definedPoints[pointKey].PrepareNode();
            }
            return null;
        }

        public override Node UpdateNode()
        {
            return null;
        }

        public override Node LateUpdateNode()
        {
            return null;
        }
    }
}