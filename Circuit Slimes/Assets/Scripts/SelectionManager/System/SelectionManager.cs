using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{

    private IRayProvider       RayProvider;
    private ISelector          Selector;
    private ISelectionResponse SelectionResponse;

    public Transform CurrentSelection { get; private set; }

    public Vector2Int BoardCoords { get; private set; }

    private void Start()
    {
        this.RayProvider       = this.GetComponent<IRayProvider>();
        this.Selector          = this.GetComponent<ISelector>();
        this.SelectionResponse = this.GetComponent<ISelectionResponse>();
    }

    private void Update()
    {
        //Selection Determination
        var selection = GetSelection();

        //Deselection Response
        if (selection != this.CurrentSelection)
        {
            if(this.CurrentSelection != null)
            {
                this.SelectionResponse.OnDeselect(this.CurrentSelection);
            }

            this.CurrentSelection = selection;

            //Selection Response
            if (this.CurrentSelection != null)
            {
                this.SelectionResponse.OnSelect(this.CurrentSelection);
            }
        }

        //Debug.Log(CurrentSelection);
        //Debug.Log(BoardCoords);
    }

    private Transform GetSelection()
    {
        return this.Selector.Check(this.RayProvider.CreateRay());
    }
}