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

        [NodeHandle(0, ConnectionType.Output, 0f, true, "When the point ends with null then it will continue from here.")]
        public NodeConnection output;

        public string pointKey = "";
    }
}