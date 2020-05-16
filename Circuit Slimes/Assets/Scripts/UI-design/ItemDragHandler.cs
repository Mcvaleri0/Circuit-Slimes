using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ItemDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Vector3 StartPosition;
    public ListPositionCtrl selectionWheel;
    public ListBox Slime;

    public void OnDrag(PointerEventData eventData)
    {
        // Checks if it is the centered slime, and enables drag/drop then
        int centeredId = selectionWheel.GetCenteredContentID();
        int slimeId = Slime.listBoxID;
       // Debug.Log(slimeId == centeredId);
        if (slimeId == centeredId){
            transform.position = Input.mousePosition;

        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localPosition = Vector3.zero;

    }

}
