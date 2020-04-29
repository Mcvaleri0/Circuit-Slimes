using UnityEngine;

internal interface IBoardSelectionResponse
{
    // Response for the selected space(s) on the board
    void UpdateSelection(Vector2Int boardCoords, bool boardHover, Transform selection);

}
