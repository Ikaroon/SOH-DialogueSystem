using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    [System.Serializable]
    public class NodeHandleID
    {
        public int ID;
        public Vector2 position;

        public NodeHandleID(int ID, Vector2 position)
        {
            this.ID = ID;
            this.position = position;
        }
    }
}