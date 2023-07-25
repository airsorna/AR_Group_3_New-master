using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTrackingObjectManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Image manager on the AR Session Origin")]
    ARTrackedImageManager m_ImageManager;

    [SerializeField]
    private Camera arCamera;

    /// <summary>
    /// Get the <c>ARTrackedImageManager</c>
    /// </summary>
    public ARTrackedImageManager ImageManager
    {
        get => m_ImageManager;
        set => m_ImageManager = value;
    }

    [SerializeField]
    [Tooltip("Reference Image Library")]
    XRReferenceImageLibrary m_ImageLibrary;

    /// <summary>
    /// Get the <c>XRReferenceImageLibrary</c>
    /// </summary>
    public XRReferenceImageLibrary ImageLibrary
    {
        get => m_ImageLibrary;
        set => m_ImageLibrary = value;
    }

    [SerializeField]
    private List<GameObject> prefabList;

    [SerializeField]
    [Tooltip("Prefab for tracked 1 image")]
    GameObject m_OnePrefab;

    [SerializeField]
    private RectTransform parentInstantiation;

    /// <summary>
    /// Get the one prefab
    /// </summary>
    public GameObject onePrefab
    {
        get => m_OnePrefab;
        set => m_OnePrefab = value;
    }

    GameObject m_SpawnedOnePrefab;
    
    /// <summary>
    /// get the spawned one prefab
    /// </summary>
    public GameObject spawnedOnePrefab
    {
        get => m_SpawnedOnePrefab;
        set => m_SpawnedOnePrefab = value;
    }

    [SerializeField]
    [Tooltip("Prefab for tracked 2 image")]
    GameObject m_TwoPrefab;

    /// <summary>
    /// get the two prefab
    /// </summary>
    public GameObject twoPrefab
    {
        get => m_TwoPrefab;
        set => m_TwoPrefab = value;
    }

    GameObject m_SpawnedTwoPrefab;
    
    /// <summary>
    /// get the spawned two prefab
    /// </summary>
    public GameObject spawnedTwoPrefab
    {
        get => m_SpawnedTwoPrefab;
        set => m_SpawnedTwoPrefab = value;
    }

    int m_NumberOfTrackedImages;
    
    NumberManager m_OneNumberManager;
    NumberManager m_TwoNumberManager;

    static Guid s_FirstImageGUID;
    static Guid s_SecondImageGUID;

    private List<Guid> imageGuids = new List<Guid>();

    private Dictionary<Guid, GameObject> prefabImagePairs = new Dictionary<Guid, GameObject>();

    private Dictionary<Guid, GameObject> clones = new Dictionary<Guid, GameObject>();


    private void Awake()
    {
        for (var i = 0; i < m_ImageLibrary.count; i++)
        {
            imageGuids.Add(m_ImageLibrary[i].guid);
            prefabImagePairs.Add(imageGuids[i], prefabList[i]);
        }
    }
    void OnEnable()
    {
        s_FirstImageGUID = m_ImageLibrary[0].guid;
        s_SecondImageGUID = m_ImageLibrary[1].guid;

        
        
        m_ImageManager.trackedImagesChanged += ImageManagerOnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_ImageManager.trackedImagesChanged -= ImageManagerOnTrackedImagesChanged;
    }

    void ImageManagerOnTrackedImagesChanged(ARTrackedImagesChangedEventArgs obj)
    {
        // added, spawn prefab
        ///////////////////////////////////////////////////////////////////////
        foreach(ARTrackedImage image in obj.added)
        {
            var imageGuid = image.referenceImage.guid;
            Debug.Log(image.referenceImage.name);
            // List for AR
            if ( imageGuids.Contains(imageGuid) )
            {
                var prefab = prefabImagePairs[imageGuid];

                var canvas = prefab.GetComponent<Canvas>();
                var clone = Instantiate(prefab);
                if ( clones.ContainsKey(imageGuid) )
                {
                    clones[imageGuid] = prefab;
                }
                else
                {
                    clones.Add(imageGuid, prefab);
                }
                Debug.LogWarning($" Added [***KEYS****]: {image.referenceImage.name}");
            }
           
        }
        
        // updated, set prefab position and rotation
        ////////////////////////////////////////////////////////////////////////
        foreach(ARTrackedImage image in obj.updated)
        {
            Debug.LogWarning($" Updating [***KEYS****]: {image.referenceImage.name}");
            Debug.LogWarning($" Updating [***KEYS****]: {image.referenceImage.guid}");
              
            var _keys = clones.Keys;

            StringBuilder _builder = new StringBuilder();
            _builder.AppendLine("Updating [***KEYS****]:");
            foreach (var key in _keys)
            {
                _builder.AppendLine(key.ToString());
            }
            Debug.LogWarning(_builder);

            if (clones.ContainsKey(image.referenceImage.guid))
            {
                var clone = clones[image.referenceImage.guid];
                if (clone != null)
                {
                    Destroy(clone.gameObject);
                    clones.Remove(image.referenceImage.guid);
                }
            }
        }

        
        // removed, destroy spawned instance
        //////////////////////////////////////////////////////////////////////
        foreach(ARTrackedImage image in obj.removed)
        {
            Debug.LogWarning($" REMOVED [***KEYS****]: {image.referenceImage.name}");
            var _keys = clones.Keys;
            Debug.LogWarning($"REMOVED [***KEYS****]: {_keys}");
            if ( clones.ContainsKey(image.referenceImage.guid))
            {
                var clone = clones[image.referenceImage.guid];
                if ( clone != null )
                {
                    Destroy(clone.gameObject);
                }
            }
            
            if (image.referenceImage.guid == s_FirstImageGUID)
            {
                Destroy(m_SpawnedOnePrefab);
            }
            else if (image.referenceImage.guid == s_SecondImageGUID)
            {
                Destroy(m_SpawnedTwoPrefab);
            }
        }

        // Check against the clones dictionary just in case
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("After everything [***KEYS****]:");
        var keys = clones.Keys;
        foreach (var key in keys)
        {
            builder.AppendLine(key.ToString());
        }
        Debug.LogWarning(builder);
        foreach (var kvp in clones)
        {
            Guid key = kvp.Key;
            GameObject value = kvp.Value;
            bool isInUpdated = false;
            foreach (var item in obj.updated)
            {
                if (key == item.referenceImage.guid)
                {
                    isInUpdated = false;
                    break;
                }
            }
            if (!isInUpdated)
            {
                clones.Remove(key);
                Destroy(value);
            }
        }
    }

    public int NumberOfTrackedImages()
    {
        m_NumberOfTrackedImages = 0;
        foreach (ARTrackedImage image in m_ImageManager.trackables)
        {
            if (image.trackingState == TrackingState.Tracking)
            {
                m_NumberOfTrackedImages++;
            }
        }
        return m_NumberOfTrackedImages;
    }
}
