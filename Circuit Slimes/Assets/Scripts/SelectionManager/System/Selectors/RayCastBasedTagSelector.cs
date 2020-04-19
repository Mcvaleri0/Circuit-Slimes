using UnityEngine;
using Puzzle;

public class RayCastBasedTagSelector : MonoBehaviour, ISelector
{
    public string Tag = "Selectable";

  

    public void Start()
    {
      
    }

    public Transform Check(Ray ray)
    {
        if (Physics.Raycast(ray, out var hit))
        {
            var tr = hit.transform;

            if(tr.CompareTag(this.Tag))
            {
                //Debug.Log("Bin");
            }

            if (tr != null && tr.CompareTag(this.Tag))
            {
                return tr;
            }
        }

        return null;
    }
}
