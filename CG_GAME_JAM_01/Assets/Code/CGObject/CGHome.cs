using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGHome : CGObject
{
    void Start()
    {
        InitializeCGHome();
    }

    private void InitializeCGHome()
    {
        IsMovable = false;
    }
}
