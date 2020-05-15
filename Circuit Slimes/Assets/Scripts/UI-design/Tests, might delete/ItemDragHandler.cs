﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ItemDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Vector3 StartPosition;

    public void OnDrag(PointerEventData eventData)
    {
         transform.position = Input.mousePosition;
            
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = StartPosition;

    }

}
