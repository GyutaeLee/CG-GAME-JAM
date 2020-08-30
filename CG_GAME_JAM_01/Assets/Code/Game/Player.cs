using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPlayerView
{
    PLAYER_VIEW_NONE = 0,

    PLAYER_VIEW_THIRD_VIEW,
    PLAYER_VIEW_WORLD_VIEW,

    MAX_PLAYER_VIEW,

}

public class Player : MonoBehaviour
{
    private const int kAreaCount = 6;
    private const float kStagePlayerRotateSpeed = 100.0f;
    private readonly Vector3 kOverlapBoxForwardOffset = new Vector3(0.0f, 0.0f, 0.2f); // Player Position에서 얼만큼 앞에 Overlap Box를 둘 지 (y, z값으로 위치 조절)

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
    }

    public EPlayerView PlayerView
    {
        get { return this.m_ePlayerView; }
    }

    public float playerMoveSpeed;

    private PhysicsWorldManager m_pwManager;
    private WorldSetter m_worldSetter;
    private CharacterController m_characterController;

    private Vector3 m_moveForwardVector;
    private Vector3 m_moveRightVector;

    private Vector3 m_playerUpVector;
    private Quaternion m_originQuaternion;

    private WorldSetter.ECubeArea m_eCubeArea;

    private float m_playerHeight;
    private float m_playerGravityScale;

    private Vector3 m_overlapBoxCenter;  
    private Vector3 m_overlapBoxHalfExtent;     // Overlap Box의 Half Extent

    private GameObject m_pickUpObject;

    private EPlayerState m_ePlayerState;
    private EPlayerView m_ePlayerView;

    private void Start()
    {
        InitPlayer();
    }

    private void Update()
    {
        UpdatePlayer();
    }

    public virtual void InitPlayer()
    {
        this.gameObject.layer = LayerMask.NameToLayer("CGPlayer");

        this.m_pwManager = GameObject.Find("PhysicsWorldManager").GetComponent<PhysicsWorldManager>();
        this.m_worldSetter = GameObject.Find("WorldSetter").GetComponent<WorldSetter>();
        this.m_characterController = GetComponent<CharacterController>();

        // @Chan : upVector도 eCubeArea에 따라 설정되게 해야하지 않을까? 예를들어 Cube아래에 있는 캐릭터의 up vector가 전혀 다를거라서.
        this.m_playerUpVector = this.transform.up;
        this.m_originQuaternion = this.transform.localRotation;

        this.m_playerHeight = this.m_characterController.height;
        this.m_playerGravityScale = 0.25f;

        this.m_overlapBoxCenter = new Vector3(0.0f, 0.0f, 0.0f);
        this.m_overlapBoxHalfExtent = new Vector3(0.05f, 0.1f, 0.05f);

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
        UpdateOverlapBoxCenter();
        UpdatePlayerGrounded();
    }

    private void UpdateOverlapBoxCenter()
    {
        this.m_overlapBoxCenter = this.transform.position + this.transform.localRotation * kOverlapBoxForwardOffset;
    }

    private void UpdatePlayerGrounded()
    {
        if (this.m_characterController.isGrounded == false)
        {
            Vector3 moveVector = Vector3.zero;

            moveVector += this.m_pwManager.GetCubeAreaGravity(this.m_eCubeArea);
            moveVector *= this.m_playerGravityScale;

            moveVector *= Time.deltaTime;

            // Move Player Object
            this.m_characterController.Move(moveVector);
        }
    }

    private void OnDrawGizmos()
    {
        Collider[] colliders = Physics.OverlapBox(this.m_overlapBoxCenter, this.m_overlapBoxHalfExtent, Quaternion.identity);
        bool isHit = colliders.Length > 0 ? true : false;

        if (isHit == true)
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireCube(this.m_overlapBoxCenter, this.m_overlapBoxHalfExtent * 2);
        Gizmos.color = Color.white;
    }

    protected float AddQuadrantValue(float x, float y)
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

    public void SetPlayerViewThirdView()
    {
        this.m_ePlayerView = EPlayerView.PLAYER_VIEW_THIRD_VIEW;
    }

    public void SetPlayerViewWorldView()
    {
        this.m_ePlayerView = EPlayerView.PLAYER_VIEW_WORLD_VIEW;
    }

    public void TogglePlayerView()
    {
        if (this.m_ePlayerView == EPlayerView.PLAYER_VIEW_THIRD_VIEW)
        {
            this.m_ePlayerView = EPlayerView.PLAYER_VIEW_WORLD_VIEW;
        }
        else if (this.m_ePlayerView == EPlayerView.PLAYER_VIEW_WORLD_VIEW)
        {
            this.m_ePlayerView = EPlayerView.PLAYER_VIEW_THIRD_VIEW;
        }
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
    public virtual void MovePlayerObject(float horizontalValue, float verticalValue)
    {
        if (horizontalValue == 0 && verticalValue == 0)
            return;

        float horizontalSpeed = horizontalValue * this.playerMoveSpeed;
        float verticalSpeed = verticalValue * this.playerMoveSpeed;
        Vector3 moveVector = Vector3.zero;

        if (this.m_ePlayerView == EPlayerView.PLAYER_VIEW_THIRD_VIEW)
        {
            moveVector += transform.forward * verticalSpeed;
            moveVector += transform.right * horizontalSpeed;
        }
        else if (this.m_ePlayerView == EPlayerView.PLAYER_VIEW_WORLD_VIEW)
        {
            moveVector += this.m_moveRightVector * horizontalSpeed;
            moveVector += this.m_moveForwardVector * verticalSpeed;
        }

        moveVector *= Time.deltaTime;

        // Move Player Object
        this.m_characterController.Move(moveVector);
    }

    public virtual void RotatePlayerObject(float horizontalValue, float verticalValue)
    {
        if (horizontalValue == 0 && verticalValue == 0)
            return;

        if (this.PlayerView == EPlayerView.PLAYER_VIEW_THIRD_VIEW)
        {
            float rotateValue = horizontalValue;

            if (verticalValue < 0)
            {
                rotateValue *= -1;
            }

            this.transform.Rotate(this.transform.up, rotateValue * kStagePlayerRotateSpeed * Time.deltaTime);
        }
        else if (this.PlayerView == EPlayerView.PLAYER_VIEW_WORLD_VIEW)
        {
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
            this.transform.rotation = Quaternion.AngleAxis(-rotateAngle, this.m_playerUpVector) * this.m_originQuaternion;
        }
    }

    /*
     * PHYSICS
     */
    public bool ShotRayCastToForward(out Transform objectTransform)
    {
        // 이미 물체를 들고 있음
        if (this.m_ePlayerState == EPlayerState.PLAYER_STATE_PICK)
        {
            objectTransform = null;
            return false;
        }

        Collider[] colliders = Physics.OverlapBox(this.m_overlapBoxCenter, this.m_overlapBoxHalfExtent, Quaternion.identity);
        bool isHit = colliders.Length > 0 ? true : false;

        if (isHit == true)
        {
            int closestColliderIndex = 0;
            float minSquaredLen = float.MaxValue;
            for(int i = 0; i < colliders.Length; ++i)
            {
                Vector3 distVec = colliders[i].bounds.center - this.m_overlapBoxCenter;
                float distSquaredLen = Vector3.Dot(distVec, distVec);

                if(distSquaredLen < minSquaredLen)
                {
                    minSquaredLen = distSquaredLen;
                    closestColliderIndex = i;
                }
            }
            
            objectTransform = colliders[closestColliderIndex].transform;
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

    public virtual void ThrowObject(float throwStrength)
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
    }
}

//?? 규태 : normalize 및 Time.deltaTime 하기
