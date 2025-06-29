using UnityEngine;

public class Card : MonoBehaviour
{
    public string cardId;
    public AudioClip CardFlip_Aud;
    public bool IsMatched { get; private set; }
    public bool IsFlipped { get; private set; }

    [SerializeField] private GameObject frontObject; // child object for front
    [SerializeField] private GameObject backObject;  // child object for back

    private SpriteRenderer frontRenderer;
    private SpriteRenderer backRenderer;

    void Awake()
    {
        // Get SpriteRenderers on the child objects
        frontRenderer = frontObject.GetComponent<SpriteRenderer>();
        backRenderer = backObject.GetComponent<SpriteRenderer>();
    }

    public void SetCard(Sprite front, Sprite back, string name)
    {
        cardId = name; // Card ID for matching logic
        IsMatched = false;

        // Assign sprite images
        frontRenderer.sprite = front;
        backRenderer.sprite = back;

        Flip(false); // Start face-down
    }

    void OnMouseDown()
    {
        if (!IsMatched && !IsFlipped && GridManager.Instance.canInteract)
        {
            GameManager.Instance?.CardSelected(this);
        }
    }

    public void Flip(bool showFront)
    {
        IsFlipped = showFront;
        frontObject.SetActive(showFront);
        backObject.SetActive(!showFront);
        GameManager.Instance?.PlaySound(CardFlip_Aud);
    }

    public void Match()
    {
        IsMatched = true;
        gameObject.SetActive(false);
    }
}
