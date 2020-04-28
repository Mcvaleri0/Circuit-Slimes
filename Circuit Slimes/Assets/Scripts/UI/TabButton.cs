using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(image))]

public class TabButton : MonoBehaviour
{
    public TabGroup tabGroup;

    public Image background;

    void Start()
    {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    void Update()
    {

    }
}
