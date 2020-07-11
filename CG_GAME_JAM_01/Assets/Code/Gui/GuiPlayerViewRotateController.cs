using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiPlayerViewRotateController : Joystick
{
    public Player _Player;
    public MainCamera _FirstPersonViewCamera;

    public override void PointerDownFunction()
    {
        base.PointerDownFunction();

        RotateCameraOnPlayer(JoystickHorizontalValue, JoystickVerticalValue);
    }
    public void RotateCameraOnPlayer(float horizontalValue, float verticalValue)
    {
        Vector3 rotation = _FirstPersonViewCamera.CameraObject.transform.eulerAngles;
        float horizontalSpeed = -verticalValue * _FirstPersonViewCamera.cameraMoveSpeed;
        float verticalSpeed = horizontalValue * _FirstPersonViewCamera.cameraMoveSpeed;

        rotation.x += horizontalSpeed;
        rotation.y += verticalSpeed;

        Quaternion quaternion = Quaternion.Euler(rotation);
        quaternion.z = 0;

        quaternion.x = Mathf.Clamp(quaternion.x, -0.25f, 0.25f);
        quaternion.y = Mathf.Clamp(quaternion.y, -0.15f, 0.15f);

        _FirstPersonViewCamera.CameraObject.transform.rotation = Quaternion.Slerp(_FirstPersonViewCamera.CameraObject.transform.rotation, quaternion, 1.5f);
    }
}
