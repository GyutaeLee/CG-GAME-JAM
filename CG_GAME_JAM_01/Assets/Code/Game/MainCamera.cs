using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//?? 규태 : 스크립트 이름 변경하기
public class MainCamera : MonoBehaviour
{
    public GameObject cameraObject;
    private Camera m_camera;
    private Transform m_cameraOriginTransform;

    public float cameraMoveSpeed;
    public float cameraRotateSpeed;

    private void Start()
    {
        InitMainCamera();
    }

    private void InitMainCamera()
    {
        this.cameraObject = GameObject.Find("Main Camera");
        this.m_camera = this.cameraObject.GetComponent<Camera>();
        this.m_cameraOriginTransform = this.cameraObject.transform;

        this.m_cameraOriginTransform = this.cameraObject.transform;
        this.cameraMoveSpeed = 1.0f;
        this.cameraRotateSpeed = 0.1f;
    }
}
