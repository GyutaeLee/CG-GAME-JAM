using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsWorldManager : MonoBehaviour
{
    const float kLaunchPower = 30f;
    private WorldSetter m_worldSetter;
    private class CGPhysicsObject
    {
        public enum EObjectState
        {
            OBJECT_LAUNCH_TRIGGER,
            OBJECT_MOVING,
            OBJECT_TARGET_AREA
        }

        public CGPhysicsObject(CGThrowableObject o, Vector3 d, float p, WorldSetter ws)
        {
            WorldSetter.CubeAreaEnum ae = ws.GetCubeAreaEnum(o.transform.position);

            previousArea = ae;
            currentArea = ae;
            targetArea = WorldSetter.CubeAreaEnum.NONE;
            isAreaChanged = false;
            state = EObjectState.OBJECT_LAUNCH_TRIGGER;
            obj = o;
            rb = obj.GetComponent<Rigidbody>();
            direction = d;
            power = p;
        }

        public WorldSetter.CubeAreaEnum previousArea;
        public WorldSetter.CubeAreaEnum currentArea;
        public WorldSetter.CubeAreaEnum targetArea;
        public bool isAreaChanged;
        public EObjectState state;
        public CGThrowableObject obj;
        public Rigidbody rb;
        public Vector3 direction;
        public float power;
    }

    List<CGPhysicsObject> m_objects = new List<CGPhysicsObject>();
    public void LaunchCGObject(CGThrowableObject obj, Vector3 direction, float power)
    {
        CGPhysicsObject a = new CGPhysicsObject(obj, direction, power, m_worldSetter);
        m_objects.Add(a);
    }

    public int GetObservedObjectCount()
    {
        return m_objects.Count;
    }

    bool[,] m_canAreaChange = new bool[(uint)WorldSetter.CubeAreaEnum.NONE, (uint)WorldSetter.CubeAreaEnum.NONE];
    const float kForceSpacePower = 150f;
    Vector3[,] m_forceSpace = new Vector3[(uint)WorldSetter.CubeAreaEnum.NONE, (uint)WorldSetter.CubeAreaEnum.NONE];
    WorldSetter.CubeAreaEnum[,] m_targetArea = new WorldSetter.CubeAreaEnum[(uint)WorldSetter.CubeAreaEnum.NONE, (uint)WorldSetter.CubeAreaEnum.NONE];

    void SetAreaInfo(WorldSetter.CubeAreaEnum from, WorldSetter.CubeAreaEnum to, Vector3 force, WorldSetter.CubeAreaEnum target)
    {
        m_canAreaChange[(uint)from, (uint)to] = true;
        m_forceSpace[(uint)from, (uint)to] = force;
        m_targetArea[(uint)from, (uint)to] = target;
    }

    void Start()
    {
        m_worldSetter = GameObject.Find("Cube").GetComponent<WorldSetter>();

        // 초기화
        for (int i = 0; i < (uint)WorldSetter.CubeAreaEnum.NONE; ++i)
        {
            for (int j = 0; j < (uint)WorldSetter.CubeAreaEnum.NONE; ++j)
            {
                m_canAreaChange[i, j] = false;
                m_forceSpace[i, j] = new Vector3(0f, 0f, 0f);
                m_targetArea[i, j] = WorldSetter.CubeAreaEnum.NONE;
            }
        }

        SetAreaInfo(WorldSetter.CubeAreaEnum.XP, WorldSetter.CubeAreaEnum.XP_YP, Vector3.left, WorldSetter.CubeAreaEnum.YP);
        SetAreaInfo(WorldSetter.CubeAreaEnum.XP, WorldSetter.CubeAreaEnum.XP_YM, Vector3.left, WorldSetter.CubeAreaEnum.YM);
        SetAreaInfo(WorldSetter.CubeAreaEnum.XP, WorldSetter.CubeAreaEnum.XP_ZP, Vector3.left, WorldSetter.CubeAreaEnum.ZP);
        SetAreaInfo(WorldSetter.CubeAreaEnum.XP, WorldSetter.CubeAreaEnum.XP_ZM, Vector3.left, WorldSetter.CubeAreaEnum.ZM);

        SetAreaInfo(WorldSetter.CubeAreaEnum.XM, WorldSetter.CubeAreaEnum.XM_YP, Vector3.right, WorldSetter.CubeAreaEnum.YP);
        SetAreaInfo(WorldSetter.CubeAreaEnum.XM, WorldSetter.CubeAreaEnum.XM_YM, Vector3.right, WorldSetter.CubeAreaEnum.YM);
        SetAreaInfo(WorldSetter.CubeAreaEnum.XM, WorldSetter.CubeAreaEnum.XM_ZP, Vector3.right, WorldSetter.CubeAreaEnum.ZP);
        SetAreaInfo(WorldSetter.CubeAreaEnum.XM, WorldSetter.CubeAreaEnum.XM_ZM, Vector3.right, WorldSetter.CubeAreaEnum.ZM);

        SetAreaInfo(WorldSetter.CubeAreaEnum.YP, WorldSetter.CubeAreaEnum.XP_YP, Vector3.down, WorldSetter.CubeAreaEnum.XP);
        SetAreaInfo(WorldSetter.CubeAreaEnum.YP, WorldSetter.CubeAreaEnum.XM_YP, Vector3.down, WorldSetter.CubeAreaEnum.XM);
        SetAreaInfo(WorldSetter.CubeAreaEnum.YP, WorldSetter.CubeAreaEnum.YP_ZP, Vector3.down, WorldSetter.CubeAreaEnum.ZP);
        SetAreaInfo(WorldSetter.CubeAreaEnum.YP, WorldSetter.CubeAreaEnum.YP_ZM, Vector3.down, WorldSetter.CubeAreaEnum.ZM);

        SetAreaInfo(WorldSetter.CubeAreaEnum.YM, WorldSetter.CubeAreaEnum.XP_YM, Vector3.up, WorldSetter.CubeAreaEnum.XP);
        SetAreaInfo(WorldSetter.CubeAreaEnum.YM, WorldSetter.CubeAreaEnum.XM_YM, Vector3.up, WorldSetter.CubeAreaEnum.XM);
        SetAreaInfo(WorldSetter.CubeAreaEnum.YM, WorldSetter.CubeAreaEnum.YM_ZP, Vector3.up, WorldSetter.CubeAreaEnum.ZP);
        SetAreaInfo(WorldSetter.CubeAreaEnum.YM, WorldSetter.CubeAreaEnum.YM_ZM, Vector3.up, WorldSetter.CubeAreaEnum.ZM);

        SetAreaInfo(WorldSetter.CubeAreaEnum.ZP, WorldSetter.CubeAreaEnum.XP_ZP, Vector3.back, WorldSetter.CubeAreaEnum.XP);
        SetAreaInfo(WorldSetter.CubeAreaEnum.ZP, WorldSetter.CubeAreaEnum.XM_ZP, Vector3.back, WorldSetter.CubeAreaEnum.XM);
        SetAreaInfo(WorldSetter.CubeAreaEnum.ZP, WorldSetter.CubeAreaEnum.YP_ZP, Vector3.back, WorldSetter.CubeAreaEnum.YP);
        SetAreaInfo(WorldSetter.CubeAreaEnum.ZP, WorldSetter.CubeAreaEnum.YM_ZP, Vector3.back, WorldSetter.CubeAreaEnum.YM);

        SetAreaInfo(WorldSetter.CubeAreaEnum.ZM, WorldSetter.CubeAreaEnum.XP_ZM, Vector3.forward, WorldSetter.CubeAreaEnum.XP);
        SetAreaInfo(WorldSetter.CubeAreaEnum.ZM, WorldSetter.CubeAreaEnum.XM_ZM, Vector3.forward, WorldSetter.CubeAreaEnum.XM);
        SetAreaInfo(WorldSetter.CubeAreaEnum.ZM, WorldSetter.CubeAreaEnum.YP_ZM, Vector3.forward, WorldSetter.CubeAreaEnum.YP);
        SetAreaInfo(WorldSetter.CubeAreaEnum.ZM, WorldSetter.CubeAreaEnum.YM_ZM, Vector3.forward, WorldSetter.CubeAreaEnum.YM);
    }

    private void FixedUpdate()
    {
        for (int objectIndex = 0; objectIndex < m_objects.Count; ++objectIndex)
        {
            switch (m_objects[objectIndex].state)
            {
                case CGPhysicsObject.EObjectState.OBJECT_LAUNCH_TRIGGER:
                    {
                        Vector3 launchForce = m_objects[objectIndex].direction * m_objects[objectIndex].power * kLaunchPower;
                        m_objects[objectIndex].obj.SetLayerAsThrown();  // Preventing CGThrowableObject from colliding with world barrier planes
                        m_objects[objectIndex].rb.AddForce(launchForce);
                        m_objects[objectIndex].state = CGPhysicsObject.EObjectState.OBJECT_MOVING;
                        break;
                    }
                case CGPhysicsObject.EObjectState.OBJECT_MOVING:
                    {
                        WorldSetter.CubeAreaEnum ae = m_worldSetter.GetCubeAreaEnum(m_objects[objectIndex].obj.transform.position);
                        Bounds areaWorldBound = m_worldSetter.GetCubeAreaBound(ae);
                        Bounds objWorldBound = m_objects[objectIndex].obj.GetComponent<Renderer>().bounds;
                        bool isInside = WorldSetter.IsAInsideB(objWorldBound.min, objWorldBound.max, areaWorldBound.min, areaWorldBound.max);

                        // Area가 한 번 바뀌었으므로, Target으로 가는 중이다.
                        // Target Area에 있다면, TARGET_AREA state로 바꾸고, Update() 함수에서 없애준다.
                        if (m_objects[objectIndex].isAreaChanged == true)
                        {
                            // 해당 target area에 넘어왔고, bound box가 완전히 들어왔다면
                            // Layer 설정을 바꾸어서 plane과 충돌처리 되게 한다.
                            if (m_objects[objectIndex].targetArea == ae && isInside)
                            {
                                m_objects[objectIndex].state = CGPhysicsObject.EObjectState.OBJECT_TARGET_AREA;
                                m_objects[objectIndex].obj.SetLayerAsInArea();  // In order to make CGThrowableObject collide with world barrier planes
                            }

                            continue;
                        }

                        if (m_objects[objectIndex].currentArea != ae)
                        {
                            // 해당 넘어가는 지역으로 완전히 들어 갔으면, Area Changed를 true로 해주고,
                            // 해당 지역의 Force를 넘겨준다.
                            if(isInside)
                            {
                                m_objects[objectIndex].isAreaChanged = true;
                                m_objects[objectIndex].previousArea = m_objects[objectIndex].currentArea;
                                m_objects[objectIndex].currentArea = ae;
                                m_objects[objectIndex].targetArea = m_targetArea[(uint)m_objects[objectIndex].previousArea, (uint)m_objects[objectIndex].currentArea];

                                if (m_canAreaChange[(uint)m_objects[objectIndex].previousArea, (uint)m_objects[objectIndex].currentArea] == true)
                                {
                                    m_objects[objectIndex].rb.AddForce(m_forceSpace[(uint)m_objects[objectIndex].previousArea, (uint)m_objects[objectIndex].currentArea] * kForceSpacePower);
                                }
                                else
                                {
                                    // PutBack the Throwable Object
                                    m_objects[objectIndex].obj.ResetObject();
                                }
                            }
                        }

                        break;
                    }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int objectIndex = 0; objectIndex < m_objects.Count; ++objectIndex)
        {
            switch (m_objects[objectIndex].state)
            {
                case CGPhysicsObject.EObjectState.OBJECT_TARGET_AREA:
                    {
                        // 해당 area에 맞는 gravity 설정
                        if (m_objects[objectIndex].obj.GetComponent<CGThrowableObject>().CheckAndChangeObjectState() == true)
                        {
                            // 업데이트 할 PhysicsObject Array에서 제거
                            m_objects.RemoveAt(objectIndex);
                        }
                        break;
                    }
            }
        }
    }
}
