using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MapLayoutSO mapLayout;

    public List<Enemy> aliveEnemyList = new();

    [Header("事件广播")] public ObjectEventSO gameWinEvent;
    public ObjectEventSO gameOverEvent;

    /// <summary>
    /// 更新房间，加载地图
    /// </summary>
    /// <param name="value"></param>
    public void UpdateMapLayoutData(object value)
    {
        var roomVector = (Vector2Int)value;
        if (mapLayout.mapRoomDataList.Count == 0)
            return;
        var currentRoom = mapLayout.mapRoomDataList.Find(r => r.column == roomVector.x && r.line == roomVector.y);

        currentRoom.roomState = RoomState.Visited;
        var sameColumnRooms = mapLayout.mapRoomDataList.FindAll(r => r.column == currentRoom.column);
        foreach (var room in sameColumnRooms)
        {
            if (room.line != roomVector.y)
                room.roomState = RoomState.Locked;
        }

        foreach (var link in currentRoom.linkTo)
        {
            var linkedRoom = mapLayout.mapRoomDataList.Find(r => r.column == link.x && r.line == link.y);
            linkedRoom.roomState = RoomState.Attainable;
        }

        aliveEnemyList.Clear();
    }

    public void OnRoomLoadedEvent(object obj)
    {
        var enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            aliveEnemyList.Add(enemy);
        }
    }

    public void OnCharacterDeadEvent(object character)
    {
        if (character is Player)
        {
            //发出失败的通知
            StartCoroutine(EventDelayAction(gameOverEvent));
        }

        if (character is Enemy)
        {
            aliveEnemyList.Remove(character as Enemy);

            if (aliveEnemyList.Count == 0)
            {
                //发出获胜的通知
                StartCoroutine(EventDelayAction(gameWinEvent));
            }
        }
    }

    IEnumerator EventDelayAction(ObjectEventSO eventSO)
    {
        yield return new WaitForSeconds(1.5f);
        eventSO.RaisEvent(null, this);
    }

    public void OnNewGameEvent()
    {
        mapLayout.mapRoomDataList.Clear();
        mapLayout.linePositionList.Clear();
    }
}
