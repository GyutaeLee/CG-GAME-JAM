using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGObject : MonoBehaviour
{
    public enum EObjectType
    { 
        OBJECT_TYPE_NONE = 0,
        OBJECT_TYPE_MOVABLE,
        OBJECT_TYPE_UNMOVABLE,

        MAX_OBJECT_TYPE
    }

    public EObjectType ObjectType;
}
