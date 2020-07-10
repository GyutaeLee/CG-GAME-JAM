using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiMoveController : Joystick
{
    public Player _Player;

    public override void PointerDownFunction()
    {
        _Player.MovePlayerObject(JoystickHorizontalValue, JoystickVerticalValue);
    }
}
