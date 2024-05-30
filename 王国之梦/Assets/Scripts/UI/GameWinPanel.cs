using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameWinPanel : MonoBehaviour
{
    private VisualElement rootElement;
    private Button pickCardButton;
    private Button backToMapButton;

    [Header("事件广播")] 
    public ObjectEventSO loadMapEvent;
    public ObjectEventSO pickCardEvent;

    private void Awake()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        pickCardButton = rootElement.Q<Button>("PickCardButton");
        backToMapButton = rootElement.Q<Button>("BackToMapButton");

        pickCardButton.clicked += OnPickCardButtonClicked;
        backToMapButton.clicked += OnBackToMapButtonClicked;
    }

    private void OnPickCardButtonClicked()
    {
        pickCardEvent.RaisEvent(null, this);
    }

    private void OnBackToMapButtonClicked()
    {
        loadMapEvent.RaisEvent(null, this);
    }

    public void OnFinishPickCardEvent()
    {
        pickCardButton.style.display = DisplayStyle.None;
    }
}
