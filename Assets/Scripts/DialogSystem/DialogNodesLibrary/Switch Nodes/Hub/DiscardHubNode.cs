using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Discard Hub", "Discards the last hub from the playing stack of hubs.", 128f, 64f)]
    public class DiscardHubNode : Node
    {
        [NodeHandle(0, ConnectionType.Input, 0f)]
        public NodeConnection input;

        [NodeHandle(0, ConnectionType.Output, 0f, true, "Continue the dialog without the return to the last hub.")]
        public NodeConnection output;
    }
}