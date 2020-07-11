using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsWorldManager : MonoBehaviour
{
    List<CGObject> m_cgObjects;
    public void LaunchCGObject(CGObject obj)
    {
        m_cgObjects.Add(obj);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
