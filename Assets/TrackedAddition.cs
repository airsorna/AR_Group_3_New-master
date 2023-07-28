using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackedAddition : MonoBehaviour
{
    public Guid activatingGuid;

    public ImageTrackingObjectManager imageTracking;

    private Canvas canvas;

    private string prettyGUID;

    private void LazyLoadCanvas()
    {
        if ( canvas != null )
        {
            return;
        }
        canvas = GetComponent<Canvas>();
    }

    private void Update()
    {
        prettyGUID = activatingGuid.ToString();
    }

    public void OnImageAdded(Guid guid)
    {
        LazyLoadCanvas();
        if ( guid == activatingGuid)
        {
            canvas.enabled = true;
            Debug.Log($"******************** ADDDED {guid}");
        }
    }

    public void OnImageUpdated(Guid guid)
    {

    }

    public void OnImageRemoved(Guid guid)
    {
        LazyLoadCanvas();
        if (guid == activatingGuid)
        {
            canvas.enabled = false;
            Debug.Log($"******************** REMOVED {guid}");
        }
    }

    private void OnEnable()
    {
        LazyLoadCanvas();
        imageTracking.OnImageAdded += OnImageAdded;
        imageTracking.OnImageUpdated += OnImageUpdated;
        imageTracking.OnImageRemoved += OnImageRemoved;
    }

    private void OnDisable()
    {
        imageTracking.OnImageAdded -= OnImageAdded;
        imageTracking.OnImageUpdated -= OnImageUpdated;
        imageTracking.OnImageRemoved -= OnImageRemoved;
    }


}
