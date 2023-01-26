using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour
{

    public GameObject podiumPrefab;
    private float podiumWidth = 10;
    private string[] loadedPlayers;
    private GameObject[] loadedPlayersInstances;
    void Start()
    {
        MultiplayerSingleton.Instance.MyLobbyDataCallback += lobbyNewPlayers;
        MultiplayerSingleton.Instance.SendLobbyMessage();
    }

    // Update is called once per frame
    void Update() { }

    void lobbyNewPlayers(LobbyData data)
    {
        if (data.timer != null)
        {
            // TODO: show timer and update onscreen timer;
        }
        // List<LobbyPlayerData> players = new List<LobbyPlayerData>(data.players);
        // List<string> loadedPlayerList = new List<string>(loadedPlayers);
        // foreach (LobbyPlayerData player in players)
        // {
        //     if (loadedPlayerList.Contains(player.name))
        //     {
        //         players.Remove(player);
        //     }
        // }
        // for (int i = 0; i < players.ToArray().Length; i++)
        // {
        //     Instantiate(podiumPrefab, new Vector3(i * podiumWidth, 0, -12), podiumPrefab.transform.rotation);
        // }
    }
}
