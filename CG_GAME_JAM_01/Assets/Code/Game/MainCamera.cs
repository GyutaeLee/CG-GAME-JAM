using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public GameObject CameraObject;
    public Camera _Camera;

    public Transform cameraOriginTransform;
    public float cameraMoveSpeed = 1.0f;
    public float cameraRotateSpeed = 0.1f;

    private void Start()
    {
        InitializeMainCamera();
    }

    private void InitializeMainCamera()
    {
        cameraOriginTransform = CameraObject.transform;
    }
}
