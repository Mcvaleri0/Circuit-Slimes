using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{

    public Vector2 LastMousePosition { get; private set; } = default(Vector2);

    public Vector2 MousePosition { get; private set; }


    // Update is called once per frame
    void Update()
    {
        LastMousePosition = MousePosition;
        MousePosition = Input.mousePosition;
    }
}
