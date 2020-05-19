using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineSelectionResponse : MonoBehaviour, IPieceSelectionResponse
{
    public void OnSelect(Transform selection)
    {
        var outline = selection.GetComponent<Outline>();

        if(outline == null)
        {
            outline = selection.gameObject.AddComponent<Outline>();
        }

        outline.Select();
    }

    public void OnDeselect(Transform selection)
    {
        var outline = selection.GetComponent<Outline>();

        if (outline != null)
        {
            outline.DeSelect();
        }
    }
}
