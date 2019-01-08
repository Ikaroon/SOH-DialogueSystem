using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;
using SpyOnHuman.DialogSystem.LanguageSystem;
using SpyOnHuman.CharacterSystem;

using Northwind.Essentials;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Decision", "A Dialog Decision Node which waits for the player input to choose the path", 256f, 352f, true, 1f, 1f, 0.1f)]
    public class TextDecisionNode : Node
    {
        #region Handles

        [NodeHandle(0, ConnectionType.Input, 0f)]
        public NodeConnection input;

        [NodeHandle(0, ConnectionType.Output, 1f/6f, fixedPosition: false)]
        public NodeConnection decisionA;

        [NodeHandle(1, ConnectionType.Output, 3f / 6f, fixedPosition: false)]
        public NodeConnection decisionB;

        [NodeHandle(2, ConnectionType.Output, 5f / 6f, fixedPosition: false)]
        public NodeConnection decisionC;

        #endregion

        #region Content

        public Character speaker;

        public LangText decisionAText = new LangText();
        public LangAudio decisionAAudio = new LangAudio();

        public LangText decisionBText = new LangText();
        public LangAudio decisionBAudio = new LangAudio();

        public LangText decisionCText = new LangText();
        public LangAudio decisionCAudio = new LangAudio();

        #endregion

        #region Display Data

        private Node output;
        private int decision = -1;
        private string outcome = "";

        private string[] words;
        private Vector2Int wordRange = new Vector2Int(0, 1);
        private float directionATime, directionBTime = 0f;
        private float remainingTime = 0f;

        #endregion

        #region Update Methods

        public override bool IsAuto()
        {
            return (output == null || output == this);
        }

        public override Node PrepareNode()
        {
            decision = -1;
            output = this;
            return this;
        }

        public override Node UpdateNode()
        {
            if (output)
            {
                return output.PrepareNode();
            }
            return null;
        }

        #endregion

        #region Display Methods

        private static GUIStyle labelcolored;

        void InitStyle(float sizeMod)
        {
            if (labelcolored != null)
            {
                return;
            }

            labelcolored = new GUIStyle(GUI.skin.GetStyle("Label"));
            labelcolored.alignment = TextAnchor.MiddleCenter;
            labelcolored.fontStyle = FontStyle.Bold;
            labelcolored.normal.textColor = Color.black;
            labelcolored.fontSize = Mathf.RoundToInt(13f * sizeMod);
        }

        public override void DisplayNode(Rect rect)
        {

            float sizeMod = (rect.width / 1920);
            float sizeModH = (rect.height / 1080);

            InitStyle(sizeMod);

            if (output == this)
            {
                GUIShapes.Disc(new Vector2(rect.width / 2f - 100f * sizeModH, rect.height - 100f * sizeModH), 90f * sizeModH, Color.white);
                GUI.Label(new Rect(rect.width / 2f - 130f * sizeModH, rect.height - 130f * sizeModH, 60f * sizeModH, 60f * sizeModH), new GUIContent("Friendly\n[A]"), labelcolored);
                GUIShapes.Disc(new Vector2(rect.width / 2f - 60f * sizeModH, rect.height - 70f * sizeModH), 40f * sizeModH, Color.white);

                GUIShapes.Disc(new Vector2(rect.width / 2f, rect.height - 140f * sizeModH), 90f * sizeModH, Color.white);
                GUI.Label(new Rect(rect.width / 2f - 30f * sizeModH, rect.height - 170f * sizeModH, 60f * sizeModH, 60f * sizeModH), new GUIContent("Strategic\n[S]"), labelcolored);
                GUIShapes.Disc(new Vector2(rect.width / 2f, rect.height - 100f * sizeModH), 40f * sizeModH, Color.white);

                GUIShapes.Disc(new Vector2(rect.width / 2f + 100f * sizeModH, rect.height - 100f * sizeModH), 90f * sizeModH, Color.white);
                GUI.Label(new Rect(rect.width / 2f + 70f * sizeModH, rect.height - 130f * sizeModH, 60f * sizeModH, 60f * sizeModH), new GUIContent("Aggressive\n[D]"), labelcolored);
                GUIShapes.Disc(new Vector2(rect.width / 2f + 60f * sizeModH, rect.height - 70f * sizeModH), 40f * sizeModH, Color.white);

                if (Input.GetKeyDown(KeyCode.A))
                {
                    if (decisionA)
                    {
                        if (decisionAAudio.audio)
                        {
                            AudioSource.PlayClipAtPoint(decisionAAudio.audio, Vector3.zero);
                        }
                        output = decisionA.to;
                    } else
                    {
                        output = null;
                    }
                    decision = 0;
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    if (decisionB)
                    {
                        if (decisionBAudio.audio)
                        {
                            AudioSource.PlayClipAtPoint(decisionBAudio.audio, Vector3.zero);
                        }
                        output = decisionB.to;
                    }
                    else
                    {
                        output = null;
                    }
                    decision = 1;
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    if (decisionC)
                    {
                        if (decisionBAudio.audio)
                        {
                            AudioSource.PlayClipAtPoint(decisionBAudio.audio, Vector3.zero);
                        }
                        output = decisionC.to;
                    }
                    else
                    {
                        output = null;
                    }
                    decision = 2;
                }
            } else
            {
                switch (decision)
                {
                    case 0:
                        {
                            DisplayText(rect, decisionAText.text);
                        } break;
                    case 1:
                        {
                            DisplayText(rect, decisionBText.text);
                        } break;
                    case 2:
                        {
                            DisplayText(rect, decisionCText.text);
                        } break;
                }
            }

        }

        void DisplayText(Rect rect, string text)
        {
            float sizeMod = (rect.width / 1920);

            GUIStyle label = new GUIStyle(GUI.skin.label);
            label.alignment = TextAnchor.MiddleCenter;
            label.fontSize = (int)(30 * sizeMod);
            label.richText = true;

            GUIStyle smallLabel = new GUIStyle(GUI.skin.label);
            smallLabel.alignment = TextAnchor.MiddleCenter;
            smallLabel.fontSize = (int)(30 * sizeMod);
            smallLabel.richText = true;

            Rect textRect = new Rect(0f, rect.height - 128f * sizeMod, rect.width, 128f * sizeMod);
            string displayText = "";
            string whiteText = "";

            if (speaker != null)
            {
                string speakerColor = "#" + ColorUtility.ToHtmlStringRGBA(speaker.textColor);
                displayText = "<b><color=" + speakerColor + ">" + speaker.forename + ": </color></b>" + text;
                whiteText = "<b>" + speaker.forename + ": </b>" + text;
            }
            else
            {
                displayText = text;
                whiteText = text;
            }
            
            Northwind.Essentials.GUIEffects.DrawOutline(textRect, whiteText, label, Color.black, 1f);

            GUI.Label(textRect, new GUIContent(displayText), label);

        }

        #endregion

    }
}