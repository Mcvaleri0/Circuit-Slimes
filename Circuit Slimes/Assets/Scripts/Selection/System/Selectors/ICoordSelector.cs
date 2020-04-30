using UnityEngine;

public interface ICoordSelector
{
    Vector2Int GetCoords(Ray ray);

    bool GetHover();
   
}
