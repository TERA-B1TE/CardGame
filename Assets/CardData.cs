using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card Game/CardData")]
public class CardData : ScriptableObject
{
    public enum CardType
    {
        // Number Cards
        Zero,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,

        // Action Cards
        Skip,
        Reverse,
        DrawTwo,

        // Wild Cards
        Wild,
        WildDrawFour
    }

    public enum CardColor
    {
        Red,
        Yellow,
        Green,
        Blue
    }

    public Sprite CardImage;
    public CardType Type;
    public CardColor Color;


}

[CustomEditor(typeof(CardData))]
public class CardTypeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CardData cardData = (CardData)target;

        cardData.CardImage = (Sprite)EditorGUILayout.ObjectField("Card Image", cardData.CardImage, typeof(Sprite), false);
        cardData.Type = (CardData.CardType)EditorGUILayout.EnumPopup("Type", cardData.Type);

        DrawColorField(cardData);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cardData);
        }
    }

    private void DrawColorField(CardData cardData)
    {
        if (cardData.Type != CardData.CardType.Wild && cardData.Type != CardData.CardType.WildDrawFour)
        {
            cardData.Color = (CardData.CardColor)EditorGUILayout.EnumPopup("Color", cardData.Color);
        }
    }
}
