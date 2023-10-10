using System;
using Unity.Netcode;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class NetworkGameController : NetworkBehaviour
{

    private static NetworkGameController _singelton;
    public UnityEvent OnNetworkSpawned;
    public UnityEvent OnGameAbandoned;
    public UnityEvent<GameData> OnDataGameUpdated;
    public UnityEvent<GameData> OnGameFinished;
    public UnityEvent<NetworkString> OnCombinationDetected;



    [SerializeField]
    private Card cardPrefab;

    [SerializeField]
    private Transform discardPilePoint;

    [SerializeField]
    private Transform player1CardSpawnPoint;

    [SerializeField]
    private Transform player2CardSpawnPoint;

    [SerializeField]
    private CardAnimator cardAnimator;


    NetworkPlayer _player1;
    NetworkPlayer _player2;
    DiscardPile discardPile = new();
    NetworkPlayer currentPlayer;

    private bool IsGameStarted;
    private bool _initialized;

    NetworkVariable<GameState> _state = new();
    NetworkVariable<GameData> _gameData = new();

    public static NetworkGameController Singelton => _singelton;
    public int PlayersCount => NetworkManager.Singleton.ConnectedClients.Count;

    public GameData gameData => _gameData.Value;
    public GameState gameState => _state.Value;


 
    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerInfoServerRpc( WebAppUser webAppUser, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("#ID: " +  serverRpcParams.Receive.SenderClientId + " | " + _player1.sessionId + " | " +  _player2.sessionId);
        if(_player1 != null &&   _player1.sessionId == serverRpcParams.Receive.SenderClientId){
            _player1.name = $"{webAppUser.first_name} {webAppUser.last_name}";
            _player1.avatar = webAppUser.photo_url ?? "";
        }
        else if(_player2 != null && _player2.sessionId == serverRpcParams.Receive.SenderClientId){
            _player2.name = $"{webAppUser.first_name} {webAppUser.last_name}";
            _player2.avatar = webAppUser.photo_url ?? "";
        }

         UpdateData();

    }

    public void InvokePlayCard()
    {
        PlayCardServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayCardServerRpc(ServerRpcParams serverRpcParams = default)
    {
        try
        {
            if (currentPlayer?.sessionId != serverRpcParams.Receive.SenderClientId)
            {
                Debug.Log($"It is not your move");
                return;
            }

            if (!IsServer || _state.Value != GameState.Active)
            {
                return;
            }
            var cardValue = currentPlayer.GetCard();
            var cardSpawnPoint = _player1?.sessionId == serverRpcParams.Receive.SenderClientId ?
                player1CardSpawnPoint : player2CardSpawnPoint;

            var card = Instantiate(cardPrefab, cardSpawnPoint.position, Quaternion.identity);

            // card.transform.SetParent(discardPilePoint);
            card.GetComponent<NetworkObject>()?.Spawn();
            card.transform.SetParent(discardPilePoint);

            var networkCard = card.gameObject.GetComponent<NetworkCard>();
            networkCard.Setup($"{cardValue.Rank.GetDescription()}_of_{cardValue.Suit.GetDescription()}", discardPile.CardCount);
            discardPile.AddCard(cardValue);
            cardAnimator.AddAnimation(card, discardPilePoint.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

            TogglePlayer();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void Slap()
    {
        SlapServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void SlapServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (discardPile.CardCount == 0)
        {
            return;
        }

        var slapCombination = discardPile.GetSlapCombination();


        if (serverRpcParams.Receive.SenderClientId == _player1.sessionId)
        {

            if (slapCombination != SlapCombination.None)
                TakeCardsFromDiscardPile(_player1);
            else
                TakeCardsFromDiscardPile(_player2);
        }
        else
        {
            if (slapCombination != SlapCombination.None)
                TakeCardsFromDiscardPile(_player2);
            else
                TakeCardsFromDiscardPile(_player1);
        }
    }

    private void TakeCardsFromDiscardPile(NetworkPlayer player)
    {
        var cardsIds = new ulong[discardPilePoint.childCount];
        var cardSpawnPoint = NetworkManager.Singleton.LocalClientId == player.sessionId ? player1CardSpawnPoint : player2CardSpawnPoint;
        var slapCombination = discardPile.GetSlapCombination();

        for (int i = discardPilePoint.childCount - 1; i >= 0; --i)
        {
            var card = discardPilePoint.GetChild(i).GetComponent<Card>();
            cardsIds[i] = (card.GetComponent<NetworkCard>().NetworkObjectId);
            card.transform.parent = null;
            cardAnimator.AddAnimation(card, cardSpawnPoint.position, () =>
            {
                card?.GetComponent<NetworkObject>()?.Despawn();
            });

            player.AddCard(discardPile.GetTopCard());

        }

        currentPlayer = player;

        UpdateData();

        TakeCardsFromDiscardPileClientRpc(slapCombination.GetDescription(), gameData);
    }

    [ClientRpc]
    private void TakeCardsFromDiscardPileClientRpc(NetworkString combinationDescription, GameData data)
    {
        if (data.player1.cardsCount == 0)
        {
            OnGameFinished?.Invoke(data);
        }
        else if (data.player2.cardsCount == 0)
        {
            OnGameFinished?.Invoke(data);
        }
        else
        {
            OnCombinationDetected?.Invoke(combinationDescription);
           
        }
    }

    [ClientRpc]
    private void GameAbandonedClientRpc()
    {
        if (IsClient)
        {
            OnGameAbandoned?.Invoke();
        }
    }

    private void TogglePlayer()
    {
        if (_player1.CardCount == 0)
            currentPlayer = _player2;
        else if (_player2.CardCount == 0)
            currentPlayer = _player1;
        else if (currentPlayer == _player1)
            currentPlayer = _player2;
        else if (currentPlayer == _player2)
            currentPlayer = _player1;
    }

    void Update()
    {

        if (!IsSpawned)
        {
            return;
        }

        UpdateData();
    }

    private void UpdateData()
    {
        if (_state.Value == GameState.Active)
        {
            var data = _gameData.Value;

            if (IsServer)
            {
                if (_player1 != null)
                {
                    data.player1.id = _player1.sessionId;
                    data.player1.cardsCount = _player1.CardCount;
                    data.player1.isPlayersMove = currentPlayer.ID == _player1.ID;
                    data.player1.isConnected = _player1.IsConnected;
                    data.player1.name = _player1.name;
                    data.player1.avatar = _player1.avatar;
                    data.player1.isInit = true;
                }

                if (_player2 != null)
                {
                    data.player2.id = _player2.sessionId;
                    data.player2.cardsCount = _player2.CardCount;
                    data.player2.isPlayersMove = currentPlayer.ID == _player2.ID;
                    data.player2.isConnected = _player2.IsConnected;
                    data.player2.name = _player2.name;
                    data.player2.avatar = _player2.avatar;
                    data.player2.isInit = true;
                }

                if (currentPlayer != null)
                {
                    data.currentPlayer = currentPlayer.sessionId;
                }


                _gameData.Value = data;
            }
        }
    }

    private void Awake()
    {
        if (_singelton != null)
        {
            Debug.LogError("There are more then one instances of ServerGameController");
            throw new Exception("There are more then one instances of ServerGameController");
        }
        _singelton = this;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        discardPile = new DiscardPile();

    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _singelton = null;

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;

            NetworkManager.Singleton.Shutdown();
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (_initialized)
        {
            return;
        }
        _initialized = true;
        if (IsServer)
        {
            _state.Value = GameState.WaitForPlayers;
            var connectedClients = NetworkManager.Singleton.ConnectedClientsList;
        }

        if (!IsHost && !IsServer)
        {
            Camera.main.transform.Rotate(new Vector3(Camera.main.transform.rotation.x, Camera.main.transform.rotation.y, 180));
        }

        OnNetworkSpawned?.Invoke();
    }


    private void CheckoutState()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            if (PlayersCount == 2 && (_player1?.IsConnected ?? false) && (_player2?.IsConnected ?? false))
            {
                _state.Value = GameState.Active;
                if (!IsGameStarted)
                {
                    IsGameStarted = true;
                    DeckManager.DealCards(_player1, _player2);
                }
            }
        }
    }

    public void OnClientConnected(ulong id)
    {
    }

    public void OnClientDisconnected(ulong id)
    {
        if (IsServer)
        {
            GameAbandonedClientRpc();
        }
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (!IsHost)
        {
            OnGameAbandoned?.Invoke();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void QuitGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (IsServer)
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }

            try
            {
                LobbyController.Singelton.DeleteLobbyAsync();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }
    }

    public async void QuitGame()
    {
        if (IsClient)
        {
            try
            {
                await LobbyController.Singelton.DisconnectLobby();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            QuitGameServerRpc();

            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.Shutdown();
                //  Destroy(NetworkManager.Singleton.gameObject);
            }

            SceneManager.LoadScene("Lobby");
        }
    }

    public void SimulateWin()
    {
        SimulateWinServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SimulateWinServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var player = serverRpcParams.Receive.SenderClientId == gameData.player1.id ? _player1 : _player2;
        var op_player = serverRpcParams.Receive.SenderClientId != gameData.player1.id ? _player1 : _player2;
        var cardsIds = new ulong[discardPilePoint.childCount];
        for (int i = discardPilePoint.childCount - 1; i >= 0; --i)
        {
            var card = discardPilePoint.GetChild(i).GetComponent<Card>();
            cardsIds[i] = (card.GetComponent<NetworkCard>().NetworkObjectId);
            card.transform.parent = null;
            player.AddCard(discardPile.GetTopCard());
        }


        while (op_player.CardCount > 0)
        {
            player.AddCard(op_player.GetCard());
        }

        currentPlayer = player;

        UpdateData();
        TakeCardsFromDiscardPileClientRpc(SlapCombination.CheatWin.GetDescription(), gameData);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ConnectClientServerRpc(string guidStr, ServerRpcParams serverRpcParams = default)
    {
        var id = serverRpcParams.Receive.SenderClientId;

        if (IsServer)
        {
            if (_player1?.ID == guidStr)
            {
                _player1.IsConnected = true;
                _player1.sessionId = id;
                CheckoutState();
                return;
            }
            if (_player2?.ID == guidStr)
            {
                _player2.IsConnected = true;
                _player2.sessionId = id;
                CheckoutState();
                return;
            }
            if (_player1 == null)
            {
                _player1 = new(guidStr, id);
                currentPlayer = _player1;
                CheckoutState();
                return;
            }
            if (_player2 == null)
            {
                _player2 = new(guidStr, id);
                CheckoutState();
                return;
            }

            Debug.LogError($"Error, to many clients connected id({guidStr}). Connected clients count: {PlayersCount}");
        }
    }
}
