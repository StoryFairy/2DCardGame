using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSender : MonoBehaviour
{
    public ObjectEventSO senderEvent;

    [ContextMenu("SendEvent")]
    public void SendEvent()
    {
        senderEvent.RaisEvent(null, this);
    }
}
