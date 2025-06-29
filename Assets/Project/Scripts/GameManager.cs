using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Card firstCard, secondCard;
    private bool canSelect = false;

    private int turnsUsed = 0;
    private int score = 0;
    private int pairsRemaining = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void InitializeGame(int totalPairs)
    {
        pairsRemaining = totalPairs;
        turnsUsed = 0;
        score = 0;
        canSelect = true;
    }

    public void CardSelected(Card selectedCard)
    {
       UnityEngine.Debug.Log($"Card clicked | canSelect: {canSelect}, isFlipped: {selectedCard.IsFlipped},{selectedCard.cardId}");
        if (!canSelect || selectedCard.IsFlipped) return;

        selectedCard.Flip(true);

        if (firstCard == null)
        {
            firstCard = selectedCard;
        }
        else
        {
            secondCard = selectedCard;
            canSelect = false;
            GridManager.Instance.canInteract = false;
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        turnsUsed++;
        yield return new WaitForSeconds(0.5f);

        if (firstCard.cardId == secondCard.cardId)
        {
            firstCard.Match();
            secondCard.Match();
            score += 50;
            pairsRemaining--;

            if (pairsRemaining == 0)
            {
               UnityEngine.Debug.Log($"🎉 You Win! Score: {score}, Turns: {turnsUsed}");
                // TODO: Display win screen or restart UI
            }
        }
        else
        {
            firstCard.Flip(false);
            secondCard.Flip(false);
        }

        firstCard = null;
        secondCard = null;
        canSelect = true;
        GridManager.Instance.canInteract = true;
    }
}
