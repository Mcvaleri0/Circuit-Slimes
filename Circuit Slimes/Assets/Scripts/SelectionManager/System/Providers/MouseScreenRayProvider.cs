using UnityEngine;

public class MouseScreenRayProvider : MonoBehaviour, IRayProvider
{
    public Ray CreateRay()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(CreateRay());
    }
}