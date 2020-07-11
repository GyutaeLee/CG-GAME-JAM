using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager _GameManager;
    public GameObject PlayerObject;
    public Rigidbody PlayerRigidbody;

    public float playerMoveSpeed = 0.01f;

    private const int m_AreaCount = 6;
    private Vector3[] m_MoveForwardVectors;
    private Vector3[] m_MoveRightVectors;

    private Vector3 m_PlayerUpVector;
    private Quaternion m_OriginQuaternion;

    private void Start()
    {
        InitializePlayer();
    }

    private void InitializePlayer()
    {
        SetDirectionVectors();

        m_PlayerUpVector = PlayerObject.transform.up;
        m_OriginQuaternion = GetComponent<Transform>().localRotation;
    }

    private void SetDirectionVectors()
    {
        m_MoveForwardVectors = new Vector3[m_AreaCount];
        m_MoveRightVectors = new Vector3[m_AreaCount];

        m_MoveForwardVectors[(int)WorldSetter.CubeAreaEnum.YP] = Vector3.forward;
        m_MoveRightVectors[(int)WorldSetter.CubeAreaEnum.YP] = Vector3.right;

        m_MoveForwardVectors[(int)WorldSetter.CubeAreaEnum.ZM] = Vector3.up;
        m_MoveRightVectors[(int)WorldSetter.CubeAreaEnum.ZM] = Vector3.right;
    }

    private float AddQuadrantValue(float x, float y)
    {
        float quadrantValue = 0.0f;

        if (x > 0 && y > 0)
        {
            quadrantValue = 0.0f;
        }
        else if (x < 0 && y > 0)
        {
            quadrantValue = -180.0f;
        }
        else if (x < 0 && y < 0)
        {
            quadrantValue = -180.0f;
        }
        else if (x > 0 && y < 0)
        {
            quadrantValue = 0.0f;
        }

        return quadrantValue;
    }

    /*
     *  PLAYER CONTROL
     */
    public void MovePlayerObject(float horizontalValue, float verticalValue)    
    {
        if (horizontalValue == 0 && verticalValue == 0)
            return;

        float horizontalSpeed = horizontalValue * playerMoveSpeed;
        float verticalSpeed   = verticalValue * playerMoveSpeed;
        Vector3 moveVector = Vector3.zero;

        moveVector += m_MoveRightVectors[(int)_GameManager.PlayerCubeAreaEnum] * horizontalSpeed;
        moveVector += m_MoveForwardVectors[(int)_GameManager.PlayerCubeAreaEnum] * verticalSpeed;

        PlayerRigidbody.AddForce(moveVector);
    }

    public void RotatePlayerObject(float horizontalValue, float verticalValue)
    {
        if (horizontalValue == 0 && verticalValue == 0)
            return;

        float rotateAngle = 0.0f;

        // 조이스틱 degree 계산
        rotateAngle = Mathf.Atan(verticalValue / horizontalValue);
        rotateAngle *= (float)(180.0f / Mathf.PI);

        // 탄젠트 주기에 따른 가중치 계산
        rotateAngle += AddQuadrantValue(horizontalValue, verticalValue);

        // 큐브 오브젝트 위치에 맞게 -90 가중치 계산
        rotateAngle -= 90.0f;

        // 플레이어 오브젝트의 up 벡터를 기준으로 절대 각도 '-' (반시계방향으로 변경) 회전
        PlayerObject.transform.rotation = Quaternion.AngleAxis(-rotateAngle , m_PlayerUpVector) * m_OriginQuaternion;
    }
}

//?? 규태 : normalize 및 Time.deltaTime 하기
