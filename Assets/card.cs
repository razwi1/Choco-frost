using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public GameObject front;
    public GameObject back;

    public int cardID; // Unique ID to match pairs

    private bool isFlipped = false;

    public void Flip()
    {
        if (isFlipped) return;

        isFlipped = true;
        front.SetActive(true);
        back.SetActive(false);

        GameManager.Instance.OnCardFlipped(this);
    }

    public void FlipBack()
    {
        isFlipped = false;
        front.SetActive(false);
        back.SetActive(true);
    }
}

