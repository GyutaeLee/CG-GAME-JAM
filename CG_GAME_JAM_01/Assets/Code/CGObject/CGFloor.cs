using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGFloor : CGObject
{
    public override void InitializeCGObject()
    {
        base.InitializeCGObject();

        IsMovable = false;
    }
}
