using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARSubsystems;

public class TrackedAddition : MonoBehaviour
{
    public Guid activatingGuid;

    public ImageTrackingObjectManager imageTracking;

    [FormerlySerializedAs("XRReferenceImage")]
    public XRReferenceImage trackingImage;
    
    [SerializeField]
    private string Guid;

    private Canvas canvas;

    [SerializeField]
    private string prettyGUID;

    [SerializeField]
    private string imageName;
    
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
        imageName = trackingImage.name;

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
