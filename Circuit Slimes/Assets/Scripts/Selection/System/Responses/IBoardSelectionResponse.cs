using UnityEngine;

internal interface IBoardSelectionResponse
{
    // Response for the selected space(s) on the board
    void Update(Vector2Int BoardCoords, bool BoardHover);

}
