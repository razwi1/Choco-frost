using System.Collections;
using System.Diagnostics;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Card firstCard, secondCard;
    private bool canSelect = false;
    public TMP_Text scoretxt;
    public TMP_Text availabletxt;

    private int turnsUsed = 0;
    private int score = 0;
    private int pairsRemaining = 0;

    public GameObject Win;
    public GameObject Fail;

    public int availableTurns { get; private set; }

    public void SetAvailableTurns(int turns)
    {
        availableTurns = turns;
        availabletxt.text = $"{availableTurns}";
        score = 0;
        scoretxt.text = $"{score}";
    }


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
        availableTurns--;
        availabletxt.text = $"{availableTurns}";
        if (availableTurns <= 0 && pairsRemaining > 0)
        {
            UnityEngine.Debug.Log("❌ You ran out of turns. Game Over!");
            Fail.SetActive(true);
            GridManager.Instance.ClearGrid();
            yield break; // Stops further logic
        }
        yield return new WaitForSeconds(0.5f);

        if (firstCard.cardId == secondCard.cardId)
        {
            firstCard.Match();
            secondCard.Match();
            score += 50;
            scoretxt.text = $"{score}";
            pairsRemaining--;

            if (pairsRemaining == 0)
            {
               UnityEngine.Debug.Log($"🎉 You Win! Score: {score}, Turns: {turnsUsed}");
                Win.SetActive(true);
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

    public void QuitGame()
    {
        UnityEngine.Debug.Log("Quitting the game...");
        Application.Quit();
    }

}
