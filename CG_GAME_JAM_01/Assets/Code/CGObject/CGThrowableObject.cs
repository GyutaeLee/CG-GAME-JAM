using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGThrowableObject : CGObject
{
    private Vector3 m_OriginLocalPosition;
    private Quaternion m_OriginLocalRotation;
    private Vector3 m_OriginLocalScale;

    private Rigidbody m_Rigidbody;

    public bool CheckAndChangeObjectState()
    {
        bool bResult = false;

        // 굴러가는 도중에 힘이 다 되어서 멈추었을 때 상태를 변경해준다.
        if (ObjectState == EObjectState.OBJECT_STATE_ROLLING && m_Rigidbody.IsSleeping() == true)
        {
            ObjectState = EObjectState.OBJECT_STATE_GROUND;
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

            bResult = true;
        }

        return bResult;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 던져진 도중에 무언가에 부딪히면 굴러가는 상태로 변경한다.
        if (ObjectState == EObjectState.OBJECT_STATE_THROWN)
        {
            ObjectState = EObjectState.OBJECT_STATE_ROLLING;
        }
    }

    public override void InitializeCGObject()
    {
        base.InitializeCGObject();

        IsMovable = true;

        m_OriginLocalPosition = transform.localPosition;
        m_OriginLocalRotation = transform.localRotation;
        m_OriginLocalScale = transform.localScale;

        m_Rigidbody = transform.GetComponent<Rigidbody>();
    }

    public void ResetObject()
    {
        transform.localPosition = m_OriginLocalPosition;
        transform.localRotation = m_OriginLocalRotation;
        transform.localScale = m_OriginLocalScale;

        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        ObjectState = EObjectState.OBJECT_STATE_GROUND;

        m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void SetLayerAsThrown()
    {
        int layerIndex = LayerMask.NameToLayer("CGThrown");
        gameObject.layer = layerIndex;
    }

    public void SetLayerAsInArea()
    {
        int layerIndex = LayerMask.NameToLayer("CGObjectInArea");
        gameObject.layer = layerIndex;
    }
}
