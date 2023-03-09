using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{

    public GameObject podiumPrefab;
    public Button readyButton;
    public GameObject timerObj;
    private Quaternion podiumPrefabRotation;
    private float podiumWidth = 10;
    private List<string> loadedPlayers = new List<string>();
    private List<GameObject> loadedPlayersInstances = new List<GameObject>();
    private List<InstantiateFlag> instantiateFlags = new List<InstantiateFlag>();
    private List<UpdateInstancesFlag> updateInstantiateFlags = new List<UpdateInstancesFlag>();

    private bool timerStarted = false;
    private float timer = 0.0f;
    private bool isReady = false;

    void Start()
    {
        MultiplayerSingleton.Instance.MyLobbyDataCallback += lobbyNewPlayers;
        MultiplayerSingleton.Instance.SendLobbyMessage();
        podiumPrefabRotation = podiumPrefab.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        // * Instances with flags
        foreach (var item in instantiateFlags)
        {
            try
            {
                GameObject obj = Instantiate(podiumPrefab, item.position, podiumPrefabRotation);
                obj.GetComponent<podiumScript>().initiatePrefab(item.id);
                loadedPlayersInstances.Add(obj);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error instantiating podium prefab: " + e.Message);
            }
        }
        instantiateFlags = new List<InstantiateFlag>();

        foreach (var item in updateInstantiateFlags)
        {
            foreach (var player in loadedPlayersInstances)
            {
                podiumScript pdmscr = player.GetComponent<podiumScript>();
                pdmscr.updateNameAndReady(item.players[pdmscr.id].name, item.players[pdmscr.id].lobbydata);
            }
        }
        updateInstantiateFlags = new List<UpdateInstancesFlag>();

        // * Timer
        if (timerStarted)
        {
            if (!timerObj.activeSelf)
            {
                timerObj.SetActive(true);
                timer += Time.deltaTime;
                if (timer > 5)
                {
                    MultiplayerSingleton.Instance.SendLobbyTimerTimeout();
                }
                Debug.Log(timerObj.GetComponent<TMPro.TextMeshProUGUI>());
                timerObj.GetComponent<TMPro.TextMeshProUGUI>().SetText(Mathf.Round(5 - timer).ToString());
            }
        }
        else
        {
            if (timerObj.activeSelf)
            {
                timerObj.SetActive(false);
            }
        }
    }

    void lobbyNewPlayers(LobbyData data)
    {
        if (data.timer != null)
        {
            timerStarted = true;
            timer = (float)data.timer;
        }
        else
        {
            timerStarted = false;
        }
        List<string> playersId = new List<string>(data.players.Keys);
        List<LobbyPlayerData> players = new List<LobbyPlayerData>(data.players.Values);
        List<string> NotYetSpawnedPlayers = new List<string>();
        foreach (string player in playersId)
        {
            if (!loadedPlayers.Contains(player))
            {
                NotYetSpawnedPlayers.Add(player);
            }
        }

        updateInstantiateFlags.Add(new UpdateInstancesFlag(data.players));

        foreach (var player in NotYetSpawnedPlayers)
        {
            instantiateFlags.Add(new InstantiateFlag(player, data.players[player].name, new Vector3(loadedPlayers.ToArray().Length * podiumWidth, 0, -12)));
            loadedPlayers.Add(player);
        }
    }

    public void onReadyButton()
    {
        isReady = !isReady;
        MultiplayerSingleton.Instance.lobbyReady = isReady;
        MultiplayerSingleton.Instance.SendLobbyMessage();
    }
}

public class InstantiateFlag
{
    public string id;
    public string name;
    public Vector3 position;

    public InstantiateFlag(string idString, string nameString, Vector3 positionVector)
    {
        id = idString;
        name = nameString;
        position = positionVector;
    }
}

public class UpdateInstancesFlag
{
    public Dictionary<string, LobbyPlayerData> players;
    public UpdateInstancesFlag(Dictionary<string, LobbyPlayerData> playerData)
    {
        players = playerData;
    }
}
