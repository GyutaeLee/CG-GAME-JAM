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

        if (_Player.ShotRayCastToForward(out objectTransform) == true)
        {
            _Player.PickUpObject(objectTransform);
        }
        //?? 규태 : 테스트 코드
        else if (_Player.PlayerState == Player.EPlayerState.PLAYER_STATE_PICK)
        {
            _Player.ThrowObject(1.0f);
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
}
