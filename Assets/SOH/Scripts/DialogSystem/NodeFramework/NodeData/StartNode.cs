using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    [NodeData("Start", "The Start Node", 128f, 64f, false, 0.5f, 0.9f, 0.1f)]
    public class StartNode : Node
    {
        [NodeHandle(0, ConnectionType.Output, y: 0f)]
        public NodeConnection output;

        public override bool IsAuto()
        {
            return true;
        }

        public override Node PrepareNode()
        {
            return output.to.PrepareNode();
        }

        public override Node UpdateNode()
        {
            return output.to.PrepareNode();
        }
    }
}