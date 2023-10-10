using TMPro;
using Unity.Netcode;

public class GameInfoText : NetworkUI
{
    private TMP_Text info;
  
    void Start()
    {
        info = GetComponent<TMP_Text>();
    }
    
    void Update()
    {
        if (!IsSpawned)
        {
            return;
        }

        var data = NetworkGameController.Singelton.gameData;
        var state = NetworkGameController.Singelton.gameState;

        info.text = "Status: ";
        info.text += state == GameState.WaitForPlayers ? "Wait for player2 join the game" :
            (state == GameState.Active ? "Game active" : "wait for player2 reconnect");

        if (NetworkManager.Singleton.IsServer)
        {
            info.text += "\n";
            info.text +=  data.player1.isInit ?  $"Player1 connected:{data.player1.isConnected}": "player1 wait to join";
            info.text += "\n";
            info.text += data.player2.isInit ?  $"Player2 connected:{data.player2.isConnected}": "player2 wait to join";
        }
    }

    public override void OnGameFinished()
    {
        base.OnGameFinished();
        gameObject.SetActive(false);
    }
}
