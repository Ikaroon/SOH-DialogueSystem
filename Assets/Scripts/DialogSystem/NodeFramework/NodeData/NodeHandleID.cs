using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    [System.Serializable]
    public class NodeHandleID
    {
        public int ID;
        public Vector2 handlePosition;

        public NodeHandleID(int ID, Vector2 handlePosition)
        {
            this.ID = ID;
            this.handlePosition = handlePosition;
        }
    }
}