using UnityEngine;

internal interface ISelectionResponse
{
    // What the selected object does when it isn't selected anymore
    void OnDeselect(Transform selection);

    // What the newly selected object does because it was selected
    void OnSelect(Transform selection);
}
