using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int column;
    public int line;
    private SpriteRenderer _spriteRenderer;
    public RoomDataSO roomData;
    public RoomState roomState;
    public List<Vector2Int> linkTo = new();

    [Header("广播")] public ObjectEventSO loadRoomEvent;

    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        if(roomState==RoomState.Attainable)
            loadRoomEvent.RaisEvent(this, this);
    }

    public void SetupRoom(int column, int line, RoomDataSO roomData)
    {
        this.column = column;
        this.line = line;
        this.roomData = roomData;
        _spriteRenderer.sprite = roomData.roomIcon;

        _spriteRenderer.color = roomState switch
        {
            RoomState.Locked => Color.gray,
            RoomState.Visited => Color.white,
            RoomState.Attainable=> Color.yellow,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
