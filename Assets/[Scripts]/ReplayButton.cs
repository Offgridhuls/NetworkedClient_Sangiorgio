using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ReplayButton : MonoBehaviour
{
    public Button replayButton;
    // Start is called before the first frame update

    public void SendReplay()
    {
        NetworkedClient.SendMessageToHost(ClientToServerSignifiers.sendReplay + ",");
        foreach (var button in GameSystemManager.instance.gameButtons)
        {
            button.SetText("");
        }
        GameSystemManager.SetAllEnabled(false);
        EnableButton(false);
    }

    public void EnableButton(bool enable)
    {
        replayButton.interactable = enable;
    }

}
