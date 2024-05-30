using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class Line : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public float offsetSpeed = 0.1f;

    private void Update()
    {
        if (lineRenderer != null)
        {
            var material = lineRenderer.material;
            var offset = material.mainTextureOffset;

            offset.x += offsetSpeed * Time.deltaTime;

            material.mainTextureOffset = offset;
        }
    }
}
