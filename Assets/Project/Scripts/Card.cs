using UnityEngine;

public class Card : MonoBehaviour
{
    public SpriteRenderer frontRenderer;
    public SpriteRenderer backRenderer;

    private Sprite frontSprite;
    private bool isFlipped = false;
    private int cardId;

    public void SetCard(Sprite front, Sprite back, int id)
    {
        frontSprite = front;
        frontRenderer.sprite = front;
        backRenderer.sprite = back;
        cardId = id;

        Flip(false); // Default to back
    }

    void OnMouseDown()
    {
        Flip(!isFlipped); // Flip on click
    }

    public void Flip(bool showFront)
    {
        isFlipped = showFront;
        frontRenderer.gameObject.SetActive(showFront);
        backRenderer.gameObject.SetActive(!showFront);
    }
}
