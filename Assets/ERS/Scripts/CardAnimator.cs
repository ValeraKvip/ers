using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardAnimator : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 20f;

    [SerializeField]
    private float rotationSpeed = 10f;

    [SerializeField]
    private UnityEvent onAllAnimationsFinished = new();

    private bool shouldInvoke;
    private CardAnimation currentCardAnimation;
    private readonly Queue<CardAnimation> cardAnimations = new();

    private void Update()
    {
        if (currentCardAnimation == null)
            NextAnimation();
        else if (currentCardAnimation.Play(movementSpeed, rotationSpeed))
            NextAnimation();
    }

    private void NextAnimation()
    {
        currentCardAnimation = null;
        if (cardAnimations.Count > 0)
        {
            currentCardAnimation = cardAnimations.Dequeue();
        }
        else
        {
            if (shouldInvoke)
            {
                shouldInvoke = false;
                onAllAnimationsFinished.Invoke();
            }
        }
    }

    

        public void AddAnimation(Card card, Vector3 position, UnityAction OnAnimationFinishedDelegate)
    {
        AddAnimation(card, position, Quaternion.identity, OnAnimationFinishedDelegate);
    }

    public void AddAnimation(Card card, Vector3 position)
    {
        AddAnimation(card, position, Quaternion.identity);
    }

    public void AddAnimation(Card card, Vector3 position, Quaternion rotation)
    {
        AddAnimation(card, position, rotation, null);
    }

    public void AddAnimation(Card card, Vector3 position, Quaternion rotation, UnityAction OnAnimationFinishedDelegate)
    {
        shouldInvoke = true;
        cardAnimations.Enqueue(new CardAnimation(card, position, rotation, OnAnimationFinishedDelegate));
    }
}
