using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;
using SpyOnHuman.DialogSystem.LanguageSystem;
using SpyOnHuman.CharacterSystem;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Random", "A Node which picks randomly one of the set outputs", 64f, 160f, false, 1f, 1f, 0.1f)]
    public class RandomNode : Node
    {

        #region Handles

        [NodeHandle(0, ConnectionType.Input, y: 0f)]
        public NodeConnection input;

        [NodeHandle(0, ConnectionType.Output, y: 0f)]
        public NodeConnection outputA;

        [NodeHandle(1, ConnectionType.Output, y: 32f)]
        public NodeConnection outputB;

        [NodeHandle(2, ConnectionType.Output, y: 64f)]
        public NodeConnection outputC;

        [NodeHandle(3, ConnectionType.Output, y: 96f)]
        public NodeConnection outputD;

        #endregion

        #region Update Methods

        public override bool IsAuto()
        {
            return true;
        }

        public override Node PrepareNode()
        {
            List<NodeConnection> connections = new List<NodeConnection>();
            AddEnd(connections, outputA);
            AddEnd(connections, outputB);
            AddEnd(connections, outputC);
            AddEnd(connections, outputD);

            if (connections.Count <= 0)
            {
                return null;
            }

            int random = Random.Range(0, connections.Count);
            return connections[random].to.PrepareNode();
        }

        private void AddEnd(List<NodeConnection> connections, NodeConnection connection)
        {
            if (connection != null && connection.to != null)
            {
                connections.Add(connection);
            }
        }

        #endregion
    }
}