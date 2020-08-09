﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 현재 테스트 용도로, 월드에 필요한 오브젝트들을 세팅한다.
 */
public class WorldSetter : MonoBehaviour
{
    private GameObject m_emptyGameObjectPrefab;

    private Transform m_worldSetterTransform;

    private Transform m_cubeWorldTransform;
    private GameObject m_cubeWorldGameObject;

    // GUI
    public bool PlaneDebuggingEnable = true;
    public bool PlaneDebuggingDisplayEnable = true;
    public bool CubeAreaDebuggingEnable = true;
    // GUI

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


    private ECubeArea[,,] m_areaEnumLUT = new ECubeArea[3, 3, 3];

    /*
     * @brief : input은 각 axis에대해 -1, 0, 1이고, 그에 따라 CubeAreaEnum을 던져준다.
     * @details : ex) (1, 0, -1)은 XP_ZM을, (1, 1, -1)은 XP_YP_ZM을 던져준다.
     */
    private ECubeArea GetCubeAreaEnum(int x, int y, int z)
    {
        return m_areaEnumLUT[x + 1, y + 1, z + 1];
    }

    public ECubeArea GetCubeAreaEnum(Vector3 WorldPosition)
    {
        Vector3 cubeWorldPos = m_cubeWorldTransform.position;
        Vector3 cubeWorldExtent = m_cubeWorldTransform.lossyScale / 2f;

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

    private Bounds[,,] m_areaBound = new Bounds[3, 3, 3];

    public Bounds GetCubeAreaBound(ECubeArea area)
    {
        int x = m_areaEnumToCoordinate[(uint)area, 0];
        int y = m_areaEnumToCoordinate[(uint)area, 1];
        int z = m_areaEnumToCoordinate[(uint)area, 2];
        return m_areaBound[x + 1, y + 1, z + 1];
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

    enum PlaneDirectionEnum : uint
    {
        IN = 0,
        OUT = 1
    }

    enum WorldAreaEnum : uint
    {
        XPLUS = 0,
        XMINUS = 2,
        YPLUS = 4,
        YMINUS = 6,
        ZPLUS = 8,
        ZMINUS = 10
    }

    Vector3[] m_planeRelativePos = new Vector3[12];
    Quaternion[] m_planeRot = new Quaternion[12];
    GameObject[] m_prefabPlaneArea = new GameObject[12];
    Color[] m_planeColor = new Color[12];
    void initDebuggingPlanes()
    {
        // plane pos and rot init for debugging
        const float posMove = 0.52f;
        const float planeAlphaValue = 0.3f;
        m_planeRelativePos[(uint)WorldAreaEnum.XPLUS] = new Vector3(posMove, 0f, 0f);
        m_planeRot[(uint)WorldAreaEnum.XPLUS + (uint)PlaneDirectionEnum.IN] = Quaternion.Euler(0f, 0f, 90f);
        m_planeColor[(uint)WorldAreaEnum.XPLUS + (uint)PlaneDirectionEnum.IN] = new Color(1f, 0f, 0f, planeAlphaValue);

        m_planeRelativePos[(uint)WorldAreaEnum.XPLUS + 1] = new Vector3(posMove, 0f, 0f);
        m_planeRot[(uint)WorldAreaEnum.XPLUS + (uint)PlaneDirectionEnum.OUT] = Quaternion.Euler(0f, 0f, -90f);
        m_planeColor[(uint)WorldAreaEnum.XPLUS + (uint)PlaneDirectionEnum.OUT] = new Color(1f, 0f, 0f, planeAlphaValue);

        m_planeRelativePos[(uint)WorldAreaEnum.XMINUS] = new Vector3(-posMove, 0f, 0f);
        m_planeRot[(uint)WorldAreaEnum.XMINUS + (uint)PlaneDirectionEnum.IN] = Quaternion.Euler(0f, 0f, -90f);
        m_planeColor[(uint)WorldAreaEnum.XMINUS + (uint)PlaneDirectionEnum.IN] = new Color(1f, 0f, 0f, planeAlphaValue);

        m_planeRelativePos[(uint)WorldAreaEnum.XMINUS + 1] = new Vector3(-posMove, 0f, 0f);
        m_planeRot[(uint)WorldAreaEnum.XMINUS + (uint)PlaneDirectionEnum.OUT] = Quaternion.Euler(0f, 0f, 90f);
        m_planeColor[(uint)WorldAreaEnum.XMINUS + (uint)PlaneDirectionEnum.OUT] = new Color(1f, 0f, 0f, planeAlphaValue);

        m_planeRelativePos[(uint)WorldAreaEnum.YPLUS] = new Vector3(0f, posMove, 0f);
        m_planeRot[(uint)WorldAreaEnum.YPLUS + (uint)PlaneDirectionEnum.IN] = Quaternion.Euler(0f, 0f, 180f);
        m_planeColor[(uint)WorldAreaEnum.YPLUS + (uint)PlaneDirectionEnum.IN] = new Color(0f, 1f, 0f, planeAlphaValue);

        m_planeRelativePos[(uint)WorldAreaEnum.YPLUS + 1] = new Vector3(0f, posMove, 0f);
        m_planeRot[(uint)WorldAreaEnum.YPLUS + (uint)PlaneDirectionEnum.OUT] = Quaternion.Euler(0f, 0f, 0f);
        m_planeColor[(uint)WorldAreaEnum.YPLUS + (uint)PlaneDirectionEnum.OUT] = new Color(0f, 1f, 0f, planeAlphaValue);

        m_planeRelativePos[(uint)WorldAreaEnum.YMINUS] = new Vector3(0f, -posMove, 0f);
        m_planeRot[(uint)WorldAreaEnum.YMINUS + (uint)PlaneDirectionEnum.IN] = Quaternion.Euler(0f, 0f, 0f);
        m_planeColor[(uint)WorldAreaEnum.YMINUS + (uint)PlaneDirectionEnum.IN] = new Color(0f, 1f, 0f, planeAlphaValue);

        m_planeRelativePos[(uint)WorldAreaEnum.YMINUS + 1] = new Vector3(0f, -posMove, 0f);
        m_planeRot[(uint)WorldAreaEnum.YMINUS + (uint)PlaneDirectionEnum.OUT] = Quaternion.Euler(0f, 0f, 180f);
        m_planeColor[(uint)WorldAreaEnum.YMINUS + (uint)PlaneDirectionEnum.OUT] = new Color(0f, 1f, 0f, planeAlphaValue);

        m_planeRelativePos[(uint)WorldAreaEnum.ZPLUS] = new Vector3(0f, 0f, posMove);
        m_planeRot[(uint)WorldAreaEnum.ZPLUS + (uint)PlaneDirectionEnum.IN] = Quaternion.Euler(90f, 0f, 0f);
        m_planeColor[(uint)WorldAreaEnum.ZPLUS + (uint)PlaneDirectionEnum.IN] = new Color(0f, 0f, 1f, planeAlphaValue);

        m_planeRelativePos[(uint)WorldAreaEnum.ZPLUS + 1] = new Vector3(0f, 0f, posMove);
        m_planeRot[(uint)WorldAreaEnum.ZPLUS + (uint)PlaneDirectionEnum.OUT] = Quaternion.Euler(-90f, 0f, 0f);
        m_planeColor[(uint)WorldAreaEnum.ZPLUS + (uint)PlaneDirectionEnum.OUT] = new Color(0f, 0f, 1f, planeAlphaValue);

        m_planeRelativePos[(uint)WorldAreaEnum.ZMINUS] = new Vector3(0f, 0, -posMove);
        m_planeRot[(uint)WorldAreaEnum.ZMINUS + (uint)PlaneDirectionEnum.IN] = Quaternion.Euler(-90f, 0f, 0f);
        m_planeColor[(uint)WorldAreaEnum.ZMINUS + (uint)PlaneDirectionEnum.IN] = new Color(0f, 0f, 1f, planeAlphaValue);

        m_planeRelativePos[(uint)WorldAreaEnum.ZMINUS + 1] = new Vector3(0f, 0f, -posMove);
        m_planeRot[(uint)WorldAreaEnum.ZMINUS + (uint)PlaneDirectionEnum.OUT] = Quaternion.Euler(90f, 0f, 0f);
        m_planeColor[(uint)WorldAreaEnum.ZMINUS + (uint)PlaneDirectionEnum.OUT] = new Color(0f, 0f, 1f, planeAlphaValue);

        Material planeMat = Resources.Load("Prefab/PlaneTransparency") as Material;
        GameObject prefab = Resources.Load("Prefab/pAreaPlane") as GameObject;

        GameObject planeParent = GameObject.Instantiate(m_emptyGameObjectPrefab, m_cubeWorldTransform);
        planeParent.name = "PlaneParent";
        Transform planeParentTransform = planeParent.transform;

        int worldBarrierLayerIndex = LayerMask.NameToLayer("CGWorldBarrier");
        for (int i = 0; i < 12; ++i)
        {
            m_prefabPlaneArea[i] = GameObject.Instantiate(prefab, planeParentTransform);
            m_prefabPlaneArea[i].GetComponent<Renderer>().enabled = PlaneDebuggingDisplayEnable;
            m_prefabPlaneArea[i].layer = worldBarrierLayerIndex;

            // Basic Setting
            WorldAreaEnum ew = ((i % 2) == 0 ? (WorldAreaEnum)i : (WorldAreaEnum)(i - 1));
            PlaneDirectionEnum ep = (PlaneDirectionEnum)(i % 2);
            m_prefabPlaneArea[i].name = ew.ToString() + '_' + ep.ToString();
            m_prefabPlaneArea[i].transform.localPosition = m_planeRelativePos[i];
            m_prefabPlaneArea[i].transform.localRotation = m_planeRot[i];

            // Material Setting
            MeshRenderer mr = m_prefabPlaneArea[i].GetComponent<MeshRenderer>();
            mr.material = Material.Instantiate(planeMat);
            mr.material.SetColor("_Color", m_planeColor[i]);

            // Plane Collider Setting for preventing from a character moving to another cube face
            Rigidbody rb = m_prefabPlaneArea[i].AddComponent<Rigidbody>();
            rb.mass = 100f;
            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }
        planeParent.SetActive(PlaneDebuggingEnable);
    }

    GameObject[] m_worldLight = new GameObject[6];
    void initLightSetting()
    {
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
            Vector3 lPos = m_planeRelativePos[i * 2] * 500f;
            m_worldLight[i] = GameObject.Instantiate(m_emptyGameObjectPrefab, lPos, lightRot[i], m_worldSetterTransform);

            WorldAreaEnum ew = (WorldAreaEnum)(i * 2);
            m_worldLight[i].name = ew.ToString() + "_Light";

            Light light = m_worldLight[i].AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;
            light.shadows = LightShadows.None;
        }
    }

    GameObject m_cubeAreaParent;
    GameObject[] m_cubeAreaObject = new GameObject[26];
    void setCubeAreaGameObject(ref GameObject go, string goName, Vector3 pos, Vector3 scale, ref Material resourceMat, Color colour)
    {
        go.name = goName;
        go.transform.localPosition = pos;
        go.transform.localScale = scale;

        MeshRenderer mr = go.GetComponent<MeshRenderer>();
        mr.material = Material.Instantiate(resourceMat);
        mr.material.SetColor("_Color", colour);
    }

    void initCubeAreaSetting()
    {
        m_areaEnumLUT[0, 0, 0] = ECubeArea.XM_YM_ZM;
        m_areaEnumLUT[0, 0, 1] = ECubeArea.XM_YM;
        m_areaEnumLUT[0, 0, 2] = ECubeArea.XM_YM_ZP;

        m_areaEnumLUT[0, 1, 0] = ECubeArea.XM_ZM;
        m_areaEnumLUT[0, 1, 1] = ECubeArea.XM;
        m_areaEnumLUT[0, 1, 2] = ECubeArea.XM_ZP;

        m_areaEnumLUT[0, 2, 0] = ECubeArea.XM_YP_ZM;
        m_areaEnumLUT[0, 2, 1] = ECubeArea.XM_YP;
        m_areaEnumLUT[0, 2, 2] = ECubeArea.XM_YP_ZP;

        m_areaEnumLUT[1, 0, 0] = ECubeArea.YM_ZM;
        m_areaEnumLUT[1, 0, 1] = ECubeArea.YM;
        m_areaEnumLUT[1, 0, 2] = ECubeArea.YM_ZP;

        m_areaEnumLUT[1, 1, 0] = ECubeArea.ZM;
        m_areaEnumLUT[1, 1, 1] = ECubeArea.NONE;
        m_areaEnumLUT[1, 1, 2] = ECubeArea.ZP;

        m_areaEnumLUT[1, 2, 0] = ECubeArea.YP_ZM;
        m_areaEnumLUT[1, 2, 1] = ECubeArea.YP;
        m_areaEnumLUT[1, 2, 2] = ECubeArea.YP_ZP;

        m_areaEnumLUT[2, 0, 0] = ECubeArea.XP_YM_ZM;
        m_areaEnumLUT[2, 0, 1] = ECubeArea.XP_YM;
        m_areaEnumLUT[2, 0, 2] = ECubeArea.XP_YM_ZP;

        m_areaEnumLUT[2, 1, 0] = ECubeArea.XP_ZM;
        m_areaEnumLUT[2, 1, 1] = ECubeArea.XP;
        m_areaEnumLUT[2, 1, 2] = ECubeArea.XP_ZP;

        m_areaEnumLUT[2, 2, 0] = ECubeArea.XP_YP_ZM;
        m_areaEnumLUT[2, 2, 1] = ECubeArea.XP_YP;
        m_areaEnumLUT[2, 2, 2] = ECubeArea.XP_YP_ZP;

        const float cubeAreaAlphaValue = 0.3f;

        Material cubeAreaMat = Resources.Load("Prefab/CubeAreaMat") as Material;
        GameObject cubeAreaPrefab = Resources.Load("Prefab/pAreaCube") as GameObject;

        m_cubeAreaParent = GameObject.Instantiate(m_emptyGameObjectPrefab, m_cubeWorldTransform);
        m_cubeAreaParent.name = "CubeParent";
        m_cubeAreaParent.SetActive(CubeAreaDebuggingEnable);

        for (int i = 0; i < m_cubeAreaObject.Length; ++i)
        {
            m_cubeAreaObject[i] = GameObject.Instantiate(cubeAreaPrefab, m_cubeAreaParent.transform);
        }

        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.XP], 
            "CubeArea_X+",
            new Vector3(1f, 0f, 0f), 
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 0f, 0f, cubeAreaAlphaValue));

        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.XM],
            "CubeArea_X-",
            new Vector3(-1f, 0f, 0f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 0f, 0f, cubeAreaAlphaValue));

        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.YP],
            "CubeArea_Y+",
            new Vector3(0f, 1f, 0f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(0f, 1f, 0f, cubeAreaAlphaValue));
        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.YM],
            "CubeArea_Y-",
            new Vector3(0f, -1f, 0f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(0f, 1f, 0f, cubeAreaAlphaValue));

        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.ZP],
            "CubeArea_Z+",
            new Vector3(0f, 0f, 1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(0f, 0f, 1f, cubeAreaAlphaValue));
        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.ZM],
            "CubeArea_Z-",
            new Vector3(0f, 0f, -1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(0f, 0f, 1f, cubeAreaAlphaValue));

        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.XP_YP],
            "CubeArea_X+Y+",
            new Vector3(1f, 1f, 0f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 0f, cubeAreaAlphaValue));
        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.XP_YM],
            "CubeArea_X+Y-",
            new Vector3(1f, -1f, 0f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 0f, cubeAreaAlphaValue));

        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.XM_YP],
            "CubeArea_X-Y+",
            new Vector3(-1f, 1f, 0f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 0f, cubeAreaAlphaValue));
        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.XM_YM],
            "CubeArea_X-Y-",
            new Vector3(-1f, -1f, 0f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 0f, cubeAreaAlphaValue));

        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.XP_ZP],
            "CubeArea_X+Z+",
            new Vector3(1f, 0f, 1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 0f, 1f, cubeAreaAlphaValue));
        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.XP_ZM],
            "CubeArea_X-Z+",
            new Vector3(1f, 0f, -1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 0f, 1f, cubeAreaAlphaValue));

        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.XM_ZP],
            "CubeArea_X-Z+",
            new Vector3(-1f, 0f, 1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 0f, 1f, cubeAreaAlphaValue));
        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.XM_ZM],
            "CubeArea_X-Z-",
            new Vector3(-1f, 0f, -1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 0f, 1f, cubeAreaAlphaValue));

        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.YP_ZP],
            "CubeArea_Y+Z+",
            new Vector3(0f, 1f, 1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(0f, 1f, 1f, cubeAreaAlphaValue));
        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.YP_ZM],
            "CubeArea_Y-Z+",
            new Vector3(0f, 1f, -1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(0f, 1f, 1f, cubeAreaAlphaValue));

        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.YM_ZP],
            "CubeArea_Y-Z+",
            new Vector3(0f, -1f, 1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(0f, 1f, 1f, cubeAreaAlphaValue));
        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.YM_ZM],
            "CubeArea_Y-Z-",
            new Vector3(0f, -1f, -1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(0f, 1f, 1f, cubeAreaAlphaValue));


        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.XP_YP_ZP],
            "CubeArea_X+Y+Z+",
            new Vector3(1f, 1f, 1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 1f, cubeAreaAlphaValue));
        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.XP_YP_ZM],
            "CubeArea_X+Y+Z-",
            new Vector3(1f, 1f, -1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 1f, cubeAreaAlphaValue));

        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.XP_YM_ZP],
            "CubeArea_X+Y-Z+",
            new Vector3(1f, -1f, 1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 1f, cubeAreaAlphaValue));
        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.XP_YM_ZM],
            "CubeArea_X+Y-Z-",
            new Vector3(1f, -1f, -1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 1f, cubeAreaAlphaValue));

        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.XM_YP_ZP],
            "CubeArea_X-Y+Z+",
            new Vector3(-1f, 1f, 1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 1f, cubeAreaAlphaValue));
        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.XM_YP_ZM],
            "CubeArea_X-Y+Z-",
            new Vector3(-1f, 1f, -1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 1f, cubeAreaAlphaValue));

        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.XM_YM_ZP],
            "CubeArea_X-Y-Z+",
            new Vector3(-1f, -1f, 1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 1f, cubeAreaAlphaValue));
        setCubeAreaGameObject(ref m_cubeAreaObject[(uint)ECubeArea.XM_YM_ZM],
            "CubeArea_X-Y-Z-",
            new Vector3(-1f, -1f, -1f),
            new Vector3(1f, 1f, 1f),
            ref cubeAreaMat,
            new Color(1f, 1f, 1f, cubeAreaAlphaValue));

        for (int i = 0; i < m_cubeAreaObject.Length; ++i)
        {
            ECubeArea ae = (ECubeArea)i;

            int x = m_areaEnumToCoordinate[i, 0] + 1;
            int y = m_areaEnumToCoordinate[i, 1] + 1;
            int z = m_areaEnumToCoordinate[i, 2] + 1;

            // Get World Bound from Renderer
            m_areaBound[x, y, z] = m_cubeAreaObject[i].GetComponent<Renderer>().bounds;
        }
    }

    //?? 규태 : 게임매니저에서 순서 관리하기
    //void Start()
    void Awake()
    {
        m_emptyGameObjectPrefab = Resources.Load("Prefab/GameObject") as GameObject;

        m_worldSetterTransform = GetComponent<Transform>();

        m_cubeWorldTransform = transform.Find("CubeWorld");
        m_cubeWorldGameObject = m_cubeWorldTransform.gameObject;

        initDebuggingPlanes();
        initLightSetting();
        initCubeAreaSetting();
    }
}
