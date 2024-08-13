using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public CardData data;
    public Image CardImage;

    private HandManager hand;
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        SetupCard();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    //Setup visuals and card information according to data
    private void SetupCard()
    {
        CardImage.sprite = data.CardImage;
    }

    //Detects when mouse is over card
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hand == null) return;
        hand.SetIsCardSelected(true);
            hand.SetIsMouseControlling(true);
    }

    //Detects when mouse leaves card
    public void OnPointerExit(PointerEventData eventData)
    {
        if (hand == null) return;
        hand.SetIsCardSelected(false);
    }

    //Detects when mouse hold card
    public void OnPointerDown(PointerEventData eventData)
    {
        if (hand == null) return;
        hand.HoldingCard = this;
            hand.Cards.Remove(this);
            hand.Cards.Add(null);
            GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    //Detects when mouse releases card
    public void OnPointerUp(PointerEventData eventData)
    {
        if (hand == null) return;
        HandleCardRelease();
    }

    //Move card to mouse while being holded
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out worldPoint);
        rectTransform.localPosition = rectTransform.parent.InverseTransformPoint(worldPoint);
    }

    //Detects where the card should be released
    private void HandleCardRelease()
    {
        hand.HoldingCard = null;
        hand.Cards.Remove(null);
        if (hand.ActiveCardZone != null)
        {
            canvas = hand.ActiveCardZone.GetComponentInParent<Canvas>();
            hand.ActiveCardZone.AddCard(this);
            hand = null;
        }
        else
        {
            hand.Cards.Insert(hand.LastSelected, this);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    //Send the card directly to a card zone if there is one active
    public void SendCard()
    {
        if (hand == null || hand.ActiveCardZone == null) return;
            hand.Cards.Remove(this);
            canvas = hand.ActiveCardZone.GetComponentInParent<Canvas>();
            hand.ActiveCardZone.AddCard(this);

            hand.LastSelected = 0;
            hand = null;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void SetHand(HandManager i)
    { hand = i; }
}
