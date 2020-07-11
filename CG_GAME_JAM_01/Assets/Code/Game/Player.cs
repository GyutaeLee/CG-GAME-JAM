using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager _GameManager;
    public GameObject PlayerObject;
    public Rigidbody PlayerRigidbody;

    public float playerMoveSpeed = 0.01f;

    private const int areaCount = 6;
    public Vector3[] forwardVectors;
    public Vector3[] rightVectors;

    private void Start()
    {
        InitializePlayer();
    }

    private void InitializePlayer()
    {
        SetDirectionVectors();
    }

    private void SetDirectionVectors()
    {
        forwardVectors = new Vector3[areaCount];
        rightVectors = new Vector3[areaCount];

        forwardVectors[(int)WorldSetter.CubeAreaEnum.YP] = Vector3.forward;
        rightVectors[(int)WorldSetter.CubeAreaEnum.YP] = Vector3.right;

        forwardVectors[(int)WorldSetter.CubeAreaEnum.ZM] = Vector3.up;
        rightVectors[(int)WorldSetter.CubeAreaEnum.ZM] = Vector3.right;
    }


    public void MovePlayerObject(float horizontalValue, float verticalValue)    
    {
        if (horizontalValue == 0 && verticalValue == 0)
            return;

        float horizontalSpeed = horizontalValue * playerMoveSpeed;
        float verticalSpeed   = verticalValue * playerMoveSpeed;
        Vector3 moveVector = Vector3.zero;

        moveVector += rightVectors[(int)_GameManager.PlayerCubeAreaEnum] * horizontalSpeed;
        moveVector += forwardVectors[(int)_GameManager.PlayerCubeAreaEnum] * verticalSpeed;

        PlayerRigidbody.AddForce(moveVector);
    }

    public void RotatePlayerObject(float horizontalValue, float verticalValue)
    {
        if (horizontalValue == 0 && verticalValue == 0)
            return;

        Vector3 rotateVector = Vector3.zero;

        rotateVector += Vector3.right * horizontalValue;
        rotateVector += Vector3.forward * verticalValue;

        Quaternion quaternion = Quaternion.LookRotation(rotateVector);
        PlayerObject.transform.rotation = quaternion;
    }
}
