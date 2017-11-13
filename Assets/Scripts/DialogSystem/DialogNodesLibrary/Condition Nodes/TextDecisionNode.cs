using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;
using SpyOnHuman.DialogSystem.LanguageSystem;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Player Decision", "A Dialog Node", 256f, 272f, true, 1f, 0.8f, 0.1f)]
    public class TextDecisionNode : Node
    {
        [NodeHandle(0, ConnectionType.Input, 0f)]
        public NodeConnection input;

        [NodeHandle(0, ConnectionType.Output, 1f/6f, fixedPosition: false)]
        public NodeConnection decisionA;

        [NodeHandle(1, ConnectionType.Output, 3f / 6f, fixedPosition: false)]
        public NodeConnection decisionB;

        [NodeHandle(2, ConnectionType.Output, 5f / 6f, fixedPosition: false)]
        public NodeConnection decisionC;

        public LangText decisionAText = new LangText();
        public LangAudio decisionAAudio = new LangAudio();

        public LangText decisionBText = new LangText();
        public LangAudio decisionBAudio = new LangAudio();

        public LangText decisionCText = new LangText();
        public LangAudio decisionCAudio = new LangAudio();

        private Node output;
        private int decision = -1;

        public override bool IsAuto()
        {
            if (output == null || output == this)
            {
                return true;
            } else
            {
                return false;
            }
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

        public override void DisplayNode(Rect rect)
        {
            float sizeMod = (rect.width / 1920);

            GUIStyle label = new GUIStyle(GUI.skin.label);
            label.alignment = TextAnchor.MiddleCenter;
            label.fontSize = (int)(30 * sizeMod);
            label.richText = true;

            Rect textRect = new Rect(0f, rect.height - 128f * sizeMod, rect.width, 128f * sizeMod);
            string displayText = "";
            string whiteText = "";

            if (output == this)
            {
                string agressiveColor = "#" + ColorUtility.ToHtmlStringRGBA(new Color(1f, 0.2f, 0.1f, 1f));
                string friendlyColor = "#" + ColorUtility.ToHtmlStringRGBA(new Color(0.1f, 0.6f, 1f, 1f));
                string strategyColor = "#" + ColorUtility.ToHtmlStringRGBA(new Color(0.9f, 0.7f, 0.1f, 1f));

                displayText = "<color=" + friendlyColor + ">X: Freundlich</color>  <color=" + 
                    strategyColor + ">Y: Strategisch</color>  <color=" + agressiveColor + ">B: Aggressiv</color>";
                whiteText = "X: Freundlich  Y: Strategisch  B: Aggressiv";

                if (Input.GetAxis("Aggressive") > 0f)
                {
                    if (decisionA)
                    {
                        output = decisionA.to;
                    } else
                    {
                        output = null;
                    }
                    decision = 0;
                }
                else if (Input.GetAxis("Friendly") > 0f)
                {
                    if (decisionB)
                    {
                        output = decisionB.to;
                    }
                    else
                    {
                        output = null;
                    }
                    decision = 1;
                }
                else if (Input.GetAxis("Strategic") > 0f)
                {
                    if (decisionC)
                    {
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
                            displayText = "<b>Du: </b>" + decisionAText.text;
                            whiteText = displayText;
                        } break;
                    case 1:
                        {
                            displayText = "<b>Du: </b>" + decisionBText.text;
                            whiteText = displayText;
                        } break;
                    case 2:
                        {
                            displayText = "<b>Du: </b>" + decisionCText.text;
                            whiteText = displayText;
                        } break;
                }
            }

            DrawOutline(textRect, whiteText, label, Color.black);
            GUI.Label(textRect, new GUIContent(displayText), label);
        }

        private void DrawOutline(Rect rect, string text, GUIStyle style, Color color, float thickness = 1f)
        {
            Rect offsetRect = rect;
            string outlineColor = "#" + ColorUtility.ToHtmlStringRGBA(color);
            string finalText = "<color=" + outlineColor + ">" + text + "</color>";

            for (int x = -1; x <= 1; x += 2)
            {
                for (int y = -1; y <= 1; y += 2)
                {
                    offsetRect.position = rect.position + new Vector2(x, y) * thickness;

                    GUI.Label(offsetRect, new GUIContent(finalText), style);
                }
            }
        }
    }
}