using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiCameraRotateController : Joystick
{
    public MainCamera _MainCamera;

    public override void PointerDownFunction()
    {
        _MainCamera.RotateCameraCenteredOnMap(JoystickHorizontalValue, JoystickVerticalValue);
    }
}
