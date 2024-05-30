using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLayoutManager : MonoBehaviour
{
    public bool isHorizontal;

    public float maxWidth = 7f;
    public float cardSpacing = 2f;

    [Header("弧形参数")] public float angleBetweenCards = 7f;

    public float radius = 17f;

    public Vector3 centerPoint;

    private List<Vector3> cardPositions = new();
    private List<Quaternion> cardRotations = new();


    private void Awake()
    {
        centerPoint = isHorizontal ? Vector3.up * -4.5f : Vector3.up * -21.5f;
    }

    public CardTransform GetCardTransform(int index, int totalCards)
    {
        CalculatePosition(totalCards, isHorizontal);
        return new CardTransform(cardPositions[index], cardRotations[index]);
    }

    private void CalculatePosition(int numberOfCards, bool horizontal)
    {
        cardPositions.Clear();
        cardRotations.Clear();
        if (horizontal)
        {
            float currentWidth = cardSpacing * (numberOfCards - 1);
            float totalWidth = Mathf.Min(currentWidth, maxWidth);

            float currentSpacing = totalWidth > 0 ? totalWidth / (numberOfCards - 1) : 0;

            for (int i = 0; i < numberOfCards; i++)
            {
                float xPos = 0 - (totalWidth / 2) + (i * currentSpacing);
                var pos = new Vector3(xPos, centerPoint.y, 0f);
                var rotation = Quaternion.identity;

                cardPositions.Add(pos);
                cardRotations.Add(rotation);
            }
        }
        else
        {
            float cardAngle = (numberOfCards - 1) * angleBetweenCards / 2;

            for (int i = 0; i < numberOfCards; i++)
            {
                var pos = FanCardPosition(cardAngle - i * angleBetweenCards);

                var rotation = Quaternion.Euler(0, 0, cardAngle - i * angleBetweenCards);
                cardPositions.Add(pos);
                cardRotations.Add(rotation);
            }
        }
    }

    private Vector3 FanCardPosition(float angle)
    {
        return new Vector3(
            centerPoint.x - Mathf.Sin(Mathf.Deg2Rad * angle) * radius,
            centerPoint.y + Mathf.Cos(Mathf.Deg2Rad * angle) * radius,
            0
            );
    }
}
