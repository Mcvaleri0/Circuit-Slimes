using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lean.Touch;

public class InputController : MonoBehaviour
{

    public Vector2 LastMousePosition { get; private set; } = default(Vector2);

    public Vector2 MousePosition { get; private set; }

    //Single Finger Touch Filter
    public LeanFingerFilter Use = new LeanFingerFilter(LeanFingerFilter.FilterType.AllFingers, true, 2, 0, null);


    private void Start()
    {
        //completely separate mouse and touch input
        Input.simulateMouseWithTouches = false;
    }

    // Update is called once per frame
    void Update()
    {
        LastMousePosition = MousePosition;
        MousePosition = Input.mousePosition;
    }
}
