using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class GameButton : MonoBehaviour
{
    public Text buttonText;
    public Button btn;

    public void SendPlay()
    {
      for (int i = 0; i < GameSystemManager.instance.gameButtons.Length; i++)
      {
      
            if (GameSystemManager.instance.gameButtons[i] == this)
            {
            
                NetworkedClient.SendPlay(i);
                GameSystemManager.instance.gameButtons[i].SetText(GameSystemManager.isO ? "O" : "X");
                GameSystemManager.SetStatusLabel("Waiting For Other Player");
                GameSystemManager.SetAllEnabled(false);
                break;
            }
      }
    }

    public void EnableButton(bool enable = true)
    {
        btn.interactable = enable;
    }

    public void SetText(string text)
    {
        buttonText.text = text;
    }
}
