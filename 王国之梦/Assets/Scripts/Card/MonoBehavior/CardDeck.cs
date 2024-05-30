using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class CardDeck : MonoBehaviour
{
    public CardManager cardManager;
    public CardLayoutManager cardLayoutManager;
    public Vector3 deckPosition;

    private List<CardDataSO> drawDeck = new(); //抽牌堆
    private List<CardDataSO> discardDeck = new(); //弃牌堆
    private List<Card> handCardObjectList = new(); //当前手牌堆

    [Header("事件广播")]
    public IntEventSO drawCountEvent;
    public IntEventSO discardCountEvent;

    private void Start()
    {
        InitializeDeck();
    }

    public void InitializeDeck()
    {
        drawDeck.Clear();
        foreach (var entry in cardManager.currentLibrary.cardLibraryList)
        {
            for (int i = 0; i < entry.amount; i++)
            {
                drawDeck.Add(entry.cardData);
            }
        }

        ShuffleDeck();
    }

    [ContextMenu("TestDrawCard")]
    public void TestDrawCard()
    {
        DrawCard(1);
    }

    public void NewTurnDrawCards()
    {
        DrawCard(4);
    }

    public void DrawCard(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            CardDataSO currentCardData = drawDeck[0];
            drawDeck.RemoveAt(0);
            if (drawDeck.Count == 0)
            {
                foreach (var item in discardDeck)
                {
                    drawDeck.Add(item);
                }

                ShuffleDeck();
            }

            drawCountEvent.RaisEvent(drawDeck.Count, this);
            
            var card = cardManager.GetCardObject().GetComponent<Card>();
            card.Init(currentCardData);
            card.transform.position = deckPosition;

            handCardObjectList.Add(card);
            var delay = i * 0.2f;
            SetCardLayout(delay);
        }
    }

    private void SetCardLayout(float delay)
    {
        for (int i = 0; i < handCardObjectList.Count; i++)
        {
            Card currentCard = handCardObjectList[i];

            CardTransform cardTransform = cardLayoutManager.GetCardTransform(i, handCardObjectList.Count);
            //currentCard.transform.SetPositionAndRotation(cardTransform.position, cardTransform.rotation);
            
            //卡牌能量判断
            currentCard.UpdateCardState();
            
            currentCard.isAnimating = true;

            currentCard.transform.DOScale(Vector3.one, 0.2f).SetDelay(delay).onComplete = () =>
            {
                currentCard.transform.DOMove(cardTransform.position, 0.5f).onComplete =
                    () => currentCard.isAnimating = false;
                currentCard.transform.DORotateQuaternion(cardTransform.rotation, 0.5f);
            };

            //设置卡牌排序
            currentCard.GetComponent<SortingGroup>().sortingOrder = i;
            currentCard.UpdatePositionRotation(cardTransform.position, cardTransform.rotation);
        }
    }

    /// <summary>
    /// 洗牌
    /// </summary>
    private void ShuffleDeck()
    {
        discardDeck.Clear();

        drawCountEvent.RaisEvent(drawDeck.Count, this);
        discardCountEvent.RaisEvent(discardDeck.Count, this);

        for (int i = 0; i < drawDeck.Count; i++)
        {
            CardDataSO temp = drawDeck[i];
            int randomIndex = Random.Range(i, drawDeck.Count);
            drawDeck[i] = drawDeck[randomIndex];
            drawDeck[randomIndex] = temp;
        }
    }

    /// <summary>
    /// 弃牌
    /// </summary>
    /// <param name="card"></param>
    public void DiscardCard(object obj)
    {
        Card card = obj as Card;
        
        discardDeck.Add(card.cardData);
        handCardObjectList.Remove(card);

        cardManager.DiscardCard(card.gameObject);
        
        discardCountEvent.RaisEvent(discardDeck.Count, this);
        SetCardLayout(0f);
    }

    public void OnPlayerTurnEnd()
    {
        for (int i = 0; i < handCardObjectList.Count; i++)
        {
            discardDeck.Add(handCardObjectList[i].cardData);
            cardManager.DiscardCard(handCardObjectList[i].gameObject);
        }

        handCardObjectList.Clear();
        discardCountEvent.RaisEvent(discardDeck.Count, this);
    }

    public void ReleaseAllCards(object obj)
    {
        foreach (var card in handCardObjectList)
        {
            cardManager.DiscardCard(card.gameObject);
        }
        
        handCardObjectList.Clear();
        InitializeDeck();
    }
}
