using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGThrowableObject : CGObject
{
    private Vector3 m_OriginLocalPosition;
    private Quaternion m_OriginLocalRotation;
    private Vector3 m_OriginLocalScale;

    public override void InitializeCGObject()
    {
        base.InitializeCGObject();

        IsMovable = true;

        m_OriginLocalPosition = transform.localPosition;
        m_OriginLocalRotation = transform.localRotation;
        m_OriginLocalScale = transform.localScale;
    }

    public void ResetObject()
    {
        transform.localPosition = m_OriginLocalPosition;
        transform.localRotation = m_OriginLocalRotation;
        transform.localScale = m_OriginLocalScale;

        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
