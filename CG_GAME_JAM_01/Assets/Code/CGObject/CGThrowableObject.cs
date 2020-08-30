using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGThrowableObject : CGObject
{
    private Vector3 m_originLocalPosition;
    private Quaternion m_originLocalRotation;
    private Vector3 m_originLocalScale;

    private Rigidbody m_rigidbody;
    private Vector3 m_cubeAreaGravity;

    private void FixedUpdate()
    {
        if(this.m_cubeAreaGravity != Vector3.zero)
        {
            this.m_rigidbody.AddForce(this.m_cubeAreaGravity * this.m_rigidbody.mass * this.m_rigidbody.mass);
        }
    }

    public bool CheckAndChangeObjectState()
    {
        bool bResult = false;

        // 굴러가는 도중에 힘이 다 되어서 멈추었을 때 상태를 변경해준다.
        if (eObjectState == EObjectState.OBJECT_STATE_ROLLING && IsObjectStop() == true)
        {
            SetObjectStateGround();

            bResult = true;
        }

        return bResult;
    }

    private bool IsObjectStop()
    {
        bool bResult = false;
        const float kCGEpsilon = 0.1f;

        if (kCGEpsilon >= Mathf.Abs(this.m_rigidbody.velocity.x)
            && kCGEpsilon >= Mathf.Abs(this.m_rigidbody.velocity.y)
            && kCGEpsilon >= Mathf.Abs(this.m_rigidbody.velocity.z)
            && kCGEpsilon >= Mathf.Abs(this.m_rigidbody.angularVelocity.x)
            && kCGEpsilon >= Mathf.Abs(this.m_rigidbody.angularVelocity.y)
            && kCGEpsilon >= Mathf.Abs(this.m_rigidbody.angularVelocity.z)
            )
        {
            bResult = true;
        }

        return bResult;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 던져진 도중에 무언가에 부딪히면 굴러가는 상태로 변경한다.
        if (eObjectState == EObjectState.OBJECT_STATE_THROWN)
        {
            SetObjectStateRolling();
        }
    }

    public override void InitializeCGObject()
    {
        base.InitializeCGObject();

        isMovable = true;

        this.m_originLocalPosition = transform.localPosition;
        this.m_originLocalRotation = transform.localRotation;
        this.m_originLocalScale = transform.localScale;

        this.m_rigidbody = GetComponent<Rigidbody>();
        this.m_rigidbody.useGravity = false;
        
        this.m_rigidbody.mass = 0.3f;
        this.m_rigidbody.drag = 1.0f;
        this.m_rigidbody.angularDrag = 1.0f;

        this.m_cubeAreaGravity = Vector3.zero;
    }

    public void ResetObject()
    {
        transform.localPosition = this.m_originLocalPosition;
        transform.localRotation = this.m_originLocalRotation;
        transform.localScale = this.m_originLocalScale;

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
        eObjectState = EObjectState.OBJECT_STATE_GROUND;

        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        this.m_rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        this.m_cubeAreaGravity = Vector3.zero;
    }

    public void SetObjectStatePicked()
    {
        eObjectState = EObjectState.OBJECT_STATE_PICKED;
    }

    public void SetObjectStateThrown()
    {
        eObjectState = EObjectState.OBJECT_STATE_THROWN;

        this.m_rigidbody.constraints = RigidbodyConstraints.None;
        transform.parent = null;
    }

    public void SetObjectStateRolling()
    {
        eObjectState = EObjectState.OBJECT_STATE_ROLLING;
    }

    public void SetCubeAreaGravity(Vector3 v)
    {
        this.m_cubeAreaGravity = v;
    }
}
