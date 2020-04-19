using UnityEngine;

public class RayCastBasedTagSelector : MonoBehaviour, ISelector
{
    public string Tag = "Selectable";

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
