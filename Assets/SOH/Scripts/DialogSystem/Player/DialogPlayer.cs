using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SpyOnHuman.DialogSystem.NodeFramework;

using SpyOnHuman;

namespace SpyOnHuman.DialogSystem
{
    [System.Serializable]
    public class DialogPlayer
    {
        #region Dialog Data

        [SerializeField]
        public DialogCanvas canvas;

        #endregion

        #region Player Data

        public static DialogPlayer player;

        public bool pauseGame;

        public Vector3 dialogPosition;

        public enum PlayerState { Playing, Paused, Stopped, Skip };
        [SerializeField]
        public PlayerState playerState = PlayerState.Stopped;

        private Node currentNode;

        #endregion

        #region Hub/Point Data

        public Stack<Node> registeredHubs = new Stack<Node>();

        #endregion

        #region Hub/Point Methods

        public void RegisterHub(Node hubNode)
        {
            registeredHubs.Push(hubNode);
        }

        public Node GetCurrentHub()
        {
            if (registeredHubs.Count > 0)
            {
                return registeredHubs.Peek();
            }
            return null;
        }

        public void DiscardHub()
        {
            if (registeredHubs.Count > 0)
            {
                registeredHubs.Pop();
            }
        }

        #endregion

        #region Play Methods

        public void UpdateDialog()
        {
            if (canvas == null)
            {
                return;
            }

            switch (playerState)
            {
                case PlayerState.Paused: return;
                case PlayerState.Stopped:
                    {
                        ResetToStartNode();
                    }
                    break;
                case PlayerState.Playing:
                    {
                        PlayNode(false);
                    }
                    break;
                case PlayerState.Skip:
                    {
                        PlayNode(false, true);
                        if (playerState != PlayerState.Stopped)
                        {
                            playerState = PlayerState.Playing;
                        }
                    }
                    break;
            }
        }

        public void ManualUpdate()
        {
            if (canvas == null)
            {
                return;
            }

            switch (playerState)
            {
                case PlayerState.Paused: return;
                case PlayerState.Stopped:
                    {
                        ResetToStartNode();
                    }
                    break;
                case PlayerState.Playing:
                    {
                        PlayNode(true);
                    }
                    break;
            }
        }

        private void PlayNode(bool manual, bool force = false)
        {
            if (canvas != null)
            {
                player = this;
                
                if (currentNode == null)
                {
                    CheckHubs();
                }
                else
                {
                    if (force || (currentNode.IsAuto() && !manual) || (!currentNode.IsAuto() && manual))
                    {
                        currentNode = currentNode.UpdateNode();
                    }
                }
            }
        }

        public void CheckHubs()
        {
            if (registeredHubs.Count > 0)
            {
                currentNode = GetCurrentHub().PrepareNode();
            }
            else if (currentNode == null)
            {
                playerState = PlayerState.Stopped;
            }
        }

        public void DisplayNode(Rect rect)
        {
            if (currentNode != null)
            {
                currentNode.DisplayNode(rect);
            }
        }

        public void ResetToStartNode()
        {
            player = null;
            currentNode = canvas.startNode;
            registeredHubs.Clear();
        }

        #endregion
    }
}