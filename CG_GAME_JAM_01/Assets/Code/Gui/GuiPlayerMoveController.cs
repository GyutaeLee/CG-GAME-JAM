using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiPlayerMoveController : Joystick
{
    public Player _Player;
        
    public override void KeyboardDownFunction()
    {
        float horizontalValue = 0.0f;
        float verticalValue = 0.0f;

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.W))
        {
            verticalValue = 1.0f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            verticalValue = -1.0f;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            horizontalValue = 1.0f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            horizontalValue = -1.0f;
        }

        _Player.MovePlayerObject(horizontalValue, verticalValue);
#endif
    }

    public override void PointerDownFunction()
    {
        base.PointerDownFunction();

        _Player.MovePlayerObject(JoystickHorizontalValue, JoystickVerticalValue);
    }
}
