using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Register Hub", "Registers this hub to the stack of hubs to return to it when a dialog ends in nothing.", 128f, 64f)]
    public class RegisterHubNode : Node
    {
        [NodeHandle(0, ConnectionType.Input, 0f)]
        public NodeConnection input;

        [NodeHandle(0, ConnectionType.Output, 0f)]
        public NodeConnection output;

    }
}