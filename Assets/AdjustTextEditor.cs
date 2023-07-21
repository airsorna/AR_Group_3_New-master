using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AdjustTextEditor : MonoBehaviour
{

    private void OnValidate()
    {
        var texts = GetComponentsInChildren<TMP_Text>();
        foreach (var text in texts)
        {
            var parent = text.transform.parent;
            var grandParent = parent.transform.parent;

            var grandParentName = grandParent.name;
            text.text = grandParentName;
        }
    }
}
