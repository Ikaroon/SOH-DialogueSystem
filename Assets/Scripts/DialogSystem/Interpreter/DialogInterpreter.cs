using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;
using SpyOnHuman.DialogSystem.CharacterSystem;

namespace SpyOnHuman.DialogSystem
{
    public class DialogInterpreter : MonoBehaviour
    {

        public static DialogInterpreter current;

        public DialogCanvas interpretedDialog;
        private Node currentNode;
        private bool started = false;

        public bool paused = true;

        public string currentText = "";
        public bool next = false;

        private bool buttonUp = true;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                paused = false;
                started = false;
            }
            if (!paused)
            {
                current = this;
                if (currentNode == null)
                {
                    if (!started)
                    {
                        started = true;
                        currentNode = interpretedDialog.startNode.PrepareNode();
                    } else
                    {
                        currentText = "Dialog beendet!";
                    }
                } else
                {
                    if (currentNode.IsAuto())
                    {
                        UpdateCurrentNode();
                    }
                    else if (next || (Input.GetAxis("OK") > 0f && buttonUp))
                    {
                        buttonUp = false;
                        UpdateCurrentNode();
                        next = false;
                    }
                }
            }

            if ((Input.GetAxis("OK") <= 0f && !buttonUp))
            {
                buttonUp = true;
            }
        }

        private void OnGUI()
        {
            if (currentNode != null)
            {
                currentNode.DisplayNode(new Rect(0f, 0f, Screen.width, Screen.height));
            }
        }

        private void UpdateCurrentNode()
        {
            currentNode = currentNode.UpdateNode();
            if (currentNode)
            {
                currentNode = currentNode.LateUpdateNode();
            }
        }
    }
}