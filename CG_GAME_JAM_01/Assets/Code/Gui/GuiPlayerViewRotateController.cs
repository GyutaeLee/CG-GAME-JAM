using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiPlayerViewRotateController : Joystick
{
    public Player player;
    public MainCamera firstPersonViewCamera;

    public override void PointerDownFunction()
    {
        base.PointerDownFunction();

        RotateCameraOnPlayer(base.JoystickHorizontalValue, base.JoystickVerticalValue);
    }
    public void RotateCameraOnPlayer(float horizontalValue, float verticalValue)
    {
        Vector3 rotation = this.firstPersonViewCamera.cameraObject.transform.eulerAngles;
        float horizontalSpeed = -verticalValue * this.firstPersonViewCamera.cameraMoveSpeed;
        float verticalSpeed = horizontalValue * this.firstPersonViewCamera.cameraMoveSpeed;

        rotation.x += horizontalSpeed;
        rotation.y += verticalSpeed;

        Quaternion quaternion = Quaternion.Euler(rotation);
        quaternion.z = 0;

        quaternion.x = Mathf.Clamp(quaternion.x, -0.25f, 0.25f);
        quaternion.y = Mathf.Clamp(quaternion.y, -0.15f, 0.15f);

        this.firstPersonViewCamera.cameraObject.transform.rotation = Quaternion.Slerp(this.firstPersonViewCamera.cameraObject.transform.rotation, quaternion, 1.5f);
    }
}
