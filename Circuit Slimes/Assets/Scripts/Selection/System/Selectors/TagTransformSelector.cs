using UnityEngine;
using Puzzle;
using System.Collections.Generic;

public class TagTransformSelector : MonoBehaviour, ITransformSelector
{

    public string Tag = "Selectable";

    private SelectionManager Manager;

    private List<Transform> WhiteList;

    public void Start()
    {
        this.Manager = this.GetComponent<SelectionManager>();
    }

    private bool IgnoreSelection(Ray ray)
    {
        //ignore selecion when doing a gesture 
        if (Lean.Touch.LeanTouch.Fingers.Count >= 2)
        {
            return true;
        }
        //or clicking on ui
        if (Lean.Touch.LeanTouch.Fingers.Count == 1 && Lean.Touch.LeanTouch.Fingers[0].StartedOverGui)
        {
            return true;
        }
        //or just hovering ui
        var screenpos = Camera.main.WorldToScreenPoint(ray.origin);
        if (Lean.Touch.LeanTouch.PointOverGui(screenpos))
        {
            return true;
        }
        return false;
    }

    public Transform Check(Ray ray)
    {
        this.WhiteList = Manager.WhiteList;

        if (Physics.Raycast(ray, out var hit))
        {
            var tr = hit.transform;

            //see if any object was hit
            if (tr == null) return null;

            //check if filter allows it
            if (WhiteList != null && (WhiteList.Count == 0 || !WhiteList.Contains(tr))) return null;

            //check if tag is correct and if we should not ignore
            if (tr.CompareTag(this.Tag) && !this.IgnoreSelection(ray)) return tr;
        }

        return null;
    }

}
