using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineSelectionResponse : MonoBehaviour, ISelectionResponse
{
    public void OnSelect(Transform selection)
    {
        var outline = selection.GetComponent<Outline>();

        if(outline == null)
        {
            var outln = selection.gameObject.AddComponent<Outline>();

            outln.OutlineMode  = Outline.Mode.OutlineAll;
            outln.OutlineColor = Color.white;
            
            outln.OutlineWidth = 8;
        }
        else
        {
            outline.OutlineWidth = 8;
        }
    }

    public void OnDeselect(Transform selection)
    {
        var outline = selection.GetComponent<Outline>();

        if (outline != null)
        {
            outline.OutlineWidth = 0;
        }
    }
}
