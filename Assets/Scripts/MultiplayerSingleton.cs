using System;
using UnityEngine;
using WebSocketSharp;
using System.Collections.Generic;

public class MultiplayerSingleton : MonoBehaviour
{
    private static MultiplayerSingleton _instance;
    public static MultiplayerSingleton Instance
    {
        get
        {
            // * Create logic and Instance
            if (_instance == null)
            {
                GameObject go = new GameObject("MultiplayerSingleton");
                go.AddComponent<MultiplayerSingleton>();
            }

            return _instance;
        }
    }
    // # Websocket itself
    private WebSocket webSocket;


    // # Default URL for the websocket with this it makes it so it will just connect to a local server if available even if the user passes in no correct ip.
    private const String DEFAULT_URL = "ws://localhost:8888/ws";

    // # Callbacks for various functions in the game.
    public delegate void LobbyDataCallback(LobbyData data);
    public LobbyDataCallback MyLobbyDataCallback;

    public delegate void LobbyToGameDataCallback(LobbyToGameData data);
    public LobbyToGameDataCallback MyLobbyToGameDataCallback;


    // # Other useful data
    private bool _firstMessage = true;
    public string id;

    public string username { get; set; }

    public int prefered_map { get; set; }


    // # LobbyData
    public bool lobbyReady { get; set; }

    // # Player Data
    public Vector3 pos { get; set; }
    public Vector3 rot { get; set; }
    public Vector3 vel { get; set; }
    public int anim { get; set; }
    public bool anim_reversed { get; set; }

    // # Other game data
    private bool gameLoop = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }
        _instance = this;
        Debug.Log("Multiplayer Singleton online");
    }

    public void StartConnection(String url = "")
    {
        String urlToUse = DEFAULT_URL;
        if (!url.IsNullOrEmpty())
        {
            urlToUse = url;
        }
        using (var ws = new WebSocket(urlToUse))
        {
            webSocket = ws;
            webSocket.OnMessage += OnMessage;
            webSocket.Connect();
        }
    }

    private void OnMessage(object sender, MessageEventArgs e)
    {
        string data = e.Data;
        if (_firstMessage)
        {
            _firstMessage = false;
            id = data;
        }
        else if (data.StartsWith("0"))
        {
            receiveLobbyData(data.Substring(1));
        }
        else if (data.StartsWith("2"))
        {
            receiveLobbyToGameData(data.Substring(1));
        }
        else if (data.StartsWith("3"))
        {
            receiveLobbyLoadingUpdates(data.Substring(1));
        }
        else if (data.StartsWith("4"))
        {
            receiveVelocityData(data.Substring(1));
        }
        else if (data.StartsWith("5"))
        {
            receiveGameOverData(data.Substring(1));
        }
        else if (data.StartsWith("6"))
        {
            receiveDeathData(data.Substring(1));
        }
    }

    // # Receive data functions,
    // * Receive all data from server and into different catagories.
    private void receiveLobbyData(string data)
    {
        LobbyData newData = JsonUtility.FromJson<LobbyData>(data);
        MyLobbyDataCallback(newData);
    }
    private void receiveLobbyToGameData(string data)
    {
        LobbyToGameData newData = JsonUtility.FromJson<LobbyToGameData>(data);
        MyLobbyToGameDataCallback(newData);
    }
    private void receiveLobbyLoadingUpdates(string data)
    {
        // TODO: Implement Function
    }
    private void receiveVelocityData(string data)
    {
        // TODO: Implement Function
    }
    private void receiveDeathData(string data)
    {
        // TODO: Implement Function
    }
    private void receiveGameOverData(string data)
    {
        // TODO: Implement Function
    }
    // # All functions that deal in sending data about this client

    public void SendFirstMessage()
    {
        FirstMessage data = new FirstMessage
        {
            name = username,
            prefered_map = prefered_map,
        };


        webSocket.Send(JsonUtility.ToJson(data));
    }
    public void SendLobbyMessage()
    {
        string data = "0{\"" + id + "\" : \"" + (lobbyReady ? "true" : "false") + "\"}";
        webSocket.Send(data);
    }
    public void SendLobbyLoaded()
    {
        webSocket.Send("2true");
    }
    public void SendLobbyTimerTimeout()
    {
        webSocket.Send("1timer");
    }

    public void SendVelocityData()
    {
        VelocityData data = new VelocityData
        {
            pos = pos,
            rot = rot,
            vel = vel,
            anim = new AnimationData
            {
                num = anim,
                reversed = anim_reversed
            }
        };
        webSocket.Send(JsonUtility.ToJson(data));
    }
    public void SendGameTimerTimeout()
    {
        webSocket.Send("1gametimer");
    }

    public void SendPlayerFound(int idFound)
    {
        webSocket.Send("5{\"playerfound\": \"" + idFound + "\"}");
    }

    // # Multiplayer send loop
    public IEnumerator<WaitForSeconds> SendVelData()
    {
        WaitForSeconds waitTime = new WaitForSeconds(1 / 30);
        while (gameLoop)
        {
            SendVelocityData();
            yield return waitTime;
        }
    }
}

public class LobbyData
{
    public LobbyPlayerData[] players { get; set; }
    public float timer { get; set; }
}
public class LobbyToGameData
{
    public int playersdoneloading { get; set; }
    public String[] players { get; set; }
    public String[] playerNames { get; set; }
    public float timer { get; set; }
    public int totalTime { get; set; }
    public string seeker { get; set; }
}

public class LobbyPlayerData
{
    public bool lobbydata { get; set; }
    public string name { get; set; }
}

public class FirstMessage
{
    public String name { get; set; }
    public int prefered_map { get; set; }
}

public class VelocityData
{
    public Vector3 pos { get; set; }
    public Vector3 rot { get; set; }
    public Vector3 vel { get; set; }
    public AnimationData anim { get; set; }
}
public class AnimationData
{
    public int num { get; set; }
    public bool reversed { get; set; }
}