using UnityEngine;

public class RayCastBasedTagSelector : MonoBehaviour, ISelector
{
    public string Tag = "Selectable";

    [Range(1f, 10f)]
    public float MaxDistance = float.PositiveInfinity;

    private GameObject Player;

    public void Start()
    {
        this.Player = GameObject.Find("PlayerCharacter");
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
