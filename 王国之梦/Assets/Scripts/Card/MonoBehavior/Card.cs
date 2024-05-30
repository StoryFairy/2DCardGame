using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SpriteRenderer cardSprite;
    public TextMeshPro costText, descriptionText, typeText;

    public CardDataSO cardData;

    [Header("原始数据")] public Vector3 originalPosition;
    public Quaternion originalRotation;
    public int originalLayerOrder;

    public bool isAnimating;
    public bool isAvailiable;

    public Player player;

    [Header("广播方法")] public ObjectEventSO discardCardEvent;
    public IntEventSO costEvent;

    public void Init(CardDataSO data)
    {
        cardData = data;
        cardSprite.sprite = data.cardImage;
        costText.text = data.cost.ToString();
        descriptionText.text = data.cardDescription;
        typeText.text = data.cardType switch
        {
            CardType.Attack => "攻击",
            CardType.Defense => "防御",
            CardType.Abilities => "技能",
            _ => throw new NotImplementedException()
        };

        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    public void UpdatePositionRotation(Vector3 position, Quaternion rotation)
    {
        originalPosition = position;
        originalRotation = rotation;
        originalLayerOrder = GetComponent<SortingGroup>().sortingOrder;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isAnimating) return;
        transform.position = originalPosition + Vector3.up;
        transform.rotation = quaternion.identity;
        GetComponent<SortingGroup>().sortingOrder = 20;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isAnimating) return;
        RestCardTransform();
    }

    public void RestCardTransform()
    {
        transform.SetPositionAndRotation(originalPosition, originalRotation);
        GetComponent<SortingGroup>().sortingOrder = originalLayerOrder;
    }

    public void ExecuteCardEffects(CharacterBase from, CharacterBase target)
    {
        costEvent.RaisEvent(cardData.cost, this);
        discardCardEvent.RaisEvent(this, this);
        foreach (var effect in cardData.effects)
        {
            effect.Execute(from, target);
        }
    }

    public void UpdateCardState()
    {
        isAvailiable = cardData.cost <= player.CurrentMana;
        costText.color = isAvailiable ? Color.green : Color.red;
    }
}
