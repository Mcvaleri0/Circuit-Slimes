using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquachStretchIdle : MonoBehaviour
{
    private float t = 0.0f;
    private Vector3 BaseScale;
    private Vector3 BaseRotation;

    public float DeformationSpeed = 0.1f;

    public Vector3 ScaleDeformation;
    public Vector3 RotationDeformation;

    private void Start()
    {
        this.BaseScale = this.transform.localScale;
        this.BaseRotation = this.transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        this.t += Time.deltaTime * DeformationSpeed;

        var scaleinc = ScaleDeformation * Mathf.Sin(this.t);
        this.transform.localScale = this.BaseScale + scaleinc;

        var rotinc = RotationDeformation * Mathf.Sin(this.t);
        this.transform.localEulerAngles = this.BaseRotation + rotinc;
    }
}
