using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGThrowableObject : CGObject
{
    private Vector3 m_OriginLocalPosition;
    private Quaternion m_OriginLocalRotation;
    private Vector3 m_OriginLocalScale;

    private Rigidbody m_Rigidbody;
    private Vector3 m_CubeAreaGravity;

    private void FixedUpdate()
    {
        if(m_CubeAreaGravity != Vector3.zero)
        {
            m_Rigidbody.AddForce(m_CubeAreaGravity * m_Rigidbody.mass * m_Rigidbody.mass);
        }
    }

    public bool CheckAndChangeObjectState()
    {
        bool bResult = false;

        // 굴러가는 도중에 힘이 다 되어서 멈추었을 때 상태를 변경해준다.
        if (ObjectState == EObjectState.OBJECT_STATE_ROLLING && IsObjectStop() == true)
        {
            SetObjectStateGround();

            bResult = true;
        }

        return bResult;
    }

    private bool IsObjectStop()
    {
        bool bResult = false;
        const float CGEpsilon = 0.01f;

        if (CGEpsilon >= Mathf.Abs(m_Rigidbody.velocity.x)
            && CGEpsilon >= Mathf.Abs(m_Rigidbody.velocity.y)
            && CGEpsilon >= Mathf.Abs(m_Rigidbody.velocity.z)
            && CGEpsilon >= Mathf.Abs(m_Rigidbody.angularVelocity.x)
            && CGEpsilon >= Mathf.Abs(m_Rigidbody.angularVelocity.y)
            && CGEpsilon >= Mathf.Abs(m_Rigidbody.angularVelocity.z)
            )
        {
            bResult = true;
        }

        return bResult;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 던져진 도중에 무언가에 부딪히면 굴러가는 상태로 변경한다.
        if (ObjectState == EObjectState.OBJECT_STATE_THROWN)
        {
            SetObjectStateRolling();
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
        m_Rigidbody.useGravity = false;
        
        m_Rigidbody.mass = 0.3f;
        m_Rigidbody.drag = 1.0f;
        m_Rigidbody.angularDrag = 1.0f;

        m_CubeAreaGravity = Vector3.zero;
    }

    public void ResetObject()
    {
        transform.localPosition = m_OriginLocalPosition;
        transform.localRotation = m_OriginLocalRotation;
        transform.localScale = m_OriginLocalScale;

        SetObjectStateGround();
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

    public void SetObjectStateGround()
    {
        ObjectState = EObjectState.OBJECT_STATE_GROUND;

        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        m_Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        m_CubeAreaGravity = Vector3.zero;
    }

    public void SetObjectStatePicked()
    {
        ObjectState = EObjectState.OBJECT_STATE_PICKED;
    }

    public void SetObjectStateThrown()
    {
        ObjectState = EObjectState.OBJECT_STATE_THROWN;

        m_Rigidbody.constraints = RigidbodyConstraints.None;
        transform.parent = null;
    }

    public void SetObjectStateRolling()
    {
        ObjectState = EObjectState.OBJECT_STATE_ROLLING;
    }

    public void SetCubeAreaGravity(Vector3 v)
    {
        m_CubeAreaGravity = v;
    }
}
