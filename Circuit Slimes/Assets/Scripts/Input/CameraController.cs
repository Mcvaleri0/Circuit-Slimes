using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lean.Touch;

public class CameraController : MonoBehaviour
{
    class CameraState
    {
        public float pitch;
        public float yaw;
        public float roll;
        public float x;
        public float y;
        public float z;
        public float zoom;

        public CameraState() { }

        public CameraState(Vector3 p, float z, Vector3 r)
        {
            pitch = r.x;
            yaw = r.y;
            roll = r.z;

            x = p.x;
            y = p.y;
            z = p.z;

            zoom = z;
        }

        private float GetZoomDistance()
        {
            //plane
            var n = Vector3.up;
            var p = new Vector3(0, 1, 0);

            //ray
            var o = new Vector3(x, y, z);
            var d = Quaternion.Euler(new Vector3(pitch, yaw, roll)) * Vector3.forward;
            d.Normalize();
            var t = (Vector3.Dot(p - o, n) / Vector3.Dot(n, d));

            return t;
        }

        public void SetFromTransform(Transform t)
        {
            pitch = t.eulerAngles.x;
            yaw = t.eulerAngles.y;
            roll = t.eulerAngles.z;
            x = t.position.x;
            y = t.position.y;
            z = t.position.z;

            zoom = GetZoomDistance();
        }

        public void Translate(Vector3 translation)
        {
            x += translation.x;
            z += translation.z;
        }

        public void Zoom(float zoomAmmount)
        {
            zoom += zoomAmmount;
        }

        public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct, float zoommingLerpPct)
        {
            yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
            pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
            roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);

            x = Mathf.Lerp(x, target.x, positionLerpPct);
            y = Mathf.Lerp(y, target.y, positionLerpPct);
            z = Mathf.Lerp(z, target.z, positionLerpPct);

            zoom = Mathf.Lerp(zoom, target.zoom, zoommingLerpPct);
        }

        public void UpdateTransform(Transform t)
        {
            //rotation
            t.eulerAngles = new Vector3(pitch, yaw, roll);

            //panning
            var pos = new Vector3(x, 1, z);

            //zoom
            var d = Quaternion.Euler(new Vector3(pitch, yaw, roll)) * Vector3.forward;
            pos += zoom * - d.normalized;

            //update pos
            t.position = pos;
        }

        public void Copy(CameraState cam)
        {
            pitch = cam.pitch;
            yaw = cam.yaw;
            roll = cam.roll;
            x = cam.x;
            y = cam.y;
            z = cam.z;

            zoom = cam.zoom;
        }
    }

    [System.Serializable]
    public class CameraLimits{
        public float MaxZoom;
        public float MinZoom;
        public float Horizontal;
        public float Vertical;
    }

    //Current and target states
    CameraState m_TargetCameraState = new CameraState();
    CameraState m_InterpolatingCameraState = new CameraState();

    //state used for reset
    CameraState StartState;

    //camera move and rotate speed
    public float positionLerpTime = 0.2f;
    public float rotationLerpTime = 0.4f;
    public float zoommingLerpTime = 0.5f;

    //filter the number of fingers that need to be used (for camera controlls -> 2)
    public Lean.Touch.LeanFingerFilter Use = new Lean.Touch.LeanFingerFilter(Lean.Touch.LeanFingerFilter.FilterType.AllFingers, true, 2, 0, null);

    //get touch point in world coords
    public Lean.Touch.LeanScreenDepth ScreenDepth = new Lean.Touch.LeanScreenDepth(Lean.Touch.LeanScreenDepth.ConversionType.FixedDistance);

    //Speed Multipliers
    public float PanSensitivity = -1.0f;
    public float ZoomSensitivity = 4.0f;

    //Limitters
    public CameraLimits CamLimtits = new CameraLimits();

    //Input
    InputController InputController;


    #region === CAMERA CONTROLS ===

    public void ResetCamera()
    {
        m_TargetCameraState.Copy(this.StartState);
    }

    public void SetStartState(Vector3 pos, float zoom, Vector3 rot) {

        this.SetTargetState(pos, zoom, rot);
            
        StartState = new CameraState();

        this.StartState.Copy(m_TargetCameraState);
  
    }

    public void SetTargetState(Vector3 pos, float zoom, Vector3 rot) 
    {
        this.SetPosition(pos);

        this.SetZoom(zoom);

        this.SetRotation(rot);
    }

    public void EditMode()
    {
        this.ResetCamera();

        var topRot = new Vector3(90, this.StartState.yaw, this.StartState.roll);

        this.SetRotation(topRot);
    }

    public void PlayMode()
    {
        this.ResetCamera();
    }

    // Setters

    public void SetPosition(Vector3 pos)
    {
        m_TargetCameraState.x = pos.x;
        m_TargetCameraState.y = pos.y;
        m_TargetCameraState.z = pos.z;
    }

    public void SetZoom(float zoom)
    {
        m_TargetCameraState.zoom = zoom;
    }

    public void SetRotation(Vector3 rot)
    {
        m_TargetCameraState.pitch = rot.x;
        m_TargetCameraState.yaw   = rot.y;
        m_TargetCameraState.roll  = rot.z;
    }

    #endregion


    #region === TRANSLATION ===

    protected Vector3 GetInputTranslation()
    {
        // Get the fingers we want to use
        var fingers = Use.GetFingers();

        //ignore panning with middlebutton if over ui
        if (fingers.Count == 0 && LeanTouch.PointOverGui(Input.mousePosition))
        {
            return new Vector3();
        }

        // Get current point and previous point 
        var lastScreenPoint = Lean.Touch.LeanGesture.GetLastScreenCenter(fingers);
        var screenPoint = Lean.Touch.LeanGesture.GetScreenCenter(fingers);

        //if using middle mouse button for panning
        if (Input.GetMouseButton(2))
        {
            lastScreenPoint = InputController.LastMousePosition;
            screenPoint = InputController.MousePosition;
        }

        // Get the world delta of them after conversion
        var worldDelta = ScreenDepth.ConvertDelta(lastScreenPoint, screenPoint, gameObject);

        // Pan the camera based on the world delta
        var movement = worldDelta * PanSensitivity;

        return movement;
    }

    protected Vector3 ClampTranslation(Vector3 translation)
    {
        var xx = 0.0f;
        var zz = 0.0f;

        //x
        var limit = this.CamLimtits.Vertical;
        var current = m_TargetCameraState.x;
        var target = current + translation.x;
        var anchor = this.StartState.x;

        if (translation.x > 0)
        {
            xx = (target > anchor + limit / 2) ? 0 : translation.x;
        }
        else
        {
            xx = (target < anchor - limit/2) ? 0 : translation.x;
        }

        //z
        limit = this.CamLimtits.Horizontal;
        current = m_TargetCameraState.z;
        target = current + translation.z;
        anchor = this.StartState.z;

        if (translation.z > 0)
        {
            zz = (target > anchor + limit/2) ? 0 : translation.z;
        }
        else
        {
            zz = (target < anchor - limit/2) ? 0 : translation.z;
        }

        return new Vector3(xx, translation.y, zz);
    }

    #endregion


    #region === ZOOM ===

    protected float GetInputZoom()
    {

        // Get the fingers we want to use
        var fingers = Use.GetFingers();

        //ignore zoom with scrollwheel if over ui
        if (fingers.Count == 0 && LeanTouch.PointOverGui(Input.mousePosition))
        {
            return 0;
        }
        
        // Get current point and previous point 
        var lastScreenPoint = Lean.Touch.LeanGesture.GetLastScreenCenter(fingers);
        var screenPoint = Lean.Touch.LeanGesture.GetScreenCenter(fingers);

        var pinchScale = Lean.Touch.LeanGesture.GetPinchScale(fingers, -ZoomSensitivity * 0.1f);

        return (1 - Mathf.Clamp(pinchScale, 0, 2)) * ZoomSensitivity;
    }

    protected float ClampZoom(float zoomAmmount) {

        var current = m_TargetCameraState.zoom;
        var target = current + zoomAmmount;
        var limit = 0.0f;

        if (zoomAmmount > 0)
        {
            limit = this.CamLimtits.MaxZoom;
            return (target > limit) ? 0 : zoomAmmount;
        }
        else {
            limit = this.CamLimtits.MinZoom;
            return (target < limit) ? 0 : zoomAmmount;
        }
    }

    #endregion


    #region === UNITY METHODS ===

    void OnEnable()
    {
        m_TargetCameraState.SetFromTransform(transform);
        m_InterpolatingCameraState.SetFromTransform(transform);

        this.SetStartState(transform.position, this.CamLimtits.MaxZoom, transform.eulerAngles);
    }

    private void Start()
    {
        this.InputController = GameObject.Find("InputController").GetComponent<InputController>();
    }

    void LateUpdate()
    {
        // Translation
        var translation = this.ClampTranslation(this.GetInputTranslation());
        m_TargetCameraState.Translate(translation);

        // Zoom
        var zoom = this.ClampZoom(this.GetInputZoom());
        m_TargetCameraState.Zoom(zoom);

        //Reset Camera
        if (Input.GetKeyDown(KeyCode.R)) {
            this.ResetCamera();
        }

        //test
        if (Input.GetKey(KeyCode.L))
        {
            this.EditMode();
        }
        if (Input.GetKey(KeyCode.K))
        {
            this.PlayMode();
        }

        // Framerate-independent interpolation
        // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
        var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
        var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
        var zoommingLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / zoommingLerpTime) * Time.deltaTime);
        m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct, zoommingLerpPct);
        m_InterpolatingCameraState.UpdateTransform(transform);

        // Update distance used for depth in translation
        ScreenDepth.Distance = transform.position.y;
    }

    private void OnDrawGizmos()
    {
        /*
        //LookAt Position
        var ini = new Vector3(this.m_InterpolatingCameraState.x, 1, this.m_InterpolatingCameraState.z);
        var end = new Vector3(this.m_InterpolatingCameraState.x, 3, this.m_InterpolatingCameraState.z);
        Debug.DrawLine(ini, end, Color.blue);

        //Camera Ray
        var n = Vector3.up;
        var p = new Vector3(0, 1, 0);

        var o = transform.position;
        var d = transform.forward;
        var t = (Vector3.Dot(p - o, n) / Vector3.Dot(n, d));

        var ini2 = o;
        var end2 = o + t * d;
        Debug.DrawLine(ini2, end2, Color.yellow);
        */
    }

    #endregion
}
