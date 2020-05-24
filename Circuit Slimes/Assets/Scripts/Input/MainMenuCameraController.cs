using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lean.Touch;

public class MainMenuCameraController : MonoBehaviour
{
    class CameraState
    {
        public float pitch;
        public float yaw;
        public float roll;
        public float x;
        public float y;
        public float z;

        public CameraState() { }

        public CameraState(Vector3 p, Vector3 r)
        {
            pitch = r.x;
            yaw = r.y;
            roll = r.z;

            x = p.x;
            y = p.y;
            z = p.z;
        }

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
            y += translation.y;
            z += translation.z;
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
            //update rotation
            t.eulerAngles = new Vector3(pitch, yaw, roll);

            //update pos
            t.position = new Vector3(x, y, z); ;
        }

    }

    //Current and target states
    CameraState m_TargetCameraState = new CameraState();
    CameraState m_InterpolatingCameraState = new CameraState();

    //camera move and rotate speed
    public float positionLerpTime = 0.2f;
    public float rotationLerpTime = 0.4f;


    public List<Transform> CameraPositions = new List<Transform>();



    #region === CAMERA GO TO MATHODS ===


    public void GoToMenu()
    {
        m_TargetCameraState.SetFromTransform(CameraPositions[0]);
    }

    public void GoToSplashScreen()
    {
        m_TargetCameraState.SetFromTransform(CameraPositions[1]);
    }

    public void GoToInfo()
    {
        m_TargetCameraState.SetFromTransform(CameraPositions[2]);
    }

    public void GoToPlay()
    {
        m_TargetCameraState.SetFromTransform(CameraPositions[3]);
    }

    public void GoToEdit()
    {
        m_TargetCameraState.SetFromTransform(CameraPositions[4]);
    }

    #endregion



    #region === UNITY METHODS ===

    void OnEnable()
    {
        m_TargetCameraState.SetFromTransform(transform);
        m_InterpolatingCameraState.SetFromTransform(transform);
    }

    private void Start()
    {
        this.GoToMenu();
    }

    void LateUpdate()
    {
        // Framerate-independent interpolation
        // Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
        var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
        var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
        m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);
        m_InterpolatingCameraState.UpdateTransform(transform);
    }

    #endregion
}
