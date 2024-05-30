using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener : MonoBehaviour
{
    public void OnEventRaised()
    {
        Debug.Log("收到消息");
    }
}
