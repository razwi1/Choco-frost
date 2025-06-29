using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    private int currentLevel; // 1: 2x2, 2: 4x4, 3: 5x6
    private int rows, cols;
    private int totalCards, numPairs, availableturns;

    [Header("Card Settings")]
    public GameObject cardPrefab;
    public Sprite[] cardFrontSprites;
    public Sprite cardBackSprite;

    public GameObject Credits;

    private List<Card> cards = new List<Card>();

    public static GridManager Instance { get; private set; }
    public bool canInteract = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        //SetupLevel(currentLevel);
    }

    public void SetupLevel(int level)
    {
        currentLevel = level;
        // Set grid size
        (rows, cols) = level switch
        {
            1 => (2, 2),
            2 => (4, 4),
            3 => (5, 6),
            _ => (2, 2)
        };

        totalCards = rows * cols;
        numPairs = totalCards / 2;
        availableturns = totalCards;
        GameManager.Instance?.SetAvailableTurns(availableturns);

        if (cardFrontSprites == null || cardFrontSprites.Length < numPairs)
        {
            UnityEngine.Debug.LogError("Not enough card front sprites assigned!");
            return;
        }

        GenerateGrid();

        GameManager.Instance?.InitializeGame(numPairs);

        StartCoroutine(PreviewAndHideCards());
        
    }

    void GenerateGrid()
    {
        // Clear previous cards
        foreach (var c in cards)
            Destroy(c.gameObject);
        cards.Clear();

        // Prepare card pairs and shuffle
        List<Sprite> selectedSprites = new List<Sprite>();
        for (int i = 0; i < numPairs; i++)
        {
            Sprite s = cardFrontSprites[i];
            selectedSprites.Add(s);
            selectedSprites.Add(s);
        }

        for (int i = 0; i < selectedSprites.Count; i++)
        {
            Sprite temp = selectedSprites[i];
            int rand = Random.Range(i, selectedSprites.Count);
            selectedSprites[i] = selectedSprites[rand];
            selectedSprites[rand] = temp;
        }

        // Calculate layout
        Sprite sampleSprite = cardFrontSprites[0];
        Vector2 cardSize = sampleSprite.bounds.size;
        float spacingFactor = 0.2f;

        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * Camera.main.aspect;
        float screenPadding = 0.9f;

        float totalWidth = cols * cardSize.x + (cols - 1) * cardSize.x * spacingFactor;
        float totalHeight = rows * cardSize.y + (rows - 1) * cardSize.y * spacingFactor;

        float uniformScale = Mathf.Min(
            (screenWidth * screenPadding) / totalWidth,
            (screenHeight * screenPadding) / totalHeight,
            1f
        );

        Vector2 scaledCardSize = cardSize * uniformScale;
        float spacingX = scaledCardSize.x * spacingFactor;
        float spacingY = scaledCardSize.y * spacingFactor;

        float gridWidth = cols * scaledCardSize.x + (cols - 1) * spacingX;
        float gridHeight = rows * scaledCardSize.y + (rows - 1) * spacingY;

        Vector2 startPos = new Vector2(
            -gridWidth / 2f + scaledCardSize.x / 2f,
            gridHeight / 2f - scaledCardSize.y / 2f
        );

        // Instantiate cards
        for (int i = 0; i < totalCards; i++)
        {
            int row = i / cols;
            int col = i % cols;

            Vector2 pos = new Vector2(
                startPos.x + col * (scaledCardSize.x + spacingX),
                startPos.y - row * (scaledCardSize.y + spacingY)
            );

            GameObject cardGO = Instantiate(cardPrefab, pos, Quaternion.identity, transform);
            cardGO.transform.localScale = Vector3.one * uniformScale;

            Card card = cardGO.GetComponent<Card>();

            card.SetCard(selectedSprites[i], cardBackSprite, selectedSprites[i].name);
            cards.Add(card);
        }
       
    }

    IEnumerator PreviewAndHideCards()
    {
        foreach (var card in cards)
            card.Flip(true);

        yield return new WaitForSeconds(2f);

        foreach (var card in cards)
            card.Flip(false);

        canInteract = true;
    }

    public void ClearGrid()
    {
        foreach (var card in cards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }

        cards.Clear();
        canInteract = false;
    }

    public void retry()
    {
        SetupLevel(currentLevel);
    }

    public void Next_Level()
    {
       ClearGrid();
       if(currentLevel <= 3)
        {
            currentLevel++;
            SetupLevel(currentLevel);
        }
        else
        {
            Credits.SetActive(true);
        }
    }
}
