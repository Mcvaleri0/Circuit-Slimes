using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickOutlineSelectionResponse : MonoBehaviour, IPieceSelectionResponse
{
    public void OnSelect(Transform selection)
    {
        var outline = selection.GetComponent<QuickOutline>();

        if(outline == null)
        {
            var outln = selection.gameObject.AddComponent<QuickOutline>();

            outln.OutlineMode  = QuickOutline.Mode.OutlineAll;
            outln.OutlineColor = Color.white;
            
            outln.OutlineWidth = 5;
        }
        else
        {
            outline.OutlineWidth = 5;
        }
    }

    public void OnDeselect(Transform selection)
    {
        var outline = selection.GetComponent<QuickOutline>();

        if (outline != null)
        {
            outline.OutlineWidth = 0;
        }
    }
}
