using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 현재 테스트 용도로, 월드에 필요한 오브젝트들을 세팅한다.
 */
public class WorldSetter : MonoBehaviour
{
    Light[] m_cubeLights = new Light[6];
    Transform m_cubeWorldTransform;
    GameObject m_cubeWorldGameObject;

    GameObject[] m_prefabPlaneArea = new GameObject[12];

    // Start is called before the first frame update
    void Start()
    {
        m_cubeWorldTransform = transform.Find("CubeWorld");
        m_cubeWorldGameObject = m_cubeWorldTransform.gameObject;

        GameObject prefab = Resources.Load("Prefab/pAreaPlane") as GameObject;
        
        for(int i = 0; i < 12; ++i)
        {
            m_prefabPlaneArea[i] = GameObject.Instantiate(prefab, m_cubeWorldTransform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
