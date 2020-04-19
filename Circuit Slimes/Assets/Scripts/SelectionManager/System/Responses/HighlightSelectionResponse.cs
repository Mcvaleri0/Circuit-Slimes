using UnityEngine;

internal class HighlightSelectionResponse: MonoBehaviour, ISelectionResponse
{
    public Material HighlightMaterial;
    public Material DefaultMaterial;

    public void OnSelect(Transform selection)
    {
        var selectionRenderer = selection.GetComponent<Renderer>();

        if (selectionRenderer != null)
        {
            selectionRenderer.material = this.HighlightMaterial;
        }
    }


    public void OnDeselect(Transform selection)
    {
        var selectionRenderer = selection.GetComponent<Renderer>();

        if (selectionRenderer != null)
        {
            selectionRenderer.material = this.DefaultMaterial;
        }
    }
}