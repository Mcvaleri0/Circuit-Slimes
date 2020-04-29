using UnityEngine;

public interface ITransformSelector
{
    Transform Check(Ray ray);

}