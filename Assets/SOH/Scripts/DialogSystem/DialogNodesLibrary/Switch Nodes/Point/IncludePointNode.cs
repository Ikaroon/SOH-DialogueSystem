using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Include Point", "Jumps to a defined point in the canvas and will continue.", 128f, 64f)]
    public class IncludePointNode : Node
    {
        #region Handles

        [NodeHandle(0, ConnectionType.Input, 0f)]
        public NodeConnection input;

        [NodeHandle(0, ConnectionType.Output, 0f, true, "When the point ends with null then it will continue from here.")]
        public NodeConnection output;

        #endregion

        #region Content

        public string pointKey = "";

        #endregion

        #region Play Data

        private bool used = false;

        #endregion

        #region Update Methods

        public override bool IsAuto()
        {
            return false;
        }

        public override Node PrepareNode()
        {
            if (!used && DialogPlayer.player.canvas.definedPoints.ContainsKey(pointKey))
            {
                DialogPlayer.player.RegisterHub(this);
                used = true;
                return DialogPlayer.player.canvas.definedPoints[pointKey].PrepareNode();
            }
            if (used && output)
            {
                return output.to.PrepareNode();
            }
            return null;
        }

        #endregion
    }
}