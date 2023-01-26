using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public TMP_InputField serverInput;
    public TMP_InputField usernameInput;

    public TMP_Text errorElement;

    private Regex IpRegex = new Regex(@"(\\b25[0-5]|\\b2[0-4][0-9]|\\b[01]?[0-9][0-9]?)(\\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}");
    private Regex HostnameRegex = new Regex(@"^(?=.{1,255}$)[0-9A-Za-z](?:(?:[0-9A-Za-z]|-){0,61}[0-9A-Za-z])?(?:\\.[0-9A-Za-z](?:(?:[0-9A-Za-z]|-){0,61}[0-9A-Za-z])?)*\\.?$");
    void Start() { }

    void Update() { }

    public void JoinGame()
    {
        string ServerIP = serverInput.text; 
        string username = usernameInput.text;
        if (!IpRegex.IsMatch(ServerIP) && !HostnameRegex.IsMatch(ServerIP) && !string.IsNullOrEmpty(ServerIP)) {
            errorElement.text = "Server is not valid input an ip or hostname";
            return;
        }
        if (string.IsNullOrEmpty(username)) {
            errorElement.text = "Username has to be filled";
            return;
        }
        MultiplayerSingleton.Instance.username = username;
        if (MultiplayerSingleton.Instance.StartConnection(ServerIP) == "Ok") {
            SceneManager.LoadScene(1); // Load lobby scene
        } else {
            errorElement.text = "Cannot connect to server";
        }
    }
}
