using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckHandler : MonoBehaviour
{
    private HandManager hand;
    public List<CardData> CardDeck;
    public Card cardPrefab;
    void Awake()
    {
        hand = GameObject.FindWithTag("Hand").GetComponent<HandManager>();
        CreateCards(14);
    }

    //Create cards in hand taking in consideration delay time, number of cards, and cards left on the deck
    IEnumerator GenerateCardsWithDelay(float delay, int cards)
    {
        for (int i = 0; i < cards; i++)
        {
            if (CardDeck.Count > 0)
            {
                Card obj = cardPrefab;
                obj.data = CardDeck[Random.Range(0, CardDeck.Count)];
                CardDeck.Remove(obj.data);
                hand.AddCard(obj.gameObject);
                yield return new WaitForSeconds(delay);
            }
            else
            {
                print("Deck is empty!");
                break;
            }
        }
    }

    public void CreateCards(int i)
    {
        StartCoroutine(GenerateCardsWithDelay(0.1f, i));
    }
}
