using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LobbyUI : NetworkBehaviour
{
    protected LobbyController lobbyController => LobbyController.Singelton;
    public GameObject collectionPointPanel;
    public Canvas canvas;
    public TMP_InputField codeEnterField;   
    public TMP_Text infoText;
    private int playersCount;
    public GameObject NetworkManagerPrefab;

    public QrDisplay qrCodeDisplay;

    private void Awake()
    {               
        var nm = FindObjectOfType<NetworkManager>();
       
        if (nm != null)
        {
            try
            {
                nm.Shutdown();

            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        else
        {
            Instantiate(NetworkManagerPrefab);
        }

        SwitchLobbyUIInteractable(false);
        infoText.text = "Lobby starting";

    }
    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        Invoke(nameof(InitAsync), 0.1f);
   //     InitAsync();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
        StopAllCoroutines();
    }

    public void OnClientConnected(ulong id)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            ++playersCount;
            if (playersCount == 2)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("OnlineGame", LoadSceneMode.Single);
            }
        }
    }

    public void OnClientDisconnected(ulong id)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            --playersCount;
        }
    }

    private async void InitAsync()
    {
        SwitchLobbyUIInteractable(false);
        await lobbyController.Initialize();

        infoText.text = "ERS for Telegram";
        Debug.Log("Use buttons to create a game");


        SwitchLobbyUIInteractable(true);
    }


    public async void CreateGame(bool publicGame = true)
    {
        SwitchLobbyUIInteractable(false);
        if (publicGame)
        {
            infoText.text = "Creating public game";
            var lobby = await lobbyController.CreateOrJoinLobby();
            if (lobby != null)
            {
                infoText.text = "Game created";
                StartCollectionPoint();
            }
            else
            {
                infoText.text = "Unable create game";
                SwitchLobbyUIInteractable(true);
            }
        }
        else
        {
            infoText.text = "Creating private game with join code.";
            var lobby = await lobbyController.CreateLobby(false);
            if (lobby == null)
            {
                infoText.text = "Unable create game";
                SwitchLobbyUIInteractable(true);
                return;
            }
           
            StartCollectionPoint(false);
        }
    }


    public async void JoinGame()
    {
        string code = codeEnterField.text.Trim().ToUpper();
        if (string.IsNullOrEmpty(code))
        {
            infoText.text = "Please enter join code";
            return;
        }
        SwitchLobbyUIInteractable(false);
        var lobby = await lobbyController.JoinLobby(code);
        if (lobby == null)
        {
            infoText.text = "Join code is wrong";
            SwitchLobbyUIInteractable(true);
            return;
        }
        StartCollectionPoint();
    }

    public async void CancelGame()
    {
        try
        {
            playersCount = 0;
            codeEnterField.text = "";
            StopAllCoroutines();
            collectionPointPanel.SetActive(false);
            await lobbyController.DisconnectLobby();
            await lobbyController.DeleteLobbyAsync();
            NetworkManager.Singleton?.Shutdown();
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            SwitchLobbyUIInteractable(true);
        }
            
    }

    private void StartCollectionPoint(bool publicGame = true)
    {
        StartCoroutine(HeartbeatLobbyCoroutine());
        infoText.text = "Use buttons to create a lobby";
        collectionPointPanel?.SetActive(true);
        var _text = collectionPointPanel.GetComponentInChildren<TMP_Text>();
        if (publicGame)
        {
            _text.text = "Wait for players";
               qrCodeDisplay.HideQr();
        }
        else
        {
            _text.text = $"Wait for players2 enter join code\nJoin code: {lobbyController.JoinCode}";
            qrCodeDisplay.ShowQr();
        }       
    }

    private void SwitchLobbyUIInteractable(bool interactable)
    {
        var interactableUI = gameObject.GetComponentInChildren<Transform>().GetComponentsInChildren<Button>();
        foreach (var item in interactableUI)
        {
            item.interactable = interactable;
        }
        codeEnterField.interactable = interactable;
    }

    IEnumerator HeartbeatLobbyCoroutine(float waitTimeSeconds = 15)
    {
        while (true)
        {
            LobbyController.Singelton.HeartbeatLobby();          
            yield return new WaitForSecondsRealtime(waitTimeSeconds);
        }
    }

    private void OnApplicationQuit()
    {
       LobbyController.Singelton.DeleteLobbyAsync();
    }
}
