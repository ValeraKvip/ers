using TMPro;
using UnityEngine;

public class TopBar : NetworkUI
{
    public TMP_Text _player1Name;
    public TMP_Text _player2Name;
    public TMP_Text _player1Score;
    public TMP_Text _player2Score;

  

    void Update()
    {
        if (!IsSpawned)
        {
            return;
        }

        var gameData = NetworkGameController.Singelton.gameData;

        Debug.Log("TOP_BAR " + gameData.player1.name + " | " + gameData.player2.name);
        
       _player1Name.text =  gameData.player1.name;
       _player1Score.text =  gameData.player1.cardsCount.ToString();
       _player2Name.text =  gameData.player2.name;
       _player2Score.text =  gameData.player2.cardsCount.ToString();

      
    }

    public override void OnGameFinished()
    {
        base.OnGameFinished();
        gameObject.SetActive(false);
    }
}
