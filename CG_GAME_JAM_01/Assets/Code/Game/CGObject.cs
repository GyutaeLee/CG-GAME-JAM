using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGObject : MonoBehaviour
{
    public enum EObjectType
    { 
        OBJECT_TYPE_NONE    = 0,
        OBJECT_TYPE_MOVABLE,
        OBJECT_TYPE_UNMOVABLE,

        MAX_OBJECT_TYPE
    }

    public enum EObjectState
    {
        OBJECT_STATE_NONE   = 0,
        OBJECT_STATE_GROUND,
        OBJECT_STATE_PICKED,
        OBJECT_STATE_THROWN,
        OBJECT_STATE_ROLLING,

        MAX_OBJECT_STATE
    }

    public EObjectType ObjectType;
    public EObjectState ObjectState;

    public bool IsMovable()
    {
        bool isMovable = false;

        isMovable = (ObjectType == EObjectType.OBJECT_TYPE_MOVABLE);

        return isMovable;
    }

    public bool IsPickable()
    {
        bool isPickable = false;

        isPickable = (ObjectState == EObjectState.OBJECT_STATE_GROUND);

        return isPickable;
    }

    private void Start()
    {
        // Set layer
        int cgObjectLayerIndex = LayerMask.NameToLayer("CGObject");
        gameObject.layer = cgObjectLayerIndex;
    }
}
