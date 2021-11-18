using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedClient : MonoBehaviour
{

    static int connectionID;
    static int maxConnections = 1000;
    static int reliableChannelID;
    static int unreliableChannelID;
    static int hostID;
    static int socketPort = 5491;
    static byte error;
    bool isConnected = false;
    int ourClientID;

    private LinkedList<PlayerAccount> playerAccounts;
    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.S))
        //    SendMessageToHost("Hello from client");

        UpdateNetworkConnection();
    }

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    //Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
    }
    
    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            connectionID = NetworkTransport.Connect(hostID, "127.0.0.1", socketPort, 0, out error); // server is local on networka

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID);

            }
        }
    }
    
    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }
    
    public static void SendMessageToHost(string msg)
    {

        byte[] buffer = Encoding.Unicode.GetBytes(msg);

        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);

    }

    private void ProcessRecievedMsg(string msg, int id)
    {

        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split(',');

        int signifier = int.Parse(csv[0]);


        if (signifier == ClientToServerSignifiers.createAccount)
        {

        }
        if (signifier == ServerToClientSignifiers.waiting)
        {
            GameSystemManager.SetPanelActive(true);

            if (int.TryParse(csv[1], out int result))
            {
                if (result == 0)
                {
                    GameSystemManager.SetStatusLabel("Spectating");
                }
                else
                {
                    GameSystemManager.SetStatusLabel("Waiting");
                }
            }
        }
        else if (signifier == ServerToClientSignifiers.playTurn)
        {
            if (int.TryParse(csv[1], out int result))
            {
                if (result == 0)
                {
                    if(csv.Length == 2)
                    {
                        GameSystemManager.SetStatusLabel("Your Turn");
                        GameSystemManager.isO = false;
                        GameSystemManager.SetAllOtherButtonsEnabled();
                    }
                    else if(csv.Length == 3)
                    {
                        if(int.TryParse(csv[2], out int index))
                        {
                            GameSystemManager.instance.gameButtons[index].SetText("X");
                            GameSystemManager.isO = true;
                            GameSystemManager.SetAllOtherButtonsEnabled();
                        }

                    }
                    else if(csv.Length == 4)
                    {
                        if (int.TryParse(csv[2], out int index))
                        {
                            GameSystemManager.instance.gameButtons[index].SetText("X");
                        }
                    }
                }
                else if(result == 1)
                {
                    if (csv.Length == 2)
                    {
                        GameSystemManager.SetStatusLabel("Your Turn");
                        GameSystemManager.isO = true;
                        GameSystemManager.SetAllOtherButtonsEnabled();
                    }
                    else if (csv.Length == 3)
                    {
                        if (int.TryParse(csv[2], out int index))
                        {
                            GameSystemManager.instance.gameButtons[index].SetText("O");
                            GameSystemManager.isO = false;
                            GameSystemManager.SetAllOtherButtonsEnabled();
                        }
                    }
                    else if (csv.Length == 4)
                    {
                        if (int.TryParse(csv[2], out int index))
                        {
                            GameSystemManager.instance.gameButtons[index].SetText("O");
                        }
                    }
                }
                else if(result == -1)
                {
                    GameSystemManager.SetStatusLabel("Waiting for other player");
                    GameSystemManager.SetPanelActive(true);
                }
                
            }
        }
        else if (signifier == ServerToClientSignifiers.winner)
        {
            if (int.TryParse(csv[1], out int result))
            {
                if (result == 0)
                {
                    GameSystemManager.SetStatusLabel("The winner is " + csv[2] + "!");
                    GameSystemManager.SetAllEnabled(false);
                }
                else if (result == 1)
                {
                    GameSystemManager.SetStatusLabel("You Win!");
                    GameSystemManager.SetAllEnabled(false);
                }
            }
        }
    }

    public bool IsConnected()
    {
        return isConnected;
    }


    public static void SendPlay(int index)
    {
        string message = ClientToServerSignifiers.sendPlay + "," + index;
        SendMessageToHost(message);
    }

}

public class PlayerAccount
{
    public string name, password;

    public PlayerAccount(string Name, string Password)
    {

        this.name = Name;

        this.password = Password;

    }
}

public static class ClientToServerSignifiers
{

    public const int createAccount = 1;

    public const int login = 2;

    public const int sendPlay = 3;

}

public static class ServerToClientSignifiers
{

    public const int loginComplete = 1;

    public const int loginFailed = 2;

    public const int accountCreationComplete = 3;

    public const int accountCreationFailed = 4;

    public const int isSpectator = 5;

    public const int waiting = 6;

    public const int playTurn = 7;

    public const int winner = 8;
}