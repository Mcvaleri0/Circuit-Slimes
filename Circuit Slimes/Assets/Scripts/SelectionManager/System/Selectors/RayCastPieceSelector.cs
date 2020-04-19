using UnityEngine;
using Puzzle;

public class RayCastPieceSelector : MonoBehaviour, ISelector
{
    private SelectionManager Manager;

    public void Start()
    {
        this.Manager = this.GetComponent<SelectionManager>();
    }

    public Transform Check(Ray ray)
    {

        foreach (var piece in Manager.PuzzleController.Puzzle.Pieces) {

           

        }

        if (Physics.Raycast(ray, out var hit))
        {
            var tr = hit.transform;

       

            if (tr != null)
            {
                return tr;
            }
        }

        return null;
    }

}
