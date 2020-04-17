using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LedLight : MonoBehaviour
{

    public Material OnMaterial;
    public Material OffMaterial;

    private MeshRenderer LedRenderer;

    void Start()
    {
        LedRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    public void TurnOn()
    {
        this.LedRenderer.material = OnMaterial;
    }

    public void TurnOff()
    {
        this.LedRenderer.material = OffMaterial;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            this.TurnOff();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            this.TurnOn();
        }
    }
}