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

    void Start()
    {
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

        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);

        networkedClient.GetComponent<NetworkedClient>().SendMessage(msg);
    }

    public void LoginToggleChanged(bool newVal)
    {
        loginToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!newVal);
    }

    public void CreateToggleChanged(bool newVal)
    {
        createToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!newVal);
    }
}
