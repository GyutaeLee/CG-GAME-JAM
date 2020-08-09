using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GuiPickController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Player _Player;

    public bool IsPointerDown
    {
        get { return isPointerDown; }
        set { isPointerDown = value; }
    }

    [SerializeField] private bool isPointerDown;

    private void Start()
    {
        
    }

    private void InitializePickController()
    {
        
    }

    /*
     *  VIRTUAL
     */
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        IsPointerDown = true;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        Transform objectTransform = null;

        //?? 규태 : 테스트 코드
        if (_Player.CanPlayerThrowObject())
        {
            _Player.ThrowObject(1.0f);
        }
        else if (_Player.ShotRayCastToForward(out objectTransform) == true)
        {
            if (CheckObjectIsPickable(objectTransform) == true)
            {
                _Player.PickUpObject(objectTransform);
            }
        }               

        InitializePointerData();
    }

    /*
     *  INIT
     */
    private void InitializePointerData()
    {
        isPointerDown = false;
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
        if (hitTransform.GetComponent<CGThrowableObject>().isMovable == false)
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
