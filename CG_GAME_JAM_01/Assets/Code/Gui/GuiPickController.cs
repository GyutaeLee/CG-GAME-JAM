using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GuiPickController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsPointerDown
    {
        get { return isPointerDown; }
        set { isPointerDown = value; }
    }

    [SerializeField] private bool isPointerDown;

    /*
     *  VIRTUAL
     */
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        IsPointerDown = true;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("GYUT TEST : 들어 올리는 코드 추가");

        InitializePointerData();
    }

    /*
     *  INIT
     */
    private void InitializePointerData()
    {
        isPointerDown = false;
    }
}
