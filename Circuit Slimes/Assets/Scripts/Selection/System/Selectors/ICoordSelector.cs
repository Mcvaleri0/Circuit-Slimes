using UnityEngine;

public interface ICoordSelector: ITransformSelector
{
    Vector2Int GetCoords();

    Vector2Int GetOffset();

    bool GetHover();
   
}
