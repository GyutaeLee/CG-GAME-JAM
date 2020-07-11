using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiCameraRotateController : Joystick
{
    public MainCamera _ThirdPersonViewCamera;
    public GameObject CenterStandardObject;

    public override void PointerDownFunction()
    {
        base.PointerDownFunction();

        RotateCameraCenteredOnObject(JoystickHorizontalValue, JoystickVerticalValue);
    }
    public void RotateCameraCenteredOnObject(float horizontalValue, float verticalValue)
    {
        Transform camTransform = _ThirdPersonViewCamera.CameraObject.transform;
        Vector3 rotation = Vector3.zero;

        rotation.x = verticalValue;
        rotation.y = -horizontalValue;

        camTransform.RotateAround(CenterStandardObject.transform.position, rotation, _ThirdPersonViewCamera.cameraRotateSpeed);
    }
}
