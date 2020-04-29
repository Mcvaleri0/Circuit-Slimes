﻿using UnityEngine;
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

            //check if tag is correct
            if (tr.CompareTag(this.Tag) ) return tr;
        }

        return null;
    }

}