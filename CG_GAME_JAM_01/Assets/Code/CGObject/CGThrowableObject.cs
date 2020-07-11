using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGThrowableObject : CGObject
{
    private Transform m_OriginTransform;

    private void Start()
    {
        InitializeCGThrowableObject();
    }

    private void InitializeCGThrowableObject()
    {
        IsMovable = true;

        m_OriginTransform = transform;
    }

    public void ResetObject()
    {
        transform.position = m_OriginTransform.position;
        transform.rotation = m_OriginTransform.rotation;
        transform.localScale = m_OriginTransform.localScale;

        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
