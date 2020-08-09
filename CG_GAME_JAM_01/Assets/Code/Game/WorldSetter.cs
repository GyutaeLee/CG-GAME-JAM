using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 현재 테스트 용도로, 월드에 필요한 오브젝트들을 세팅한다.
 */
public class WorldSetter : MonoBehaviour
{
    public enum ECubeArea : uint
    {
        XP, XM,
        YP, YM,
        ZP, ZM,

        XP_YP, XP_YM,
        XM_YP, XM_YM,

        XP_ZP, XP_ZM,
        XM_ZP, XM_ZM,

        YP_ZP, YP_ZM,
        YM_ZP, YM_ZM,

        XP_YP_ZP, XP_YP_ZM,

        XP_YM_ZP, XP_YM_ZM,

        XM_YP_ZP, XM_YP_ZM,

        XM_YM_ZP, XM_YM_ZM,

        NONE
    }

    // GUI
    public bool bPlaneDebuggingDisplayEnable;
    public bool bCubeAreaDebuggingEnable;
    // GUI

    private enum PlaneDirectionEnum : uint
    {
        IN = 0,
        OUT = 1
    }

    private enum WorldAreaEnum : uint
    {
        XPLUS = 0,
        XMINUS = 2,
        YPLUS = 4,
        YMINUS = 6,
        ZPLUS = 8,
        ZMINUS = 10
    }

    private int[,] m_areaEnumToCoordinate = new int[27, 3]
    {
        { 1, 0, 0 }, {-1, 0, 0 }, 
        { 0, 1, 0 }, {0, -1, 0 }, 
        { 0, 0, 1 }, {0, 0, -1 },

        { 1, 1, 0 }, { 1, -1, 0 }, 
        { -1, 1, 0 }, { -1, -1, 0 },

        { 1, 0, 1 }, { 1, 0, -1 },
        { -1, 0, 1 }, { -1, 0, -1 },

        { 0, 1, 1 }, { 0, 1, -1},
        { 0, -1, 1 }, {0, -1, -1},

        { 1, 1, 1 }, {1, 1, -1},
        
        { 1, -1, 1 }, { 1, -1, -1 },

        {-1, 1, 1 }, { -1, 1, -1},
        
        { -1, -1, 1 }, {-1, -1, -1},

        { 0, 0, 0 }
    };

    private GameObject m_emptyGameObjectPrefab;     // Plane GameObject 생성을 위한 Empty Prefab
    private GameObject m_cubeWorldGameObject;       // "CubeWorld"라는 GameObject를 찾는다. 이것은 CubeWorld가 시작하는 GameObject Root가 된다.

    private Transform m_cubeWorldTransform;
    private Transform m_worldSetterTransform;

    // Area Infomation : GetCubeAreaEnum / GetCubeAreaBound methods 참조
    private ECubeArea[,,] m_eAreaEnumLUT;
    private Bounds[,,] m_areaBound;

    // Planes : 현재 디버깅 및 Cube Area간의 충돌 탐지를 위해 사용된다.
    private Vector3[] m_planeRelativePos;
    private Quaternion[] m_planeRot;
    private GameObject[] m_prefabPlaneArea;
    private Color[] m_planeColor;

    // LightingSetting : 각 Plane마다 Directional Light를 주기 위해 사용된다.
    private GameObject[] m_worldLight;

    // Cube Area Setting : 디버깅 및 각 Area Cube 공간을 나타내기 위해 사용된다.
    private GameObject m_cubeAreaParent;
    private GameObject[] m_cubeAreaObject;

    /*
     * @brief : input은 각 axis에대해 -1, 0, 1이고, 그에 따라 CubeAreaEnum을 던져준다.
     * @details : ex) (1, 0, -1)은 XP_ZM을, (1, 1, -1)은 XP_YP_ZM을 던져준다.
     */
    private ECubeArea GetCubeAreaEnum(int x, int y, int z)
    {
        return m_eAreaEnumLUT[x + 1, y + 1, z + 1];
    }

    public ECubeArea GetCubeAreaEnum(Vector3 WorldPosition)
    {
        Vector3 cubeWorldPos = this.m_cubeWorldTransform.position;
        Vector3 cubeWorldExtent = this.m_cubeWorldTransform.lossyScale / 2f;

        Vector3 cubeWorldMin = cubeWorldPos - cubeWorldExtent;
        Vector3 cubeWorldMax = cubeWorldPos + cubeWorldExtent;

        int bx = 0;
        if (WorldPosition.x > cubeWorldMax.x)
            bx = 1;
        else if (WorldPosition.x < cubeWorldMin.x)
            bx = -1;

        int by = 0;
        if (WorldPosition.y > cubeWorldMax.y)
            by = 1;
        else if (WorldPosition.y < cubeWorldMin.y)
            by = -1;

        int bz = 0;
        if (WorldPosition.z > cubeWorldMax.z)
            bz = 1;
        else if (WorldPosition.z < cubeWorldMin.z)
            bz = -1;

        return GetCubeAreaEnum(bx, by, bz);
    }

    public Bounds GetCubeAreaBound(ECubeArea area)
    {
        int x = this.m_areaEnumToCoordinate[(uint)area, 0];
        int y = this.m_areaEnumToCoordinate[(uint)area, 1];
        int z = this.m_areaEnumToCoordinate[(uint)area, 2];
        return this.m_areaBound[x + 1, y + 1, z + 1];
    }
    
    /*
     *  @brief : AABB A가 AABB B안에 있는가? 
     */
    public static bool IsAInsideB(Vector3 minA, Vector3 maxA, Vector3 minB, Vector3 maxB)
    {
        if(minB.x <= minA.x &&
            minB.y <= minA.y &&
            minB.z <= minA.z &&
            maxA.x <= maxB.x &&
            maxA.y <= maxB.y &&
            maxA.z <= maxB.z
            )
        {
            return true;
        }

        return false;
    }

    private void InitWorldSetter()
    {
        this.m_eAreaEnumLUT = new ECubeArea[3, 3, 3];
        this.m_areaBound = new Bounds[3, 3, 3];

        this.m_emptyGameObjectPrefab = Resources.Load("Prefab/GameObject") as GameObject;

        this.m_worldSetterTransform = GetComponent<Transform>();

        this.m_cubeWorldTransform = this.transform.Find("CubeWorld");
        this.m_cubeWorldGameObject = this.m_cubeWorldTransform.gameObject;
    }

    private void InitDebuggingPlanes()
    {
        this.m_planeRelativePos = new Vector3[12];
        this.m_planeRot = new Quaternion[12];
        this.m_prefabPlaneArea = new GameObject[12];
        this.m_planeColor = new Color[12];

        // plane pos and rot init for debugging
        const float posMove = 0.52f;
        const float planeAlphaValue = 0.3f;
        this.m_planeRelativePos[(uint)WorldAreaEnum.XPLUS] = new Vector3(posMove, 0f, 0f);
        this.m_planeRot[(uint)WorldAreaEnum.XPLUS + (uint)PlaneDirectionEnum.IN] = Quaternion.Euler(0f, 0f, 90f);
        this.m_planeColor[(uint)WorldAreaEnum.XPLUS + (uint)PlaneDirectionEnum.IN] = new Color(1f, 0f, 0f, planeAlphaValue);

        this.m_planeRelativePos[(uint)WorldAreaEnum.XPLUS + 1] = new Vector3(posMove, 0f, 0f);
        this.m_planeRot[(uint)WorldAreaEnum.XPLUS + (uint)PlaneDirectionEnum.OUT] = Quaternion.Euler(0f, 0f, -90f);
        this.m_planeColor[(uint)WorldAreaEnum.XPLUS + (uint)PlaneDirectionEnum.OUT] = new Color(1f, 0f, 0f, planeAlphaValue);

        this.m_planeRelativePos[(uint)WorldAreaEnum.XMINUS] = new Vector3(-posMove, 0f, 0f);
        this.m_planeRot[(uint)WorldAreaEnum.XMINUS + (uint)PlaneDirectionEnum.IN] = Quaternion.Euler(0f, 0f, -90f);
        this.m_planeColor[(uint)WorldAreaEnum.XMINUS + (uint)PlaneDirectionEnum.IN] = new Color(1f, 0f, 0f, planeAlphaValue);

        this.m_planeRelativePos[(uint)WorldAreaEnum.XMINUS + 1] = new Vector3(-posMove, 0f, 0f);
        this.m_planeRot[(uint)WorldAreaEnum.XMINUS + (uint)PlaneDirectionEnum.OUT] = Quaternion.Euler(0f, 0f, 90f);
        this.m_planeColor[(uint)WorldAreaEnum.XMINUS + (uint)PlaneDirectionEnum.OUT] = new Color(1f, 0f, 0f, planeAlphaValue);

        this.m_planeRelativePos[(uint)WorldAreaEnum.YPLUS] = new Vector3(0f, posMove, 0f);
        this.m_planeRot[(uint)WorldAreaEnum.YPLUS + (uint)PlaneDirectionEnum.IN] = Quaternion.Euler(0f, 0f, 180f);
        this.m_planeColor[(uint)WorldAreaEnum.YPLUS + (uint)PlaneDirectionEnum.IN] = new Color(0f, 1f, 0f, planeAlphaValue);

        this.m_planeRelativePos[(uint)WorldAreaEnum.YPLUS + 1] = new Vector3(0f, posMove, 0f);
        this.m_planeRot[(uint)WorldAreaEnum.YPLUS + (uint)PlaneDirectionEnum.OUT] = Quaternion.Euler(0f, 0f, 0f);
        this.m_planeColor[(uint)WorldAreaEnum.YPLUS + (uint)PlaneDirectionEnum.OUT] = new Color(0f, 1f, 0f, planeAlphaValue);

        this.m_planeRelativePos[(uint)WorldAreaEnum.YMINUS] = new Vector3(0f, -posMove, 0f);
        this.m_planeRot[(uint)WorldAreaEnum.YMINUS + (uint)PlaneDirectionEnum.IN] = Quaternion.Euler(0f, 0f, 0f);
        this.m_planeColor[(uint)WorldAreaEnum.YMINUS + (uint)PlaneDirectionEnum.IN] = new Color(0f, 1f, 0f, planeAlphaValue);

        this.m_planeRelativePos[(uint)WorldAreaEnum.YMINUS + 1] = new Vector3(0f, -posMove, 0f);
        this.m_planeRot[(uint)WorldAreaEnum.YMINUS + (uint)PlaneDirectionEnum.OUT] = Quaternion.Euler(0f, 0f, 180f);
        this.m_planeColor[(uint)WorldAreaEnum.YMINUS + (uint)PlaneDirectionEnum.OUT] = new Color(0f, 1f, 0f, planeAlphaValue);

        this.m_planeRelativePos[(uint)WorldAreaEnum.ZPLUS] = new Vector3(0f, 0f, posMove);
        this.m_planeRot[(uint)WorldAreaEnum.ZPLUS + (uint)PlaneDirectionEnum.IN] = Quaternion.Euler(90f, 0f, 0f);
        this.m_planeColor[(uint)WorldAreaEnum.ZPLUS + (uint)PlaneDirectionEnum.IN] = new Color(0f, 0f, 1f, planeAlphaValue);

        this.m_planeRelativePos[(uint)WorldAreaEnum.ZPLUS + 1] = new Vector3(0f, 0f, posMove);
        this.m_planeRot[(uint)WorldAreaEnum.ZPLUS + (uint)PlaneDirectionEnum.OUT] = Quaternion.Euler(-90f, 0f, 0f);
        this.m_planeColor[(uint)WorldAreaEnum.ZPLUS + (uint)PlaneDirectionEnum.OUT] = new Color(0f, 0f, 1f, planeAlphaValue);

        this.m_planeRelativePos[(uint)WorldAreaEnum.ZMINUS] = new Vector3(0f, 0, -posMove);
        this.m_planeRot[(uint)WorldAreaEnum.ZMINUS + (uint)PlaneDirectionEnum.IN] = Quaternion.Euler(-90f, 0f, 0f);
        this.m_planeColor[(uint)WorldAreaEnum.ZMINUS + (uint)PlaneDirectionEnum.IN] = new Color(0f, 0f, 1f, planeAlphaValue);

        this.m_planeRelativePos[(uint)WorldAreaEnum.ZMINUS + 1] = new Vector3(0f, 0f, -posMove);
        this.m_planeRot[(uint)WorldAreaEnum.ZMINUS + (uint)PlaneDirectionEnum.OUT] = Quaternion.Euler(90f, 0f, 0f);
        this.m_planeColor[(uint)WorldAreaEnum.ZMINUS + (uint)PlaneDirectionEnum.OUT] = new Color(0f, 0f, 1f, planeAlphaValue);

        Material planeMat = Resources.Load("Prefab/PlaneTransparency") as Material;
        GameObject prefab = Resources.Load("Prefab/pAreaPlane") as GameObject;

        GameObject planeParent = GameObject.Instantiate(this.m_emptyGameObjectPrefab, this.m_cubeWorldTransform);
        planeParent.name = "PlaneParent";
        Transform planeParentTransform = planeParent.transform;

        int worldBarrierLayerIndex = LayerMask.NameToLayer("CGWorldBarrier");
        for (int i = 0; i < 12; ++i)
        {
            this.m_prefabPlaneArea[i] = GameObject.Instantiate(prefab, planeParentTransform);
            this.m_prefabPlaneArea[i].GetComponent<Renderer>().enabled = this.bPlaneDebuggingDisplayEnable;
            this.m_prefabPlaneArea[i].layer = worldBarrierLayerIndex;

            // Basic Setting
            WorldAreaEnum ew = ((i % 2) == 0 ? (WorldAreaEnum)i : (WorldAreaEnum)(i - 1));
            PlaneDirectionEnum ep = (PlaneDirectionEnum)(i % 2);
            this.m_prefabPlaneArea[i].name = ew.ToString() + '_' + ep.ToString();
            this.m_prefabPlaneArea[i].transform.localPosition = this.m_planeRelativePos[i];
            this.m_prefabPlaneArea[i].transform.localRotation = this.m_planeRot[i];

            // Material Setting
            MeshRenderer mr = this.m_prefabPlaneArea[i].GetComponent<MeshRenderer>();
            mr.material = Material.Instantiate(planeMat);
            mr.material.SetColor("_Color", this.m_planeColor[i]);

            // Plane Collider Setting for preventing from a character moving to another cube face
            Rigidbody rb = this.m_prefabPlaneArea[i].AddComponent<Rigidbody>();
            rb.mass = 100f;
            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }
        planeParent.SetActive(true);
    }
    
    private void InitLightSetting()
    {
        this.m_worldLight = new GameObject[6];

        // Lighting Setting
        Quaternion[] lightRot = new Quaternion[6];
        lightRot[0] = Quaternion.Euler(0f, -90f, 0f);    // X+
        lightRot[1] = Quaternion.Euler(0f, 90f, 0f);   // X-
        lightRot[2] = Quaternion.Euler(90f, 0f, 0f);   // Y+
        lightRot[3] = Quaternion.Euler(-90f, 0f, 0f);    // Y-
        lightRot[4] = Quaternion.Euler(180f, 0f, 0f);   // Z+
        lightRot[5] = Quaternion.Euler(0f, 0f, 0f);     // Z-

        for (int i = 0; i < 6; ++i)
        {
            Vector3 lPos = this.m_planeRelativePos[i * 2] * 500f;
            this.m_worldLight[i] = GameObject.Instantiate(this.m_emptyGameObjectPrefab, lPos, lightRot[i], this.m_worldSetterTransform);

            WorldAreaEnum ew = (WorldAreaEnum)(i * 2);
            this.m_worldLight[i].name = ew.ToString() + "_Light";

            Light light = this.m_worldLight[i].AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;
            light.shadows = LightShadows.None;
        }
    }

    private void SetCubeAreaGameObject(ref GameObject go, string goName, Vector3 pos, Vector3 scale, ref Material resourceMat, Color colour)
    {
        go.name = goName;
        go.transform.localPosition = pos;
        go.transform.localScale = scale;

        MeshRenderer mr = go.GetComponent<MeshRenderer>();
        mr.material = Material.Instantiate(resourceMat);
        mr.material.SetColor("_Color", colour);
    }

    private void InitCubeAreaSetting()
    {
        this.m_cubeAreaObject = new GameObject[26];

        this.m_eAreaEnumLUT[0, 0, 0] = ECubeArea.XM_YM_ZM;
        this.m_eAreaEnumLUT[0, 0, 1] = ECubeArea.XM_YM;
        this.m_eAreaEnumLUT[0, 0, 2] = ECubeArea.XM_YM_ZP;

        this.m_eAreaEnumLUT[0, 1, 0] = ECubeArea.XM_ZM;
        this.m_eAreaEnumLUT[0, 1, 1] = ECubeArea.XM;
        this.m_eAreaEnumLUT[0, 1, 2] = ECubeArea.XM_ZP;

        this.m_eAreaEnumLUT[0, 2, 0] = ECubeArea.XM_YP_ZM;
        this.m_eAreaEnumLUT[0, 2, 1] = ECubeArea.XM_YP;
        this.m_eAreaEnumLUT[0, 2, 2] = ECubeArea.XM_YP_ZP;

        this.m_eAreaEnumLUT[1, 0, 0] = ECubeArea.YM_ZM;
        this.m_eAreaEnumLUT[1, 0, 1] = ECubeArea.YM;
        this.m_eAreaEnumLUT[1, 0, 2] = ECubeArea.YM_ZP;

        this.m_eAreaEnumLUT[1, 1, 0] = ECubeArea.ZM;
        this.m_eAreaEnumLUT[1, 1, 1] = ECubeArea.NONE;
        this.m_eAreaEnumLUT[1, 1, 2] = ECubeArea.ZP;

        this.m_eAreaEnumLUT[1, 2, 0] = ECubeArea.YP_ZM;
        this.m_eAreaEnumLUT[1, 2, 1] = ECubeArea.YP;
        this.m_eAreaEnumLUT[1, 2, 2] = ECubeArea.YP_ZP;

        this.m_eAreaEnumLUT[2, 0, 0] = ECubeArea.XP_YM_ZM;
        this.m_eAreaEnumLUT[2, 0, 1] = ECubeArea.XP_YM;
        this.m_eAreaEnumLUT[2, 0, 2] = ECubeArea.XP_YM_ZP;

        this.m_eAreaEnumLUT[2, 1, 0] = ECubeArea.XP_ZM;
        this.m_eAreaEnumLUT[2, 1, 1] = ECubeArea.XP;
        this.m_eAreaEnumLUT[2, 1, 2] = ECubeArea.XP_ZP;

        this.m_eAreaEnumLUT[2, 2, 0] = ECubeArea.XP_YP_ZM;
        this.m_eAreaEnumLUT[2, 2, 1] = ECubeArea.XP_YP;
        this.m_eAreaEnumLUT[2, 2, 2] = ECubeArea.XP_YP_ZP;

        const float cubeAreaAlphaValue = 0.3f;

        Material cubeAreaMat = Resources.Load("Prefab/CubeAreaMat") as Material;
        GameObject cubeAreaPrefab = Resources.Load("Prefab/pAreaCube") as GameObject;

        this.m_cubeAreaParent = GameObject.Instantiate(this.m_emptyGameObjectPrefab, this.m_cubeWorldTransform);
        this.m_cubeAreaParent.name = "CubeParent";
        this.m_cubeAreaParent.SetActive(this.bCubeAreaDebuggingEnable);

        for (int i = 0; i < this.m_cubeAreaObject.Length; ++i)
        {
            this.m_cubeAreaObject[i] = GameObject.Instantiate(cubeAreaPrefab, this.m_cubeAreaParent.transform);
        }

        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.XP], 
            "CubeArea_X+",
            new Vector3(1f, 0f, 0f), 
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 0f, 0f, cubeAreaAlphaValue));

        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.XM],
            "CubeArea_X-",
            new Vector3(-1f, 0f, 0f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 0f, 0f, cubeAreaAlphaValue));

        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.YP],
            "CubeArea_Y+",
            new Vector3(0f, 1f, 0f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(0f, 1f, 0f, cubeAreaAlphaValue));
        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.YM],
            "CubeArea_Y-",
            new Vector3(0f, -1f, 0f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(0f, 1f, 0f, cubeAreaAlphaValue));

        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.ZP],
            "CubeArea_Z+",
            new Vector3(0f, 0f, 1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(0f, 0f, 1f, cubeAreaAlphaValue));
        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.ZM],
            "CubeArea_Z-",
            new Vector3(0f, 0f, -1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(0f, 0f, 1f, cubeAreaAlphaValue));

        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.XP_YP],
            "CubeArea_X+Y+",
            new Vector3(1f, 1f, 0f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 0f, cubeAreaAlphaValue));
        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.XP_YM],
            "CubeArea_X+Y-",
            new Vector3(1f, -1f, 0f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 0f, cubeAreaAlphaValue));

        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.XM_YP],
            "CubeArea_X-Y+",
            new Vector3(-1f, 1f, 0f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 0f, cubeAreaAlphaValue));
        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.XM_YM],
            "CubeArea_X-Y-",
            new Vector3(-1f, -1f, 0f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 0f, cubeAreaAlphaValue));

        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.XP_ZP],
            "CubeArea_X+Z+",
            new Vector3(1f, 0f, 1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 0f, 1f, cubeAreaAlphaValue));
        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.XP_ZM],
            "CubeArea_X-Z+",
            new Vector3(1f, 0f, -1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 0f, 1f, cubeAreaAlphaValue));

        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.XM_ZP],
            "CubeArea_X-Z+",
            new Vector3(-1f, 0f, 1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 0f, 1f, cubeAreaAlphaValue));
        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.XM_ZM],
            "CubeArea_X-Z-",
            new Vector3(-1f, 0f, -1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 0f, 1f, cubeAreaAlphaValue));

        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.YP_ZP],
            "CubeArea_Y+Z+",
            new Vector3(0f, 1f, 1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(0f, 1f, 1f, cubeAreaAlphaValue));
        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.YP_ZM],
            "CubeArea_Y-Z+",
            new Vector3(0f, 1f, -1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(0f, 1f, 1f, cubeAreaAlphaValue));

        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.YM_ZP],
            "CubeArea_Y-Z+",
            new Vector3(0f, -1f, 1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(0f, 1f, 1f, cubeAreaAlphaValue));
        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.YM_ZM],
            "CubeArea_Y-Z-",
            new Vector3(0f, -1f, -1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(0f, 1f, 1f, cubeAreaAlphaValue));


        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.XP_YP_ZP],
            "CubeArea_X+Y+Z+",
            new Vector3(1f, 1f, 1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 1f, cubeAreaAlphaValue));
        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.XP_YP_ZM],
            "CubeArea_X+Y+Z-",
            new Vector3(1f, 1f, -1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 1f, cubeAreaAlphaValue));

        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.XP_YM_ZP],
            "CubeArea_X+Y-Z+",
            new Vector3(1f, -1f, 1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 1f, cubeAreaAlphaValue));
        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.XP_YM_ZM],
            "CubeArea_X+Y-Z-",
            new Vector3(1f, -1f, -1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 1f, cubeAreaAlphaValue));

        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.XM_YP_ZP],
            "CubeArea_X-Y+Z+",
            new Vector3(-1f, 1f, 1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 1f, cubeAreaAlphaValue));
        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.XM_YP_ZM],
            "CubeArea_X-Y+Z-",
            new Vector3(-1f, 1f, -1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 1f, cubeAreaAlphaValue));

        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.XM_YM_ZP],
            "CubeArea_X-Y-Z+",
            new Vector3(-1f, -1f, 1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 1f, cubeAreaAlphaValue));
        SetCubeAreaGameObject(ref this.m_cubeAreaObject[(uint)ECubeArea.XM_YM_ZM],
            "CubeArea_X-Y-Z-",
            new Vector3(-1f, -1f, -1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 1f, cubeAreaAlphaValue));

        for (int i = 0; i < this.m_cubeAreaObject.Length; ++i)
        {
            ECubeArea ae = (ECubeArea)i;

            int x = this.m_areaEnumToCoordinate[i, 0] + 1;
            int y = this.m_areaEnumToCoordinate[i, 1] + 1;
            int z = this.m_areaEnumToCoordinate[i, 2] + 1;

            // Get World Bound from Renderer
            this.m_areaBound[x, y, z] = this.m_cubeAreaObject[i].GetComponent<Renderer>().bounds;
        }
    }

    //?? 규태 : 게임매니저에서 순서 관리하기
    //void Start()
    void Awake()
    {
        InitWorldSetter();
        InitDebuggingPlanes();
        InitLightSetting();
        InitCubeAreaSetting();
    }
}
