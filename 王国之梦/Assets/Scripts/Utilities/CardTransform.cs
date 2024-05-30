using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CardTransform
{
    public Vector3 position;
    public Quaternion rotation;
    
    public CardTransform(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}
