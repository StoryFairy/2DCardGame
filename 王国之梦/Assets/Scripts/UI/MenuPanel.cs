using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuPanel : MonoBehaviour
{
    private VisualElement rootElement;
    private Button newGameButton, exitGameButton;

    public ObjectEventSO newGameEvent;

    private void OnEnable()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        newGameButton = rootElement.Q<Button>("NewGameButton");
        exitGameButton = rootElement.Q<Button>("ExitGameButton");
        newGameButton.clicked += OnNewGameButtonClicked;
        exitGameButton.clicked += OnExitGameButtonClicked;
    }

    private void OnNewGameButtonClicked()
    {
        newGameEvent.RaisEvent(null, this);
    }

    private void OnExitGameButtonClicked()
    {
        Application.Quit();
    }
}
