using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacterController : MonoBehaviour
{
    public float characterSpeed;

    private Transform m_transform;
    private CharacterController m_characterController;

    private void InitTestCharacterController()
    {
        m_transform = GetComponent<Transform>();
        m_characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        InitTestCharacterController();
    }

    private void Update()
    {
        if(m_characterController.isGrounded == false)
        {
            m_characterController.Move(Physics.gravity * Time.deltaTime);
        }

        if(Input.GetKey(KeyCode.W) == true)
        {
            m_characterController.Move(transform.forward * Time.deltaTime * characterSpeed);
        }

        if (Input.GetKey(KeyCode.A) == true)
        {
            m_characterController.Move(transform.right * -1.0f * Time.deltaTime * characterSpeed);
        }

        if (Input.GetKey(KeyCode.S) == true)
        {
            m_characterController.Move(transform.forward * -1.0f * Time.deltaTime * characterSpeed);
        }

        if (Input.GetKey(KeyCode.D) == true)
        {
            m_characterController.Move(transform.right * Time.deltaTime * characterSpeed);
        }
    }
}
