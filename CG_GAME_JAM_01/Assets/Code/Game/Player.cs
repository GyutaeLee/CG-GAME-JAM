using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject PlayerObject;

    public float playerMoveSpeed = 0.01f;

    public void MovePlayerObject(float horizontalValue, float verticalValue)    
    {
        float horizontalSpeed = horizontalValue * playerMoveSpeed;
        float verticalSpeed   = verticalValue * playerMoveSpeed;

        PlayerObject.transform.Translate(Vector3.right * horizontalSpeed);
        PlayerObject.transform.Translate(Vector3.forward * verticalSpeed);
    }
}
