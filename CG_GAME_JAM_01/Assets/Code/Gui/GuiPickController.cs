using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GuiPickController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Player player;

    public bool IsPointerDown
    {
        get { return this.m_isPointerDown; }
        set { this.m_isPointerDown = value; }
    }

    [SerializeField] private bool m_isPointerDown;

    private void Start()
    {
        InitPickController();
    }

    private void InitPickController()
    {

    }

    /*
     *  VIRTUAL
     */
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        this.m_isPointerDown = true;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        Transform objectTransform = null;

        //?? 규태 : 테스트 코드
        if (this.player.CanPlayerThrowObject())
        {
            this.player.ThrowObject(1.0f);
        }
        else if (this.player.ShotRayCastToForward(out objectTransform) == true)
        {
            if (CheckObjectIsPickable(objectTransform) == true)
            {
                this.player.PickUpObject(objectTransform);
            }
        }               

        InitPointerData();
    }

    /*
     *  INIT
     */
    private void InitPointerData()
    {
        this.m_isPointerDown = false;
    }

    /*
     *  ETC
     */ 
    private bool CheckObjectIsPickable(Transform objectTransform)
    {
        Transform hitTransform = objectTransform;

        // 1. CGThrowableObject 인지 확인한다
        if (hitTransform.GetComponent<CGThrowableObject>() == false)
        {
            return false;
        }

        // 2. EObjectType이 MOVABLE인지 확인한다.
        if (hitTransform.GetComponent<CGThrowableObject>().IsMovable == false)
        {
            return false;
        }

        // 3. EObjectState가 OBJECT_STATE_GROUND인지 확인한다.
        if (hitTransform.GetComponent<CGThrowableObject>().IsPickable() == false)
        {
            return false;
        }

        return true;
    }
}
