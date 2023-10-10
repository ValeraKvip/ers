using TMPro;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayCardButton : NetworkUI
{
    private Button _button;
    private TMP_Text _text;

    void Start()
    {       
        _button = GetComponent<Button>();
        _text = _button.GetComponentInChildren<TMP_Text>();
    }

    void Update()
    {
        if (!IsSpawned)
        {
            return;
        }

        var gameData = NetworkGameController.Singelton.gameData;
        var LocalClientId = NetworkManager.Singleton.LocalClientId;
        _button.interactable = LocalClientId == gameData.currentPlayer;
        var player = LocalClientId == gameData.player1.id ? gameData.player1 : gameData.player2;
        _text.text = $"Play card ({player.cardsCount})";

        if (NetworkGameController.Singelton.gameState != GameState.Active)
        {
            _button.interactable = false;
        }
    }

    public override void OnGameFinished()
    {
        base.OnGameFinished();
        gameObject.SetActive(false);
    }
}
