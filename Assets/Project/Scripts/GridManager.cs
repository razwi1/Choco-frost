using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int currentLevel = 1; // 1: 2x2, 2: 4x4, 3: 5x6
    private int rows, cols;
    private int totalCards, numPairs;

    [Header("Card Settings")]
    public GameObject cardPrefab;
    public Sprite[] cardFrontSprites;
    public Sprite cardBackSprite;

    private List<Card> cards = new List<Card>();

    void Start()
    {
        SetupLevel(currentLevel);
    }

    void SetupLevel(int level)
    {
        switch (level)
        {
            case 1: rows = 2; cols = 2; break;
            case 2: rows = 4; cols = 4; break;
            case 3: rows = 5; cols = 6; break;
            default: rows = 2; cols = 2; break;
        }

        totalCards = rows * cols;
        numPairs = totalCards / 2;

        if (cardFrontSprites == null || cardFrontSprites.Length == 0)
        {
            Debug.LogError("Card front sprites not assigned in the inspector!");
            return;
        }

        GenerateGrid();
        StartCoroutine(PreviewAndHideCards());
    }

    void GenerateGrid()
    {
        // Clear old cards
        foreach (var c in cards) Destroy(c.gameObject);
        cards.Clear();

        // Create and shuffle card pairs
        List<Sprite> selected = new List<Sprite>();
        for (int i = 0; i < numPairs; i++)
        {
            Sprite s = cardFrontSprites[i % cardFrontSprites.Length];
            selected.Add(s);
            selected.Add(s);
        }

        for (int i = 0; i < selected.Count; i++)
        {
            Sprite temp = selected[i];
            int rand = Random.Range(i, selected.Count);
            selected[i] = selected[rand];
            selected[rand] = temp;
        }

        // Get card size using sample sprite
        Sprite sampleSprite = cardFrontSprites[0];
        if (sampleSprite == null)
        {
            Debug.LogError("First card sprite is missing!");
            return;
        }

        Vector2 cardOriginalSize = sampleSprite.bounds.size;

        float spacingFactor = 0.2f; // 20% of card size
        float spacingX = cardOriginalSize.x * spacingFactor;
        float spacingY = cardOriginalSize.y * spacingFactor;

        // Calculate screen dimensions with padding
        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * Camera.main.aspect;
        float screenPaddingFactor = 0.9f; // 90% of screen space for grid
        screenWidth *= screenPaddingFactor;
        screenHeight *= screenPaddingFactor;

        // Total unscaled grid size
        float totalWidth = cols * cardOriginalSize.x + (cols - 1) * spacingX;
        float totalHeight = rows * cardOriginalSize.y + (rows - 1) * spacingY;

        // Uniform scaling factor
        float scaleX = screenWidth / totalWidth;
        float scaleY = screenHeight / totalHeight;
        float uniformScale = Mathf.Min(scaleX, scaleY, 1f);

        // Apply scaling to card and spacing
        Vector2 scaledCardSize = cardOriginalSize * uniformScale;
        spacingX = scaledCardSize.x * spacingFactor;
        spacingY = scaledCardSize.y * spacingFactor;

        float gridWidth = cols * scaledCardSize.x + (cols - 1) * spacingX;
        float gridHeight = rows * scaledCardSize.y + (rows - 1) * spacingY;

        Vector2 startPos = new Vector2(
            -gridWidth / 2f + scaledCardSize.x / 2f,
            gridHeight / 2f - scaledCardSize.y / 2f
        );

        for (int i = 0; i < totalCards; i++)
        {
            int row = i / cols;
            int col = i % cols;

            Vector2 pos = new Vector2(
                startPos.x + col * (scaledCardSize.x + spacingX),
                startPos.y - row * (scaledCardSize.y + spacingY)
            );

            GameObject card = Instantiate(cardPrefab, pos, Quaternion.identity, transform);
            card.transform.localScale = Vector3.one * uniformScale;

            Card cardComp = card.GetComponent<Card>();
            cardComp.SetCard(selected[i], cardBackSprite, i);
            cards.Add(cardComp);
        }
    }

    IEnumerator PreviewAndHideCards()
    {
        foreach (var c in cards)
        {
            c.Flip(true);
        }

        yield return new WaitForSeconds(2f);

        foreach (var c in cards)
        {
            c.Flip(false);
        }
    }
}
