using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    [Header("Configuration")]
    public float HandSize;
    private float totalSize;
    public float HandHeight;
    public float CardMaxDistance;
    public float AngleMultiplier;
    public float MoveSpeed;
    public float SelectedHeight;
    private bool IsMouseControlling;
    private bool IsCardSelected;
    private bool ControllerDelay;
    private GameObject[] cardZones;

    [Header("Hand")]
    public int LastSelected;
    private int LastZoneSelected;
    public CardZone ActiveCardZone;
    public Card HoldingCard;
    public List<Card> Cards;
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        cardZones = GameObject.FindGameObjectsWithTag("CardPlacement");
    }

    void Update()
    {
        if (Cards.Count > 0)
        {
            HandleCardSelection();
            UpdateCardPosition();
        }
    }

    //Handles the selection of the card either by mouse or controller/keyboard
    void HandleCardSelection()
    {
        if (HoldingCard != null)
        {
            int i = Cards.IndexOf(null);
            Cards.RemoveAt(i);
            Cards.Insert(LastSelected, null);
        }

        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && !ControllerDelay && HoldingCard == null)
        {
            HandleControllerSelection();
        }
        else if (IsMouseControlling)
        {
            HandleMouseSelection();
        }
        else if (Input.GetButtonDown("Submit"))
        {
            Cards[LastSelected]?.SendCard();
        }
        UpdateActiveCardZone();
    }

    //Handles Controller/Keyboard selection
    private void HandleControllerSelection()
    {
        IsCardSelected = true;
        IsMouseControlling = false;
        StartCoroutine(ControllerSelectDelay(0.1f));
    }

    //Handles Mouse selection
    private void HandleMouseSelection()
    {
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out worldPoint);
        LastSelected = Mathf.Clamp((int)(((rectTransform.parent.InverseTransformPoint(worldPoint).x + (totalSize / 2f)) / totalSize) * Cards.Count), 0, Cards.Count - 1);
    }

    //Creates a delay during the selection using the controller/keyboard
    IEnumerator ControllerSelectDelay(float delay)
    {
        ControllerDelay = true;

        LastSelected = (LastSelected += (int)Input.GetAxis("Horizontal")) < 0 ? Cards.Count-1 : LastSelected > Cards.Count-1 ? 0 : LastSelected;
        if (Input.GetAxis("Vertical") != 0)
        {
            LastZoneSelected = (LastZoneSelected += (int)Input.GetAxis("Vertical")) < 0 ? cardZones.Length - 1 : LastZoneSelected > cardZones.Length - 1 ? 0 : LastZoneSelected;
            ActiveCardZone = cardZones[LastZoneSelected].GetComponent<CardZone>();
        }
        yield return new WaitForSeconds(delay);

        ControllerDelay = false;
    }

    //Updates visual feedback from card zones
    private void UpdateActiveCardZone()
    {
        for (int i = 0; i < cardZones.Length; i++)
        {
            var cardZone = cardZones[i].GetComponent<CardZone>();
            if (ActiveCardZone == cardZone)
            {
                cardZone.SetHighlight(true);
            }
            else
            {
                cardZone.SetHighlight(false);
            }
        }
    }

    //Handles all card hand movements
    void UpdateCardPosition()
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            if (Cards[i] != null)
            {
                Vector3 pos = Vector3.up*HandHeight;
                float angleOffset = 0;

                // Only calculate if there is more than one card
                if (Cards.Count > 1)
                {
                    // Calculate the initial spacing between cards
                    float spacing = HandSize / (Cards.Count - 1);

                    // Use CardMaxDistance if spacing exceeds it
                    if (spacing > CardMaxDistance)
                        spacing = CardMaxDistance;

                    // Calculate the total size of the hand
                    totalSize = spacing * (Cards.Count - 1);

                    // Calculate position using the adjusted spacing
                    pos = (Vector3.left * totalSize / 2) + Vector3.right * spacing * i;

                    // Calculate height offset
                    float heightOffset = Mathf.Sin((float)i / (Cards.Count - 1) * Mathf.PI) * HandHeight;
                    pos += Vector3.up * heightOffset;

                    // Calculate angle based on height
                    float angle = Mathf.Clamp(HandHeight / AngleMultiplier, -90, 90);
                    angleOffset = Mathf.Lerp(angle, -angle, (float)i / (Cards.Count - 1));
                }

                RectTransform rect = Cards[i].GetComponent<RectTransform>();

                // Set card rotation
                rect.localRotation = Quaternion.Euler(0, 0, angleOffset);

                // Highlight the selected card by moving it upwards and set other cards to their corresponding hierarchy
                if (LastSelected == i && IsCardSelected)
                    pos += Quaternion.Euler(0, 0, angleOffset) * Vector3.up * SelectedHeight;
                else
                    Cards[i].transform.SetSiblingIndex(i);

                // Set card to their calculated position with Lerp
                rect.localPosition = Vector3.Lerp(rect.localPosition, pos, Time.deltaTime * MoveSpeed);
            }
        }
        // Set selected card to last in hierarchy
        if (IsCardSelected && Cards[LastSelected] != null)
            Cards[LastSelected].transform.SetAsLastSibling();
    }

    //Handles creating cards in the hand
    public void AddCard(GameObject obj)
    {
        if (obj.GetComponent<Card>() != null)
        {
            Card o = Instantiate(obj, transform).GetComponent<Card>();
            o.GetComponent<RectTransform>().localPosition = Vector3.up*-HandHeight;
            o.SetHand(this);
            o.gameObject.name = o.data.Type.ToString() + (o.data.Type == CardData.CardType.Wild || o.data.Type == CardData.CardType.WildDrawFour ? "" : " " + o.data.Color);
            Cards.Add(o);
        }
    }

    public void SetIsMouseControlling(bool i)
    { IsMouseControlling = i; }

    public void SetIsCardSelected(bool i)
    { IsCardSelected = i; }

}
