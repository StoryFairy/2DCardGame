using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    private AssetReference currentScene;
    public AssetReference map;
    public AssetReference menu;

    private Vector2Int currentRoomVector;
    private Room currentRoom;

    [Header("事件广播")] public ObjectEventSO afterRoomLoadedEvent;
    public ObjectEventSO updateRoomEvent;

    private void Awake()
    {
        currentRoomVector = Vector2Int.one * -1;
        LoadMenu();
    }

    public async void OnLoadRoomEvent(object data)
    {
        if (data is Room)
        {
            currentRoom = data as Room;

            var currentData = currentRoom.roomData;
            currentRoomVector = new(currentRoom.column, currentRoom.line);

            currentScene = currentData.sceneToLoad;
        }

        await UnloadSceneTask();
        await LoadSceneTask();

        afterRoomLoadedEvent.RaisEvent(currentRoom, this);
    }

    /// <summary>
    /// 异步操作加载场景
    /// </summary>
    private async Awaitable LoadSceneTask()
    {
        var s = currentScene.LoadSceneAsync(LoadSceneMode.Additive);
        await s.Task;

        if (s.Status == AsyncOperationStatus.Succeeded)
        {
            SceneManager.SetActiveScene(s.Result.Scene);
        }
    }

    private async Awaitable UnloadSceneTask()
    {
        await SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
    }

    public async void LoadMap()
    {
        await UnloadSceneTask();
        if (currentRoomVector != Vector2.one * -1)
        {
            updateRoomEvent.RaisEvent(currentRoomVector, this);
        }

        currentScene = map;
        await LoadSceneTask();
    }

    public async void LoadMenu()
    {
        if (currentScene != null)
            await UnloadSceneTask();
        currentScene = menu;
        await LoadSceneTask();
    }
}
