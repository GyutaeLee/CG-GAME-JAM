using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class PhysicsWorldManager : MonoBehaviour
{
    private const float kLaunchPower = 30f;

    private class CGPhysicsObject
    {
        public enum EObjectState
        {
            OBJECT_LAUNCH_TRIGGER,
            OBJECT_MOVING,
            OBJECT_TARGET_AREA
        }
               
        public WorldSetter.ECubeArea ePreviousArea;
        public WorldSetter.ECubeArea eCurrentArea;
        public WorldSetter.ECubeArea eTargetArea;
        public EObjectState eObjectState;

        public CGThrowableObject throwableObject;
        public Rigidbody objectRigidBody;

        public Vector3 launchDirection;
        public float launchPower;
        public bool isAreaChanged;

        public CGPhysicsObject(CGThrowableObject o, Vector3 d, float p, WorldSetter ws)
        {
            WorldSetter.ECubeArea ae = ws.GetCubeAreaEnum(o.transform.position);

            this.ePreviousArea = ae;
            this.eCurrentArea = ae;
            this.eTargetArea = WorldSetter.ECubeArea.NONE;
            this.eObjectState = EObjectState.OBJECT_LAUNCH_TRIGGER;

            this.throwableObject = o;
            this.objectRigidBody = this.throwableObject.GetComponent<Rigidbody>();

            this.launchDirection = d;
            this.launchPower = p;
            this.isAreaChanged = false;
        }
    }

    private WorldSetter m_worldSetter;
    private WorldSetter.ECubeArea[,] m_targetArea;

    private Vector3[] m_cubeAreaGravity;
    private Vector3[,] m_forceSpace; //?? 찬행 : 확인 후 지우기

    private bool[,] m_isValidAreaChange;

    private List<CGPhysicsObject> m_objects;

    private void InitPhysicsWorldManager()
    {
        this.m_worldSetter = GameObject.Find("WorldSetter").GetComponent<WorldSetter>();

        // 동적 할당
        this.m_targetArea = new WorldSetter.ECubeArea[(uint)WorldSetter.ECubeArea.NONE, (uint)WorldSetter.ECubeArea.NONE];
        this.m_cubeAreaGravity = new Vector3[6];
        this.m_forceSpace = new Vector3[(uint)WorldSetter.ECubeArea.NONE, (uint)WorldSetter.ECubeArea.NONE];
        this.m_isValidAreaChange = new bool[(uint)WorldSetter.ECubeArea.NONE, (uint)WorldSetter.ECubeArea.NONE];
        this.m_objects = new List<CGPhysicsObject>();

        // 초기화
        for (int i = 0; i < (uint)WorldSetter.ECubeArea.NONE; ++i)
        {
            for (int j = 0; j < (uint)WorldSetter.ECubeArea.NONE; ++j)
            {
                this.m_isValidAreaChange[i, j] = false;
                this.m_forceSpace[i, j] = new Vector3(0f, 0f, 0f);
                this.m_targetArea[i, j] = WorldSetter.ECubeArea.NONE;
            }
        }
    }

    private void SetAreaInfo(WorldSetter.ECubeArea from, WorldSetter.ECubeArea to, Vector3 force, WorldSetter.ECubeArea target)
    {
        this.m_isValidAreaChange[(uint)from, (uint)to] = true;
        this.m_forceSpace[(uint)from, (uint)to] = force;
        this.m_targetArea[(uint)from, (uint)to] = target;
    }

    private void InitAreaInfo()
    {
        SetAreaInfo(WorldSetter.ECubeArea.XP, WorldSetter.ECubeArea.XP_YP, Vector3.left, WorldSetter.ECubeArea.YP);
        SetAreaInfo(WorldSetter.ECubeArea.XP, WorldSetter.ECubeArea.XP_YM, Vector3.left, WorldSetter.ECubeArea.YM);
        SetAreaInfo(WorldSetter.ECubeArea.XP, WorldSetter.ECubeArea.XP_ZP, Vector3.left, WorldSetter.ECubeArea.ZP);
        SetAreaInfo(WorldSetter.ECubeArea.XP, WorldSetter.ECubeArea.XP_ZM, Vector3.left, WorldSetter.ECubeArea.ZM);

        SetAreaInfo(WorldSetter.ECubeArea.XM, WorldSetter.ECubeArea.XM_YP, Vector3.right, WorldSetter.ECubeArea.YP);
        SetAreaInfo(WorldSetter.ECubeArea.XM, WorldSetter.ECubeArea.XM_YM, Vector3.right, WorldSetter.ECubeArea.YM);
        SetAreaInfo(WorldSetter.ECubeArea.XM, WorldSetter.ECubeArea.XM_ZP, Vector3.right, WorldSetter.ECubeArea.ZP);
        SetAreaInfo(WorldSetter.ECubeArea.XM, WorldSetter.ECubeArea.XM_ZM, Vector3.right, WorldSetter.ECubeArea.ZM);

        SetAreaInfo(WorldSetter.ECubeArea.YP, WorldSetter.ECubeArea.XP_YP, Vector3.down, WorldSetter.ECubeArea.XP);
        SetAreaInfo(WorldSetter.ECubeArea.YP, WorldSetter.ECubeArea.XM_YP, Vector3.down, WorldSetter.ECubeArea.XM);
        SetAreaInfo(WorldSetter.ECubeArea.YP, WorldSetter.ECubeArea.YP_ZP, Vector3.down, WorldSetter.ECubeArea.ZP);
        SetAreaInfo(WorldSetter.ECubeArea.YP, WorldSetter.ECubeArea.YP_ZM, Vector3.down, WorldSetter.ECubeArea.ZM);

        SetAreaInfo(WorldSetter.ECubeArea.YM, WorldSetter.ECubeArea.XP_YM, Vector3.up, WorldSetter.ECubeArea.XP);
        SetAreaInfo(WorldSetter.ECubeArea.YM, WorldSetter.ECubeArea.XM_YM, Vector3.up, WorldSetter.ECubeArea.XM);
        SetAreaInfo(WorldSetter.ECubeArea.YM, WorldSetter.ECubeArea.YM_ZP, Vector3.up, WorldSetter.ECubeArea.ZP);
        SetAreaInfo(WorldSetter.ECubeArea.YM, WorldSetter.ECubeArea.YM_ZM, Vector3.up, WorldSetter.ECubeArea.ZM);

        SetAreaInfo(WorldSetter.ECubeArea.ZP, WorldSetter.ECubeArea.XP_ZP, Vector3.back, WorldSetter.ECubeArea.XP);
        SetAreaInfo(WorldSetter.ECubeArea.ZP, WorldSetter.ECubeArea.XM_ZP, Vector3.back, WorldSetter.ECubeArea.XM);
        SetAreaInfo(WorldSetter.ECubeArea.ZP, WorldSetter.ECubeArea.YP_ZP, Vector3.back, WorldSetter.ECubeArea.YP);
        SetAreaInfo(WorldSetter.ECubeArea.ZP, WorldSetter.ECubeArea.YM_ZP, Vector3.back, WorldSetter.ECubeArea.YM);

        SetAreaInfo(WorldSetter.ECubeArea.ZM, WorldSetter.ECubeArea.XP_ZM, Vector3.forward, WorldSetter.ECubeArea.XP);
        SetAreaInfo(WorldSetter.ECubeArea.ZM, WorldSetter.ECubeArea.XM_ZM, Vector3.forward, WorldSetter.ECubeArea.XM);
        SetAreaInfo(WorldSetter.ECubeArea.ZM, WorldSetter.ECubeArea.YP_ZM, Vector3.forward, WorldSetter.ECubeArea.YP);
        SetAreaInfo(WorldSetter.ECubeArea.ZM, WorldSetter.ECubeArea.YM_ZM, Vector3.forward, WorldSetter.ECubeArea.YM);
    }

    private void InitCubeAreaGravity()
    {
        this.m_cubeAreaGravity[(uint)WorldSetter.ECubeArea.XP] = Vector3.left * Physics.gravity.magnitude;
        this.m_cubeAreaGravity[(uint)WorldSetter.ECubeArea.XM] = Vector3.right * Physics.gravity.magnitude;
        this.m_cubeAreaGravity[(uint)WorldSetter.ECubeArea.YP] = Vector3.down * Physics.gravity.magnitude;
        this.m_cubeAreaGravity[(uint)WorldSetter.ECubeArea.YM] = Vector3.up * Physics.gravity.magnitude;
        this.m_cubeAreaGravity[(uint)WorldSetter.ECubeArea.ZP] = Vector3.back * Physics.gravity.magnitude;
        this.m_cubeAreaGravity[(uint)WorldSetter.ECubeArea.ZM] = Vector3.forward * Physics.gravity.magnitude;
    }

    public Vector3 GetCubeAreaGravity(WorldSetter.ECubeArea eCubeArea)
    {
        Vector3 areaGravity = Vector3.zero;

        areaGravity = this.m_cubeAreaGravity[(uint)eCubeArea];

        return areaGravity;
    }

    private void DoObjectLaunchTrigger(int objectIndex)
    {
        Vector3 launchForce = this.m_objects[objectIndex].launchDirection * this.m_objects[objectIndex].launchPower * kLaunchPower;
        this.m_objects[objectIndex].throwableObject.SetLayerAsThrown();  // Preventing CGThrowableObject from colliding with world barrier planes
        this.m_objects[objectIndex].objectRigidBody.AddForce(launchForce);
        this.m_objects[objectIndex].eObjectState = CGPhysicsObject.EObjectState.OBJECT_MOVING;
    }

    private void DoObjectMoving(int objectIndex)
    {
        WorldSetter.ECubeArea eCubeArea = this.m_worldSetter.GetCubeAreaEnum(this.m_objects[objectIndex].throwableObject.transform.position);
        Bounds areaWorldBound = this.m_worldSetter.GetCubeAreaBound(eCubeArea);
        Bounds objWorldBound = this.m_objects[objectIndex].throwableObject.GetComponent<Renderer>().bounds;
        bool isInside = WorldSetter.IsAInsideB(objWorldBound.min, objWorldBound.max, areaWorldBound.min, areaWorldBound.max);

        // Area가 한 번 바뀌었으므로, Target으로 가는 중이다.
        // Target Area에 있다면, TARGET_AREA state로 바꾸고, Update() 함수에서 없애준다.
        if (this.m_objects[objectIndex].isAreaChanged == true)
        {
            // 해당 target area에 넘어왔고, bound box가 완전히 들어왔다면
            // Layer 설정을 바꾸어서 plane과 충돌처리 되게 한다.
            if (this.m_objects[objectIndex].eTargetArea == eCubeArea && isInside)
            {
                this.m_objects[objectIndex].eObjectState = CGPhysicsObject.EObjectState.OBJECT_TARGET_AREA;
                this.m_objects[objectIndex].throwableObject.SetLayerAsInArea();  // In order to make CGThrowableObject collide with world barrier planes

                // 해당 지역의 gravity로 설정해준다.
                Vector3 targetGravity = this.m_cubeAreaGravity[(uint)eCubeArea];
                this.m_objects[objectIndex].throwableObject.SetCubeAreaGravity(targetGravity);
            }
        }
        else if (this.m_objects[objectIndex].eCurrentArea != eCubeArea)
        {
            // 해당 넘어가는 지역으로 완전히 들어 갔으면, Area Changed를 true로 해주고,
            // 해당 지역의 Force를 넘겨준다.
            if (isInside == true)
            {
                this.m_objects[objectIndex].isAreaChanged = true;
                this.m_objects[objectIndex].ePreviousArea = m_objects[objectIndex].eCurrentArea;
                this.m_objects[objectIndex].eCurrentArea = eCubeArea;
                this.m_objects[objectIndex].eTargetArea = m_targetArea[(uint)m_objects[objectIndex].ePreviousArea, (uint)m_objects[objectIndex].eCurrentArea];

                if (true == this.m_isValidAreaChange[(uint)m_objects[objectIndex].ePreviousArea, (uint)m_objects[objectIndex].eCurrentArea])
                {
                    // 다른 지역으로 넘어갔다.
                }
                else
                {
                    // PutBack the Throwable Object
                    this.m_objects[objectIndex].throwableObject.ResetObject();
                    this.m_objects.RemoveAt(objectIndex);
                }
            }
        }
    }

    private void FixedUpdatePhysicsWorld()
    {
        for (int objectIndex = 0; objectIndex < this.m_objects.Count; ++objectIndex)
        {
            switch (this.m_objects[objectIndex].eObjectState)
            {
                case CGPhysicsObject.EObjectState.OBJECT_LAUNCH_TRIGGER:
                    {
                        DoObjectLaunchTrigger(objectIndex);
                        break;
                    }
                case CGPhysicsObject.EObjectState.OBJECT_MOVING:
                    {
                        DoObjectMoving(objectIndex);
                        break;
                    }
            }
        }
    }

    private void UpdatePhysicsWorld()
    {
        for (int objectIndex = 0; objectIndex < this.m_objects.Count; ++objectIndex)
        {
            //switch (m_objects[objectIndex].eObjectState)
            //{
            //    case CGPhysicsObject.EObjectState.OBJECT_TARGET_AREA:
            //        {

            // 해당 area에 맞는 gravity 설정
            if (this.m_objects[objectIndex].throwableObject.GetComponent<CGThrowableObject>().CheckAndChangeObjectState() == true)
            {
                // 업데이트 할 PhysicsObject Array에서 제거
                this.m_objects.RemoveAt(objectIndex);
            }

            //            break;
            //        }
            //}
        }
    }

    private void Start()
    {
        InitPhysicsWorldManager();
        InitAreaInfo();
        InitCubeAreaGravity();
    }

    private void FixedUpdate()
    {
        FixedUpdatePhysicsWorld();
    }

    private void Update()
    {
        UpdatePhysicsWorld();
    }

    public void LaunchCGObject(CGThrowableObject obj, Vector3 direction, float power)
    {
        CGPhysicsObject physicsObj = new CGPhysicsObject(obj, direction, power, this.m_worldSetter);

        WorldSetter.ECubeArea eCubeArea = this.m_worldSetter.GetCubeAreaEnum(physicsObj.throwableObject.transform.position);
        Vector3 startGravity = this.m_cubeAreaGravity[(uint)eCubeArea];

        physicsObj.throwableObject.SetCubeAreaGravity(startGravity);
        this.m_objects.Add(physicsObj);
    }

    public int GetObservedObjectCount()
    {
        return this.m_objects.Count;
    }
}
