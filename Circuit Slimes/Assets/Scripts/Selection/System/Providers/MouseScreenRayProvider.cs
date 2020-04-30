using UnityEngine;
using Lean.Touch;

public class MouseScreenRayProvider : MonoBehaviour, IRayProvider
{
    public Ray CreateRay()
    {
        var pos = Input.mousePosition;

        if (LeanTouch.Fingers.Count > 0 &&
        !LeanTouch.Fingers[0].StartedOverGui)
        {
            pos = LeanTouch.Fingers[0].ScreenPosition;
        }

        return Camera.main.ScreenPointToRay(pos);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(CreateRay());
    }
}