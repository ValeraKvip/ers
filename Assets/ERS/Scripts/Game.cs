using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Card cardPrefab;

    [SerializeField]
    private Transform discardPilePoint;

    [SerializeField]
    private Transform localCardSpawnPoint;

    [SerializeField]
    private Transform remoteCardSpawnPoint;

    [SerializeField]
    private Button player1PlayCardButton;

    [SerializeField]
    private Button player2PlayCardButton;

    [SerializeField]
    private GameObject popup;

    [SerializeField]
    private CardAnimator cardAnimator;

    private Player currentPlayer;
    private readonly Player localPlayer = new();
    private readonly Player remotePlayer = new();
    private readonly DiscardPile discardPile = new();

    private void Awake()
    {
        DeckManager.DealCards(localPlayer, remotePlayer);
        currentPlayer = localPlayer;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !DeckManager.IsPointerOverUI())
            if (discardPile.CardCount > 0)
                Slap();
    }


    public void PlayCard()
    {
        var cardValue = currentPlayer.GetCard();
        var cardSpawnPoint = currentPlayer == localPlayer ? localCardSpawnPoint : remoteCardSpawnPoint;

        var card = Instantiate(cardPrefab, cardSpawnPoint.position, Quaternion.identity, discardPilePoint);
        card.GetComponent<NetworkObject>()?.Spawn();
        card.gameObject.name = $"{cardValue.Rank.GetDescription()}_of_{cardValue.Suit.GetDescription()}";
        card.SetDisplayingOrder(discardPile.CardCount);

        discardPile.AddCard(cardValue);
        cardAnimator.AddAnimation(card, discardPilePoint.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

        TogglePlayer();
        UpdateButtons();
    }

    private void Slap()
    {
        var slapCombination = discardPile.GetSlapCombination();

        if (Input.mousePosition.y < Screen.height * 0.5f)
        {
            if (slapCombination != SlapCombination.None)
            {
                TakeCardsFromDiscardPile(localPlayer);
                TelegramConnect.HapticFeedback("success");
            }
            else
            {


                TakeCardsFromDiscardPile(remotePlayer);
                TelegramConnect.HapticFeedback("error");
            }
        }
        else
        {
            if (slapCombination != SlapCombination.None)
            {
                TakeCardsFromDiscardPile(remotePlayer);
                TelegramConnect.HapticFeedback("error");
            }
            else
            {
                TakeCardsFromDiscardPile(localPlayer);
                TelegramConnect.HapticFeedback("success");
            }
        }

        if (localPlayer.CardCount == 0)
        {
            ShowPopup("Player 2 Won!");
            Invoke(nameof(Restart), 2f);
        }
        else if (remotePlayer.CardCount == 0)
        {
            ShowPopup("Player 1 Won!");
            Invoke(nameof(Restart), 2f);
        }
        else
        {
            ShowPopup(slapCombination.GetDescription());
        }
    }

    private void TakeCardsFromDiscardPile(Player player)
    {
        var cardStack = player == localPlayer ? localCardSpawnPoint : remoteCardSpawnPoint;

        for (int i = discardPilePoint.childCount - 1; i >= 0; --i)
        {
            var card = discardPilePoint.GetChild(i).GetComponent<Card>();
            card.transform.parent = cardStack;
            player.AddCard(discardPile.GetTopCard());
            cardAnimator.AddAnimation(card, cardStack.position);
        }

        currentPlayer = player;
        UpdateButtons();
    }

    private void TogglePlayer()
    {
        if (localPlayer.CardCount == 0)
            currentPlayer = remotePlayer;
        else if (remotePlayer.CardCount == 0)
            currentPlayer = localPlayer;
        else if (currentPlayer == localPlayer)
            currentPlayer = remotePlayer;
        else if (currentPlayer == remotePlayer)
            currentPlayer = localPlayer;
    }

    private void UpdateButtons()
    {
        player1PlayCardButton.interactable = localPlayer.CardCount != 0 && currentPlayer == localPlayer;
        player1PlayCardButton.GetComponentInChildren<TMP_Text>().text = $"Play card ({localPlayer.CardCount})";

        if (player2PlayCardButton != null)
        {
            player2PlayCardButton.GetComponentInChildren<TMP_Text>().text = $"Play card ({remotePlayer.CardCount})";
            player2PlayCardButton.interactable = remotePlayer.CardCount != 0 && currentPlayer == remotePlayer;
        }
    }

    private void ShowPopup(string text)
    {
        popup.SetActive(true);
        foreach (var t in popup.GetComponentsInChildren<TMP_Text>())
            t.text = text;
        Invoke(nameof(HidePopup), 2f);
    }

    private void HidePopup()
    {
        popup.SetActive(false);
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void OnAllAnimationsFinished()
    {
        foreach (Transform card in localCardSpawnPoint)
            Destroy(card.gameObject);
        foreach (Transform card in remoteCardSpawnPoint)
            Destroy(card.gameObject);
    }
}
