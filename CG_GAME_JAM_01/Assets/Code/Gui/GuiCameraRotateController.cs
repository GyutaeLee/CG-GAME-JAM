using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiCameraRotateController : Joystick
{
    public MainCamera thirdPersonViewCamera;
    public GameObject centerStandardObject;

    public override void PointerDownFunction()
    {
        base.PointerDownFunction();

        RotateCameraCenteredOnObject(JoystickHorizontalValue, JoystickVerticalValue);
    }
    public void RotateCameraCenteredOnObject(float horizontalValue, float verticalValue)
    {
        Transform camTransform = this.thirdPersonViewCamera.cameraObject.transform;
        Vector3 rotation = Vector3.zero;

        rotation.x = verticalValue;
        rotation.y = -horizontalValue;

        camTransform.RotateAround(this.centerStandardObject.transform.position, rotation, this.thirdPersonViewCamera.cameraRotateSpeed);
    }
}
