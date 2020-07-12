using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum EPlayerState
    {
        PLAYER_STATE_NONE   = 0,
        PLAYER_STATE_IDLE,

        PLAYER_STATE_READY,
        PLAYER_STATE_PICK,
        PLAYER_STATE_THROW,

        MAX_PLAYER_STATE,
    }

    public EPlayerState PlayerState
    { 
        get { return m_PlayerState; }
        set { m_PlayerState = value; }
    }

    public GameManager _GameManager;

    private PhysicsWorldManager PWManager;


    public GameObject PlayerObject;
    public Rigidbody PlayerRigidbody;

    public float playerMoveSpeed = 0.01f;

    private const int m_AreaCount = 6;
    private Vector3[] m_MoveForwardVectors;
    private Vector3[] m_MoveRightVectors;

    private Vector3 m_PlayerUpVector;
    private Quaternion m_OriginQuaternion;

    private Vector3 m_PlayerRayVector;
    private RaycastHit m_RaycastHit;
    private float m_PlayerHeight;
    private float m_MaxRaycastDistance = 0.3f;

    private GameObject m_PickUpObject;

    private EPlayerState m_PlayerState;

    private void Start()
    {
        InitializePlayer();
    }

    private void Update()
    {
        UpdatePlayer();

        DebugDrawRay();
    }

    private void InitializePlayer()
    {
        int cgPlayerLayer = LayerMask.NameToLayer("CGPlayer");
        gameObject.layer = cgPlayerLayer;

        SetDirectionVectors();

        PWManager = GameObject.Find("PhysicsWorldManager").GetComponent<PhysicsWorldManager>();

        m_PlayerUpVector = PlayerObject.transform.up;
        m_OriginQuaternion = PlayerObject.transform.localRotation;

        m_PlayerHeight = PlayerObject.transform.GetComponent<CapsuleCollider>().height;
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

    private void UpdatePlayer()
    {
        Vector3 rayVector = PlayerObject.transform.position;

        rayVector.y += m_PlayerHeight * 0.5f * PlayerObject.transform.localScale.y;

        m_PlayerRayVector = rayVector;
    }

    private void DebugDrawRay()
    {
        Debug.DrawRay(m_PlayerRayVector, PlayerObject.transform.forward * m_MaxRaycastDistance, Color.blue, 0.3f);
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
     * 
     */

    public void SetPlayerStateIdle()
    {
        PlayerState = EPlayerState.PLAYER_STATE_IDLE;

        // 들고 있다가 턴이 끝났을 때
        if (m_PickUpObject != null)
        {
            m_PickUpObject.GetComponent<CGThrowableObject>().ResetObject();
            m_PickUpObject.transform.parent = null;
            m_PickUpObject = null;
        }
    }

    public void SetPlayerStateReady()
    {
        PlayerState = EPlayerState.PLAYER_STATE_READY;
        m_PickUpObject = null;
    }

    public void SetPlayerStatePick(Transform objectTransform)
    {
        PlayerState = EPlayerState.PLAYER_STATE_PICK;

        m_PickUpObject = objectTransform.gameObject;
    }

    public void SetPlayerStateThrow()
    {
        PlayerState = EPlayerState.PLAYER_STATE_THROW;
        m_PickUpObject = null;
    }

    public bool CanPlayerPick()
    {
        if (PlayerState != EPlayerState.PLAYER_STATE_READY)
        {
            return false;
        }

        return true;
    }

    public bool CanPlayerThrow()
    {
        if (PlayerState != EPlayerState.PLAYER_STATE_PICK)
        {
            return false;
        }

        return true;
    }

    /*
     *  PLAYER CONTROL
     */
    public void MovePlayerObject(float horizontalValue, float verticalValue)    
    {
        if (horizontalValue == 0 && verticalValue == 0)
            return;

        float horizontalSpeed = horizontalValue * playerMoveSpeed * Time.deltaTime;
        float verticalSpeed   = verticalValue * playerMoveSpeed * Time.deltaTime;
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
        if (horizontalValue != 0)
        {
            rotateAngle = Mathf.Atan(verticalValue / horizontalValue);
        }

        rotateAngle *= (float)(180.0f / Mathf.PI);

        // 탄젠트 주기에 따른 가중치 계산
        rotateAngle += AddQuadrantValue(horizontalValue, verticalValue);

        // 큐브 오브젝트 위치에 맞게 -90 가중치 계산
        rotateAngle -= 90.0f;

        // 플레이어 오브젝트의 up 벡터를 기준으로 절대 각도 '-' (반시계방향으로 변경) 회전
        PlayerObject.transform.rotation = Quaternion.AngleAxis(-rotateAngle , m_PlayerUpVector) * m_OriginQuaternion;
    }

    /*
     * PHYSICS
     */
    public bool ShotRayCastToForward(out Transform objectTransform)
    {
        if (Physics.Raycast(m_PlayerRayVector, PlayerObject.transform.forward, out m_RaycastHit, m_MaxRaycastDistance))
        {
            objectTransform = m_RaycastHit.transform;            
            return true;
        }
        objectTransform = null;
        return false;
    }

    public bool PickUpObject(Transform objectTransform)
    {
        if (CanPlayerPick() == false)
        {
            return false;
        }

        // 3. 물체를 집는다.
        objectTransform.GetComponent<CGThrowableObject>().SetObjectStatePicked();

        // 4. 물체를 플레이어 우측 팔 부근으로 이동시킨다. + 자식 오브젝트로 붙인다.
        objectTransform.parent = PlayerObject.transform;

        Vector3 pickUpObjectPosition = objectTransform.position;
        pickUpObjectPosition.y += PlayerObject.transform.position.y * 0.1f;
        objectTransform.position = pickUpObjectPosition;

        // 플레이어의 상태를 집은 상태로 변경한다.
        SetPlayerStatePick(objectTransform);

        return true;
    }

    public void ThrowObject(float throwStrength)
    {
        Vector3 playerThrowVector;

        //?? 규태 : 방향은 유저가 바라보고 있는 방향으로 변경 요망
        playerThrowVector = (PlayerObject.transform.up + PlayerObject.transform.forward).normalized;

        // PickUpObject가 날아갈 수 있게 상태를 만든다.
        m_PickUpObject.GetComponent<CGThrowableObject>().SetObjectStateThrown();

        // 오브젝트를 던진다.
        PWManager.LaunchCGObject(m_PickUpObject.GetComponent<CGThrowableObject>(), playerThrowVector, throwStrength);

        // 플레이어와 게임의 상태를 던지고 있는 상태로 변경한다.
        SetPlayerStateThrow();

        //?? 규태 : 다른 곳에 콜백 만들고 지우기
        SetPlayerStateIdle();

        _GameManager.SetGameStateThrowing();
    }
}

//?? 규태 : normalize 및 Time.deltaTime 하기
