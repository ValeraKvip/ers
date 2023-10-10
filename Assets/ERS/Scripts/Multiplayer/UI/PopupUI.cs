using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PopupUI : NetworkUI
{
    public GameObject popupGameFinish;
    public GameObject popupCombination;

    public void ShowPopup(GameData data)
    {       
        var LocalClientId = NetworkManager.Singleton.LocalClientId;
        var textLabel = popupGameFinish.GetComponentInChildren<TMP_Text>();

       
        if (data.player1.cardsCount == 0)
        {
            textLabel.text = data.player1.id == LocalClientId ? $"{data.player2.name} Won!" : "you Won!";          
        }
        else if (data.player2.cardsCount == 0)
        {
            textLabel.text = data.player2.id == LocalClientId ? $"{data.player1.name} Won!" : "you Won!";           
        }
        else
        {
            textLabel.text =  (data.player1.id == LocalClientId ? data.player1.name:data.player2.name) + " left the game, you Won!";
        }

        popupCombination.SetActive(false);
        popupGameFinish.SetActive(true);
    }

    public void OnCombinationDetected(NetworkString combination)
    {
        var data = NetworkGameController.Singelton.gameData;      
        popupCombination.SetActive(true);
        popupCombination.GetComponentInChildren<TMP_Text>().text = combination;
        Invoke(nameof(HidePopup), 2f);    
    }

    public void OnGameAbandoned()
    {             
        popupGameFinish.GetComponentInChildren<TMP_Text>().text = "Player 2 left the game";

        popupCombination.SetActive(false);
        popupGameFinish.SetActive(true);
    }

    private  void HidePopup()
    {        
        popupCombination.SetActive(false);
    }
}
