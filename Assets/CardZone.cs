using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public List<Card> Cards;
    public float MoveSpeed;
    public GameObject Highlight;
    private HandManager hand;

    public void Awake()
    {
        hand = GameObject.FindWithTag("Hand").GetComponent<HandManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hand.ActiveCardZone = this;
        hand.SetIsMouseControlling(true);
        hand.SetIsCardSelected(false);
        hand.ActiveCardZone = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hand.ActiveCardZone == this)
            hand.ActiveCardZone = null;
        SetHighlight(false);
    }

    public void SetHighlight(bool state)
    {
        if (Highlight != null)
        {
            Highlight.SetActive(state);
        }
    }

    public void AddCard(Card card)
    {
        card.transform.SetParent(transform);
        StartCoroutine(AnimateCardPlacement(card.GetComponent<RectTransform>()));
        Cards.Add(card);
    }

    IEnumerator AnimateCardPlacement(RectTransform rect)
    {
        while (Vector3.Distance(rect.localScale, Vector3.one) > 0.01f && Vector3.Distance(rect.localPosition, Vector3.zero) > 0.01f && Vector3.Distance(rect.localRotation.eulerAngles, Vector3.zero) > 0.01f)
        {
            rect.localScale = Vector3.Lerp(rect.localScale, Vector3.one, Time.deltaTime * MoveSpeed);
            rect.localPosition = Vector3.Lerp(rect.localPosition, Vector3.zero, Time.deltaTime * MoveSpeed);
            rect.localRotation = Quaternion.Lerp(rect.localRotation, Quaternion.identity, Time.deltaTime * MoveSpeed * 2);
            yield return null;
        }
        rect.localScale = Vector3.one;
        rect.localPosition = Vector3.zero;
        rect.localRotation = Quaternion.identity;
    }
}
