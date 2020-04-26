using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    class CameraState
    {
        public float yaw;
        public float pitch;
        public float roll;
        public float x;
        public float y;
        public float z;

        public void SetFromTransform(Transform t)
        {
            pitch = t.eulerAngles.x;
            yaw = t.eulerAngles.y;
            roll = t.eulerAngles.z;
            x = t.position.x;
            y = t.position.y;
            z = t.position.z;
        }

        public void Translate(Vector3 translation)
        {
            x += translation.x;
            z += translation.z;
        }

        public void Zoom(float zoomAmmount)
        {
            var forwardVector = Quaternion.Euler(new Vector3(pitch, yaw, roll)) * Vector3.forward;
            forwardVector.Normalize();

            Vector3 zoomTranslation = forwardVector * -zoomAmmount;

            x += zoomTranslation.x;
            y += zoomTranslation.y;
            z += zoomTranslation.z;

        }

        public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
        {
            yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
            pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
            roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);

            x = Mathf.Lerp(x, target.x, positionLerpPct);
            y = Mathf.Lerp(y, target.y, positionLerpPct);
            z = Mathf.Lerp(z, target.z, positionLerpPct);
        }

        public void UpdateTransform(Transform t)
        {
            t.eulerAngles = new Vector3(pitch, yaw, roll);
            t.position = new Vector3(x, y, z);
        }
    }

    CameraState m_TargetCameraState = new CameraState();
    CameraState m_InterpolatingCameraState = new CameraState();

    //camera move and rotate speed
    public float positionLerpTime = 0.2f;
    public float rotationLerpTime = 0.01f;

    //filter the number of fingers that need to be used (for camera controlls -> 2)
    public Lean.Touch.LeanFingerFilter Use = new Lean.Touch.LeanFingerFilter(Lean.Touch.LeanFingerFilter.FilterType.AllFingers, true, 2, 0, null);

    //get touch point in world coords
    public Lean.Touch.LeanScreenDepth ScreenDepth = new Lean.Touch.LeanScreenDepth(Lean.Touch.LeanScreenDepth.ConversionType.FixedDistance);

    //Speed Multipliers
    public float PanSensitivity = -1.0f;
    public float ZoomSensitivity = 2.0f;

    InputController InputController;



    Vector3 GetInputTranslation()
    {
        // Get the fingers we want to use
        var fingers = Use.GetFingers();

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

    float GetInputZoom()
    {

        // Get the fingers we want to use
        var fingers = Use.GetFingers();

        var pinchScale = Lean.Touch.LeanGesture.GetPinchScale(fingers, -ZoomSensitivity * 0.5f);

        return (1 - Mathf.Clamp(pinchScale, 0, 2)) * ZoomSensitivity;
    }



    private void Start()
    {
        this.InputController = GameObject.Find("InputController").GetComponent<InputController>();
    }

    void OnEnable()
    {
        m_TargetCameraState.SetFromTransform(transform);
        m_InterpolatingCameraState.SetFromTransform(transform);
    }

    void LateUpdate()
    {
        // Translation
        var translation = GetInputTranslation();
        m_TargetCameraState.Translate(translation);

        // Zoom
        var zoom = GetInputZoom();
        m_TargetCameraState.Zoom(zoom);

        // Framerate-independent interpolation
        // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
        var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
        var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
        m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);
        m_InterpolatingCameraState.UpdateTransform(transform);

        // Update distance used for depth in translation
        ScreenDepth.Distance = transform.position.y;
    }
}
