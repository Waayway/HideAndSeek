using System;
using UnityEngine;
using WebSocketSharp;
using System.Collections.Generic;
using Newtonsoft.Json;

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
    private const String DEFAULT_URL = "localhost:8888";

    // # Callbacks for various functions in the game.
    public delegate void LobbyDataCallback(LobbyData data);
    public LobbyDataCallback MyLobbyDataCallback;

    public delegate void LobbyToGameDataCallback(LobbyToGameData data);
    public LobbyToGameDataCallback MyLobbyToGameDataCallback;
    public delegate void VelocityDataCallback(Dictionary<string, VelocityData> data);
    public VelocityDataCallback MyVelocityDataCallback;

    public delegate void PlayerDeathCallback(string playerId);
    public PlayerDeathCallback playerDeathCallback;


    // # Other useful data
    private bool _firstMessage = true;
    public string id;

    public string username { get; set; } = "";

    public int prefered_map { get; set; } = 0;


    // # LobbyData
    public bool lobbyReady { get; set; }

    // # Player Data
    public Vector3 pos { get; set; }
    public Vector3 rot { get; set; }
    public Vector3 vel { get; set; }

    public float anim_left_to_right { get; set; }
    public bool anim_walking { get; set; }
    public bool anim_running { get; set; }

    // # Other game data
    public bool gameLoop = false;

    private LobbyToGameData lobbyToGameData;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("Multiplayer Singleton online");
    }

    private void OnApplicationQuit()
    {
        if (webSocket.IsAlive)
        {
            webSocket.Close();
        }
    }

    public string StartConnection(String url = "")
    {
        String urlToUse = DEFAULT_URL;
        if (!url.IsNullOrEmpty())
        {
            urlToUse = url;
        }
        if (!urlToUse.Contains(':'))
        {
            urlToUse += ":8888";
        }
        webSocket = new WebSocket("ws://" + urlToUse + "/ws");
        webSocket.OnMessage += OnMessage;
        webSocket.Connect();
        if (webSocket.IsAlive)
        {
            return "Ok";
        }
        return null;
    }

    private void OnMessage(object sender, MessageEventArgs e)
    {
        string data = e.Data;
        if (_firstMessage)
        {
            _firstMessage = false;
            id = data;
            SendFirstMessage();
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
        LobbyData newData = JsonConvert.DeserializeObject<LobbyData>(data);
        MyLobbyDataCallback.Invoke(newData);
    }
    private void receiveLobbyToGameData(string data)
    {
        LobbyToGameData newData = JsonUtility.FromJson<LobbyToGameData>(data);
        lobbyToGameData = newData;
        //Debug.Log(newData);
        MyLobbyToGameDataCallback.Invoke(newData);
    }
    private void receiveLobbyLoadingUpdates(string data)
    {
        // TODO: Implement Function probably not gonna make this
    }
    private void receiveVelocityData(string data)
    {
        Dictionary<string, VelocityData> newData = JsonConvert.DeserializeObject<Dictionary<string, VelocityData>>(data);
        MyVelocityDataCallback.Invoke(newData);
    }
    private void receiveDeathData(string data)
    {
        playerDeathCallback.Invoke(data);
    }
    private void receiveGameOverData(string data)
    {
        // TODO: Implement Function
    }
    // # All functions that deal in sending data about this client

    public void SendFirstMessage()
    {
        FirstMessage data = new FirstMessage();
        data.name = username;
        data.prefered_map = prefered_map;
        string json = JsonUtility.ToJson(data);
        webSocket.Send(json);
    }
    public void SendLobbyMessage()
    {
        string data = "0{\"" + id + "\" : " + (lobbyReady ? "true" : "false") + "}";
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
                left_right = anim_left_to_right,
                walking = anim_walking,
                running = anim_running
            }
        };
        webSocket.Send("4" + JsonUtility.ToJson(data));
    }
    public void SendGameTimerTimeout()
    {
        webSocket.Send("1gametimer");
    }

    public void SendPlayerFound(string idFound)
    {
        webSocket.Send("5{\"playerFound\": \"" + idFound + "\"}");
    }
    public LobbyToGameData GetLobbyToGameData() {
        return lobbyToGameData;
    }

    // # Multiplayer send loop
    public IEnumerator<WaitForSeconds> SendVelData()
    {
        WaitForSeconds waitTime = new WaitForSeconds(1/30);
        while (gameLoop)
        {
            SendVelocityData();
            yield return waitTime;
        }
    }
}

[Serializable]
public class LobbyData
{
    public Dictionary<string, LobbyPlayerData> players;
    public int map;
    public float? timer;
}

[Serializable]
public class LobbyPlayerData
{
    public bool lobbydata;
    public string name;
}
[Serializable]
public class LobbyToGameData
{
    public int playersdoneloading;
    public String[] players;
    public String[] playerNames;
    public float timer;
    public int totalTime;
    public string chosenplayer;
}


[Serializable]
public class FirstMessage
{
    public string name;
    public int prefered_map;
}
[Serializable]
public class VelocityData
{
    public Vector3 pos;
    public Vector3 rot;
    public Vector3 vel;
    public AnimationData anim;
}
[Serializable]
public class AnimationData
{
    public float left_right;
    public bool walking;
    public bool running;
}