using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Note", "A simple node for notes.", 128f, 64f, true, 0.4f, 0.4f, 0.4f)]
    public class NoteNode : Node
    {

        public string note;

    }
}