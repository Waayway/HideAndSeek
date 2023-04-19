using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public Image blackoutScreen;
    public TMPro.TextMeshProUGUI introText;
    public GameObject MultiplayerPlayerPrefab;
    public Color surfaceColor = Color.black;
    public Color hiderJointColor = Color.cyan;
    public Color seekerJointColor = Color.magenta;
    private int totalTime;
    private string seeker;
    private bool isSeeker;
    private string[] players;
    private string[] playerNames;
    private LobbyToGameData lobbyToGameData;
    private Dictionary<string, GameObject> loadedPlayersInstances;
    public GameObject thisPlayer;

    private float fadeOutDuration = 4f;
    void Start()
    {
        MultiplayerSingleton.Instance.gameLoop = true;
        loadedPlayersInstances = new Dictionary<string, GameObject>();

        MultiplayerSingleton.Instance.MyVelocityDataCallback += updateVelocityData;

        lobbyToGameData = MultiplayerSingleton.Instance.GetLobbyToGameData();
        seeker = lobbyToGameData.seeker;
        isSeeker = seeker == MultiplayerSingleton.Instance.id;
        if (!isSeeker)
        {
            Color transparent = Color.black;
            transparent.a = 0;
            blackoutScreen.GetComponent<Image>().color = transparent;
            introText.SetText("You are a hider.");
        }
        StartCoroutine(FadeOutTimer());


        foreach (var player in lobbyToGameData.players)
        {
            if (player == MultiplayerSingleton.Instance.id)
            {
                thisPlayer.GetComponent<PlayerController>().updateColorsAndSeeker(seeker == player, surfaceColor, seekerJointColor, hiderJointColor);
                continue;
            }
            if (loadedPlayersInstances.ContainsKey(player)) {
                continue;
            }

            GameObject obj = Instantiate(MultiplayerPlayerPrefab, Vector3.up * 10, MultiplayerPlayerPrefab.transform.rotation);
            obj.GetComponent<MultiplayerPlayerController>().InitiatePrefab(player, seeker == player, surfaceColor, seekerJointColor, hiderJointColor);
            loadedPlayersInstances.Add(player, obj);

        }

        StartCoroutine(MultiplayerSingleton.Instance.SendVelData());
    }
    IEnumerator FadeOutTimer()
    {
        if (isSeeker)
        {
            yield return new WaitForSeconds(8f);
        }
        else
        {
            yield return new WaitForSeconds(5f);
        }
        StartCoroutine(FadeOut());
    }
    IEnumerator FadeOut()
    {
        float counter = 0;
        while (counter < fadeOutDuration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, counter / fadeOutDuration);
            if (isSeeker)
            {
                Color blackoutColor = blackoutScreen.GetComponent<Image>().color;
                blackoutColor.a = alpha;
                blackoutScreen.GetComponent<Image>().color = blackoutColor;
            }
            Color curColor = introText.color;
            curColor.a = alpha;
            introText.color = curColor;
            yield return null;
        }

    }

    void Update()
    {

    }

    void updateVelocityData(Dictionary<string, VelocityData> data)
    {
        Debug.Log(data);
        foreach (var (player, velData) in data)
        {
            loadedPlayersInstances[player].GetComponent<MultiplayerPlayerController>().updateVelocity(velData);
        }
    }
}
