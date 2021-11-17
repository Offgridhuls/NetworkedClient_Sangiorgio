using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemManager : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject submitButton, userNameInput, passwordInput, createToggle, loginToggle;

    private GameObject networkedClient;

    public GameObject gamePanel, accountPanel;

    public static bool isO;

    public static GameSystemManager instance;

    public Text statusLabel;

    public GameButton[] gameButtons;
    void Start()
    {
        instance = this;
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (go.name == "UsernameInputField")
                userNameInput = go;
            else if (go.name == "PasswordInputField")
                passwordInput = go;
            else if (go.name == "SubmitButton")
                submitButton = go;
            else if (go.name == "CreateToggle")
                createToggle = go;
            else if (go.name == "LoginToggle")
                loginToggle = go;
            else if (go.name == "NetworkedClient")
                networkedClient = go;

        }

        submitButton.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);

        loginToggle.GetComponent<Toggle>().onValueChanged.AddListener(LoginToggleChanged);
        createToggle.GetComponent<Toggle>().onValueChanged.AddListener(CreateToggleChanged);

        loginToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(false);

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SubmitButtonPressed()
    {
        string n = userNameInput.GetComponent<InputField>().text;
        string p = passwordInput.GetComponent<InputField>().text;
        string msg;

        if (createToggle.GetComponent<Toggle>().isOn)
            msg = ClientToServerSignifiers.createAccount + "," + n + "," + p;
        else
            msg = ClientToServerSignifiers.login + "," + n + "," + p;

        NetworkedClient.SendMessageToHost(msg);

        //networkedClient.GetComponent<NetworkedClient>().SendMessage(msg);
    }


    public void LoginToggleChanged(bool newVal)
    {
        createToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!newVal);
    }

    public void CreateToggleChanged(bool newVal)
    {
        loginToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!newVal);
    }
    
    public static void SetTextOnButton(int index, string text)
    {
        instance.gameButtons[index].SetText(text);
    }

    public static void SetEnabled(int index, bool enable)
    {
        instance.gameButtons[index].EnableButton(enable);
    }
    
    public static void SetAllEnabled(bool enabled)
    {
        for(int i = 0; i < instance.gameButtons.Length; i ++)
        {
            SetEnabled(i, enabled);
        }
    }

    public static void SetStatusLabel(string text)
    {
        instance.statusLabel.text = text;
    }

    public static void SetPanelActive(bool enable)
    {
        instance.gamePanel.SetActive(enable);
        instance.accountPanel.SetActive(!enable);
    }

    public static void SetAllOtherButtonsEnabled()
    {
        for(int i = 0; i < instance.gameButtons.Length; i++)
        {
            if(instance.gameButtons[i].buttonText.text != "")
            {
                SetEnabled(i, false);
            }
            else
            {
                SetEnabled(i, true);
            }
        }
    }

}
