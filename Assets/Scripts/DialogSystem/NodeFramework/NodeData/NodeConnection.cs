using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpyOnHuman.DialogSystem.NodeFramework
{
    public class NodeConnection : ScriptableObject
    {

        //--------------------------------\\
        //-----------< EDITOR >-----------\\
        //--------------------------------\\

        #if UNITY_EDITOR

        #region Connection Editor Data

        //The position of the from Handle (in the Node marked as OutputConnection)
        public List<Vector2> fromPositions;

        //The position of the to Handle (in the Node marked as InputConnection)
        public Vector2 toPosition;

        public enum ConnectionPart { From = 0, To = 1 };

        #endregion

        #region Connection Editor Methods

        /// <summary>
        /// Initiates the Connection with the from and to Nodes and the according positions
        /// </summary>
        /// <param name="from">The from Node (in the Node marked as OutputConnection)</param>
        /// <param name="fromPos">The position of the from Handle</param>
        /// <param name="to">The to Node (in the Node marked as InputConnection)</param>
        /// <param name="toPos">The position of the to Handle</param>
        public void InitiateConnection(Node from, Vector2 fromPos, Node to, Vector2 toPos)
        {
            this.froms = new List<Node>();
            this.froms.Add(from);
            this.fromPositions = new List<Vector2>();
            this.fromPositions.Add(fromPos);
            this.to = to;
            this.toPosition = toPos;
        }

        /// <summary>
        /// Discards one Node from the from Nodes list
        /// </summary>
        /// <param name="node">The Node to delete from the from list</param>
        public bool DiscardFromConnection(Node node)
        {
            if (froms.Count <= 1)
            {
                DiscardConnection();
                return true;
            }
            for (int c = 0; c < froms.Count; c++)
            {
                if (froms[c] == node)
                {
                    DiscardConnectionFromNode(node, ConnectionType.Output);
                    froms.RemoveAt(c);
                    fromPositions.RemoveAt(c);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Discards all References to this Connection in order to destroy it.
        /// </summary>
        public void DiscardConnection()
        {
            foreach (Node node in froms)
            {
                DiscardConnectionFromNode(node, ConnectionType.Output);
            }
            DiscardConnectionFromNode(to, ConnectionType.Input);
            DestroyImmediate(this);
        }

        /// <summary>
        /// Discards a specific Reference to this Connection
        /// </summary>
        /// <param name="node">The Node which should lose it's reference to this connection</param>
        /// <param name="type">The type of handle which should be cleared</param>
        private void DiscardConnectionFromNode(Node node, ConnectionType type)
        {
            List<NodeHandlePackage> packs = GetConnections(node, type);
            foreach (NodeHandlePackage pack in packs)
            {
                if (pack.info.GetValue(node) as NodeConnection == this)
                {
                    pack.info.SetValue(node, null);
                }
            }
        }

        /// <summary>
        /// Adds a Node into the from List
        /// </summary>
        /// <param name="from">The node which should be added</param>
        /// <param name="fromPos">The position of the handle of the added Node</param>
        public void AddFrom(Node from, Vector2 fromPos)
        {
            if (!Contains(from, ConnectionPart.From))
            {
                froms.Add(from);
                fromPositions.Add(fromPos);
            }
        }

        /// <summary>
        /// Checks if the given Node is stored in the Connection
        /// </summary>
        /// <param name="node">The Node which should be contained</param>
        /// <param name="direction">The part which should store the Node</param>
        /// <returns>Returns True if the Node is stored and False otherwise</returns>
        public bool Contains(Node node, ConnectionPart direction)
        {
            switch (direction)
            {
                case ConnectionPart.From:
                    {
                        foreach (Node cNode in froms)
                        {
                            if (cNode == node)
                            {
                                return true;
                            }
                        }
                    }
                    break;
                case ConnectionPart.To:
                    {
                        if (to == node)
                        {
                            return true;
                        }
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// This method should be called before the Connection is saved to prevent irregular list sizes
        /// </summary>
        public void Improve()
        {
            for (int n = 0; n < froms.Count; n++)
            {
                if (froms[n] == null)
                {
                    froms.RemoveAt(n);
                    fromPositions.RemoveAt(n);
                }
            }

            if (froms.Count < 1)
            {
                DiscardConnection();
            }
        }

        #endregion

        #region Connection Editor Static Methods

        /// <summary>
        /// Creates, Initiates and return a NodeConnection
        /// </summary>
        /// <param name="from">The from Node (in the Node marked as OutputConnection)</param>
        /// <param name="fromPos">The position of the from Handle</param>
        /// <param name="to">The to Node (in the Node marked as InputConnection)</param>
        /// <param name="toPos">The position of the to Handle</param>
        /// <returns>The created Connection</returns>
        public static NodeConnection CreateConnection(Node from, Vector2 fromPos, Node to, Vector2 toPos)
        {
            NodeConnection connection = ScriptableObject.CreateInstance<NodeConnection>();
            connection.InitiateConnection(from, fromPos, to, toPos);
            return connection;
        }
        
        /// <summary>
        /// Collects all NodeHandles in the given node
        /// </summary>
        /// <param name="node">The Node which should expose it's handles</param>
        /// <param name="type">The Type of handle searched</param>
        /// <returns>A List of all collected NodeHandlePackes</returns>
        public static List<NodeHandlePackage> GetConnections(Node node, ConnectionType type)
        {
            List<NodeHandlePackage> fields = new List<NodeHandlePackage>();

            System.Reflection.FieldInfo[] connectionFields = node.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            for (int i = 0; i < connectionFields.Length; i++)
            {
                NodeHandleAttribute attribute = System.Attribute.GetCustomAttribute(connectionFields[i], typeof(NodeHandleAttribute)) as NodeHandleAttribute;

                if (attribute != null && attribute.handleType == type)
                {
                    if (connectionFields.GetType() == typeof(NodeConnection))
                    {
                        fields.Add(new NodeHandlePackage(attribute, connectionFields[i]));
                    }
                }
            }

            return fields;
        }

        #endregion

        #endif

        //--------------------------------\\
        //-----------< PLAYER >-----------\\
        //--------------------------------\\

        #region Connection Data

        //All connections which point to the to Node
        public List<Node> froms;

        //The connection which receives all Informations
        public Node to;

        #endregion

        #region Connection Methods

        #endregion

        #region Connection Static Methods

        #endregion
    }
}