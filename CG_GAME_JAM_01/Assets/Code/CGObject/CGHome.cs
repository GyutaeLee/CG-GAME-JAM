using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGHome : CGObject
{
    public int HomeHP = 100;

    public override void InitializeCGObject()
    {
        base.InitializeCGObject();

        IsMovable = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        CGObject _CGObject = collision.transform.GetComponent<CGObject>();
        if (_CGObject == null)
        {
            return;
        }

        if (_CGObject.IsMovable == false)
        {
            return;
        }

        if (_CGObject.ObjectState == CGObject.EObjectState.OBJECT_STATE_THROWN)
        {
            //?? 추후에 오브젝트 가중치 추가
            HomeHP -= 5;

            Debug.Log("HOME HIT in THROWN! - HP : " + HomeHP);
        }
        else if (_CGObject.ObjectState == CGObject.EObjectState.OBJECT_STATE_ROLLING)
        {
            //?? 추후에 오브젝트 가중치 추가
            HomeHP -= 5;

            Debug.Log("HOME HIT in ROLLING! - HP : " + HomeHP);
        }

    }
}
