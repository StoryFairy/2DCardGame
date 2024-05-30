using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "MapLayoutSO",menuName = "Map/MapLayoutSO")]
public class MapLayoutSO : ScriptableObject
{
    public List<MapRoomData> mapRoomDataList = new();
    public List<LinePosition> linePositionList = new();
}

[Serializable]
public class MapRoomData
{
    public float posX, posY;
    public int column;
    public int line;
    public RoomDataSO roomData;
    public RoomState roomState;
    public List<Vector2Int> linkTo;
}

[Serializable]
public class LinePosition
{
    public SerializeVector3 startPos, endPos;
}