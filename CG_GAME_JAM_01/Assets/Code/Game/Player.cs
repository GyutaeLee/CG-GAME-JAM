using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const int kAreaCount = 6;

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
        get { return this.m_ePlayerState; }
        set { this.m_ePlayerState = value; }
    }

    public float playerMoveSpeed;

    private GameManager m_gameManager;
    private PhysicsWorldManager m_pwManager;
    private WorldSetter m_worldSetter;
    private Rigidbody m_rigidbody;

    private Vector3 m_moveForwardVector;
    private Vector3 m_moveRightVector;

    private Vector3 m_playerUpVector;
    private Quaternion m_originQuaternion;

    private WorldSetter.ECubeArea m_eCubeArea;

    private Vector3 m_playerRayPosition;
    private RaycastHit m_raycastHit;

    private bool m_isRaycastHit;

    private float m_playerHeight;
    private float m_maxRaycastDistance;

    private GameObject m_pickUpObject;

    private EPlayerState m_ePlayerState;

    private void Start()
    {
        InitPlayer();
    }

    private void Update()
    {
        UpdatePlayer();
    }

    private void InitPlayer()
    {
        this.gameObject.layer = LayerMask.NameToLayer("CGPlayer");

        this.m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.m_pwManager = GameObject.Find("PhysicsWorldManager").GetComponent<PhysicsWorldManager>();
        this.m_worldSetter = GameObject.Find("WorldSetter").GetComponent<WorldSetter>();
        this.m_rigidbody = this.GetComponent<Rigidbody>();

        this.m_playerUpVector = this.transform.up;
        this.m_originQuaternion = this.transform.localRotation;

        this.m_playerHeight = this.transform.GetComponent<CapsuleCollider>().height;
        this.m_maxRaycastDistance = this.m_playerHeight * 0.15f;

        SetPlayerDirectionVectors();
    }

    private void SetPlayerDirectionVectors()
    {
        this.m_eCubeArea = this.m_worldSetter.GetCubeAreaEnum(this.transform.position);

        switch (this.m_eCubeArea)
        {
            case WorldSetter.ECubeArea.YP:
                this.m_moveForwardVector = Vector3.forward;
                this.m_moveRightVector = Vector3.right;
                break;
            case WorldSetter.ECubeArea.ZM:
                this.m_moveForwardVector = Vector3.up;
                this.m_moveRightVector = Vector3.right;
                break;
            default:
                break;
        }
    }

    private void UpdatePlayer()
    {
        Vector3 rayVector = this.transform.position;

        rayVector += this.m_playerUpVector * (this.m_playerHeight * 0.5f * this.transform.localScale.y);

        this.m_playerRayPosition = rayVector;
    }

    private void OnDrawGizmos()
    {
        if (this.m_isRaycastHit == true)
        {
            Gizmos.DrawRay(this.m_playerRayPosition, this.transform.forward * this.m_raycastHit.distance);
            Gizmos.DrawWireCube(this.m_playerRayPosition + this.transform.forward * this.m_raycastHit.distance, this.transform.lossyScale);
        }
        else
        {
            Gizmos.DrawRay(this.m_playerRayPosition, this.transform.forward * this.m_maxRaycastDistance);
            Gizmos.DrawWireCube(this.m_playerRayPosition + this.transform.forward * this.m_maxRaycastDistance, this.transform.lossyScale);
        }
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
        this.m_ePlayerState = EPlayerState.PLAYER_STATE_IDLE;

        // 들고 있다가 턴이 끝났을 때
        if (this.m_pickUpObject != null)
        {
            this.m_pickUpObject.GetComponent<CGThrowableObject>().ResetObject();
            this.m_pickUpObject.transform.parent = null;
            this.m_pickUpObject = null;
        }

        this.m_isRaycastHit = false;
    }

    public void SetPlayerStateReady()
    {
        this.m_ePlayerState = EPlayerState.PLAYER_STATE_READY;
        this.m_pickUpObject = null;
    }

    public void SetPlayerStatePick(Transform objectTransform)
    {
        this.m_ePlayerState = EPlayerState.PLAYER_STATE_PICK;

        this.m_pickUpObject = objectTransform.gameObject;
    }

    public void SetPlayerStateThrow()
    {
        this.m_ePlayerState = EPlayerState.PLAYER_STATE_THROW;
        this.m_pickUpObject = null;
    }

    public bool CanPlayerPickObject()
    {
        if (this.m_ePlayerState != EPlayerState.PLAYER_STATE_READY)
        {
            return false;
        }

        return true;
    }

    public bool CanPlayerThrowObject()
    {
        if (this.m_ePlayerState != EPlayerState.PLAYER_STATE_PICK)
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

        float horizontalSpeed = horizontalValue * this.playerMoveSpeed * Time.deltaTime;
        float verticalSpeed   = verticalValue * this.playerMoveSpeed * Time.deltaTime;
        Vector3 moveVector = Vector3.zero;

        moveVector += this.m_moveRightVector * horizontalSpeed;
        moveVector += this.m_moveForwardVector * verticalSpeed;

        this.m_rigidbody.AddForce(moveVector);
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

#if UNITY_EDITOR
        if (horizontalValue == -1.0f && verticalValue == 0.0f)
        {
            rotateAngle = 90.0f;
        }
        else if (horizontalValue == 1.0f && verticalValue == 0.0f)
        {
            rotateAngle = 270.0f;
        }

        if (horizontalValue == 0.0f && verticalValue == -1.0f)
        {
            rotateAngle = 180.0f;
        }
        else if (horizontalValue == 0.0f && verticalValue == 1.0f)
        {
            rotateAngle = 0.0f;
        }
#endif

        // 플레이어 오브젝트의 up 벡터를 기준으로 절대 각도 '-' (반시계방향으로 변경) 회전
        this.transform.rotation = Quaternion.AngleAxis(-rotateAngle , this.m_playerUpVector) * this.m_originQuaternion;
    }

    /*
     * PHYSICS
     */
    public bool ShotRayCastToForward(out Transform objectTransform)
    {
        // 이미 물체를 들고 있음
        if (this.m_isRaycastHit == true)
        {
            objectTransform = null;
            return false;
        }

        this.m_isRaycastHit = Physics.BoxCast(this.m_playerRayPosition, this.transform.lossyScale, this.transform.forward,
                                        out this.m_raycastHit, this.transform.rotation, this.m_maxRaycastDistance);

        if (this.m_isRaycastHit == true)
        {
            objectTransform = this.m_raycastHit.transform;
            return true;
        }
        else
        {
            objectTransform = null;
            return false;
        }
    }

    public bool PickUpObject(Transform objectTransform)
    {
        if (CanPlayerPickObject() == false)
        {
            return false;
        }

        // 3. 물체를 집는다.
        objectTransform.GetComponent<CGThrowableObject>().SetObjectStatePicked();

        // 4. 물체를 플레이어 우측 팔 부근으로 이동시킨다. + 자식 오브젝트로 붙인다.
        objectTransform.parent = this.transform;

        Vector3 pickUpObjectPosition = objectTransform.position;
        pickUpObjectPosition.y += this.transform.position.y * 0.1f;
        objectTransform.position = pickUpObjectPosition;

        // 플레이어의 상태를 집은 상태로 변경한다.
        SetPlayerStatePick(objectTransform);

        return true;
    }

    public void ThrowObject(float throwStrength)
    {
        Vector3 playerThrowVector;

        //?? 규태 : 방향은 유저가 바라보고 있는 방향으로 변경 요망
        playerThrowVector = (this.transform.up + this.transform.forward).normalized;

        // PickUpObject가 날아갈 수 있게 상태를 만든다.
        this.m_pickUpObject.GetComponent<CGThrowableObject>().SetObjectStateThrown();

        // 오브젝트를 던진다.
        this.m_pwManager.LaunchCGObject(this.m_pickUpObject.GetComponent<CGThrowableObject>(), playerThrowVector, throwStrength);

        // 플레이어와 게임의 상태를 던지고 있는 상태로 변경한다.
        SetPlayerStateThrow();

        //?? 규태 : 다른 곳에 콜백 만들고 지우기
        SetPlayerStateIdle();
        this.m_gameManager.SetGameStateThrowing();
    }
}

//?? 규태 : normalize 및 Time.deltaTime 하기
