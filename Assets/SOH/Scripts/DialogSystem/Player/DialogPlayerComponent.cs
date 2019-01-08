using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    public class DialogPlayerComponent : MonoBehaviour
    {
        public DialogPlayer player = new DialogPlayer();

        public bool isAutoplay = true;

        private void Start()
        {
            if (player.playerState == DialogPlayer.PlayerState.Stopped && isAutoplay)
            {
                player.ResetToStartNode();
                player.playerState = DialogPlayer.PlayerState.Playing;
            }
        }

        void Update()
        {
            player.dialogPosition = transform.position;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.ManualUpdate();
            }
            else
            {
                player.UpdateDialog();
            }
        }

        private void OnGUI()
        {
            player.DisplayNode(new Rect(0f, 0f, Screen.width, Screen.height));
        }

        public void Play()
        {
            player.ResetToStartNode();
            player.playerState = DialogPlayer.PlayerState.Playing;
            //player.UpdateDialog();
        }

        public void Stop()
        {
            player.playerState = DialogPlayer.PlayerState.Stopped;
        }

        public void SetPauseMode(bool pause)
        {
            player.pauseGame = pause;
        }

        public void SetDialog(DialogCanvas dialog)
        {
            player.canvas = dialog;
        }
    }
}