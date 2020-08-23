using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test3rdPersonCamera : MonoBehaviour
{
    private GameObject m_camTarget;
    private Transform m_camTargetTransform;
    private Transform m_camTransform;
    private Camera m_cam;

    private float m_zoomLength;

    private void InitTest3rdPersonCamera()
    {
        m_camTarget = GameObject.Find("MainCharacter");
        m_camTargetTransform = m_camTarget.transform;
        m_camTransform = GetComponent<Transform>();
        m_cam = GetComponent<Camera>();

        m_zoomLength = 2.0f;
    }

    private void Start()
    {
        InitTest3rdPersonCamera();
    }

    private Vector3[] GetCameraNearPlaneWorldPositions()
    {
        Vector3[] nearPlanePositions = new Vector3[4];

        Vector3 camForward = m_camTransform.forward;
        Vector3 camUp = m_camTransform.up;
        Vector3 camRight = m_camTransform.right;

        float nearDist = m_cam.nearClipPlane;
        float fovRadian = Mathf.Deg2Rad * m_cam.fieldOfView;
        float nearPlaneHalfHeight = Mathf.Tan(fovRadian) * nearDist;
        float nearPlaneHalfWidth = nearPlaneHalfHeight * m_cam.aspect;

        Vector3 nearPlaneCenter = m_camTransform.position + camForward * nearDist;

        // left-upper
        nearPlanePositions[0] = nearPlaneCenter + camUp * nearPlaneHalfHeight - camRight * nearPlaneHalfWidth;

        // right-upper
        nearPlanePositions[1] = nearPlanePositions[0] + camRight * nearPlaneHalfWidth * 2.0f;

        // left-lower
        nearPlanePositions[2] = nearPlanePositions[0] - camUp * nearPlaneHalfHeight * 2.0f;

        // right-lower
        nearPlanePositions[3] = nearPlanePositions[2] + camRight * nearPlaneHalfWidth * 2.0f;

        return nearPlanePositions;
    }

    // https://www.gamasutra.com/blogs/EricUndersander/20131001/201382/Accurate_Collision_Zoom_for_Cameras.php
    private Vector3 HandleCollisionZoom()
    {
        Vector3 camPos = m_camTransform.position;
        Vector3 targetPos = m_camTargetTransform.position;

        float offsetDist = Vector3.Magnitude(targetPos - camPos);
        float rayCastLength = offsetDist - m_zoomLength;
        if(rayCastLength < 0.0f)
        {
            return camPos;
        }

        Vector3 camOut = Vector3.Normalize(targetPos - camPos);
        Vector3 nearestCamPos = targetPos - camOut * m_zoomLength;
        float minHitFraction = 1.0f;

        Vector3[] nearPlanePoints = GetCameraNearPlaneWorldPositions();
        
        for (int i = 0; i < 4; i++)
        {
            Vector3 corner = nearPlanePoints[i];
            Vector3 offsetToCorner = corner - camPos;
            Vector3 rayStart = nearestCamPos + offsetToCorner;
            Vector3 rayEnd = corner;

            float rayMaxdistance = Vector3.Magnitude(rayEnd - rayStart);

            // a result between 0 and 1 indicates a hit along the ray segment
            RaycastHit hitInfo = new RaycastHit();
            Physics.Raycast(rayStart, (rayEnd - rayStart) / rayMaxdistance, out hitInfo, rayMaxdistance);

            minHitFraction = Mathf.Min(minHitFraction, hitInfo.distance / rayMaxdistance);
        }

        if (minHitFraction < 1.0f)
        {
            return nearestCamPos - camOut * (rayCastLength * minHitFraction);
        }
        else
        {
            return camPos;
        }
    }

    private void Update()
    {
        m_camTransform.position = HandleCollisionZoom();
    }
}
