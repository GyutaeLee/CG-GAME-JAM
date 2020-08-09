using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiPlayerMoveController : Joystick
{
    //?? 규태 : 추후에 각 플레이어에 맞게 불러오자
    public Player player;
        
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

        this.player.RotatePlayerObject(horizontalValue, verticalValue);
        this.player.MovePlayerObject(horizontalValue, verticalValue);
#endif
    }

    public override void PointerDownFunction()
    {
        base.PointerDownFunction();

        this.player.RotatePlayerObject(base.JoystickHorizontalValue, base.JoystickVerticalValue);
        this.player.MovePlayerObject(base.JoystickHorizontalValue, base.JoystickVerticalValue);
    }
}
