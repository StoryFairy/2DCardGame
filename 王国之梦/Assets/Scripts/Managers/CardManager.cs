using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

public class CardManager : MonoBehaviour
{
    public PoolTool poolTool;
    public List<CardDataSO> cardDataList;

    public CardLibrarySO newGameCardLibrary;
    public CardLibrarySO currentLibrary;

    private int previousIndex;
    private void Awake()
    {
        InitializeCardDataList();

        foreach (var item in newGameCardLibrary.cardLibraryList)
        {
            currentLibrary.cardLibraryList.Add(item);
        }
    }

    private void OnDisable()
    {
        currentLibrary.cardLibraryList.Clear();
    }

    #region 获取项目卡牌

        private void InitializeCardDataList()
        {
            Addressables.LoadAssetsAsync<CardDataSO>("CardData", null).Completed += OnCardDataLoaded;
        }
    
        private void OnCardDataLoaded(AsyncOperationHandle<IList<CardDataSO>> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                cardDataList = new List<CardDataSO>(handle.Result);
            }
            else
            {
                Debug.Log("Failed to load card data");
            }
        }

    #endregion

    /// <summary>
    /// 抽卡时调用的函数获得卡牌GameObject
    /// </summary>
    /// <returns></returns>
    public GameObject GetCardObject()
    {
        var cardObj = poolTool.GetObjectFromPool();
        cardObj.transform.localScale = Vector3.zero;
        return cardObj;
    }

    public void DiscardCard(GameObject cardObject)
    {
        poolTool.ReleaseObjectToPool(cardObject);
    }

    public CardDataSO GetNewCardData()
    {
        var randomIndex = 0;
        do
        {
            randomIndex = Random.Range(0, cardDataList.Count);
        } while (previousIndex == randomIndex);
        
        previousIndex = randomIndex;
        return cardDataList[randomIndex];
    }

    /// <summary>
    /// 解锁添加新卡牌
    /// </summary>
    /// <param name="newCardData"></param>
    public void UnlockCard(CardDataSO newCardData)
    {
        var newCard = new CardLibraryEntry
        {
            cardData = newCardData,
            amount = 1,
        };

        if (currentLibrary.cardLibraryList.Contains(newCard))
        {
            var target = currentLibrary.cardLibraryList.Find(t => t.cardData == newCardData);
            target.amount++;
        }
        else
        {
            currentLibrary.cardLibraryList.Add(newCard);
        }
    }
}
