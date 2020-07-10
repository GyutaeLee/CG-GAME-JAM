using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public GameObject Cameraobject;
    public Camera _Camera;

    public void RotateCameraCenteredOnMap(float horizontalValue, float verticalValue)
    {
        //?? 규태 : 로테이션 만들기
        Debug.Log("GYUT TEST - ROTATE CAM : " + horizontalValue + " " + verticalValue);
    }
}
