using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGObject : MonoBehaviour
{
    public enum EObjectState
    {
        OBJECT_STATE_NONE   = 0,
        OBJECT_STATE_GROUND,
        OBJECT_STATE_PICKED,
        OBJECT_STATE_THROWN,
        OBJECT_STATE_ROLLING,

        MAX_OBJECT_STATE
    }

    public bool isMovable;
    public EObjectState eObjectState;

    public bool IsPickable()
    {
        bool isPickable = false;

        if (eObjectState == EObjectState.OBJECT_STATE_GROUND && isMovable == true)
        {
            isPickable = true;
        }

        return isPickable;
    }

    public virtual void Start()
    {
        InitializeCGObject();
    }

    public virtual void InitializeCGObject()
    {
        // Set layer
        int cgObjectLayerIndex = LayerMask.NameToLayer("CGObjectInArea");
        gameObject.layer = cgObjectLayerIndex;
    }
}
