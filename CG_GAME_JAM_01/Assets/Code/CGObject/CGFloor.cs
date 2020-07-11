using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGFloor : CGObject
{
    void Start()
    {
        InitializeCGFloor();
    }

    private void InitializeCGFloor()
    {
        IsMovable = false;
    }
}
