using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;
using SpyOnHuman.DialogSystem.LanguageSystem;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Dialog Fragment", "A Dialog Node", 256f, 128f, true, 0.1f, 0.5f, 0.9f)]
    public class DialogFragmentNode : Node
    {
        [NodeHandle(0, ConnectionType.Input, y: 0f)]
        public NodeConnection input;

        [NodeHandle(0, ConnectionType.Output, y: 0f)]
        public NodeConnection output;

        public LangText text = new LangText();

    }
}