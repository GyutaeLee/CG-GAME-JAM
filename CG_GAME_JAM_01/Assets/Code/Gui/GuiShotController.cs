using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;

public class GuiShotController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    const float MaxPointerDownTime = 3.0f;

    public bool IsPointerDown
    {
        get { return isPointerDown; }
        set { isPointerDown = value; }
    }

    public Image IMGStrengthBar;

    [SerializeField] private bool isPointerDown;

    [SerializeField] private float pointerDownTime;
    [SerializeField] private float shotStrength;

    private void Update()
    {
        UpdatePointerDownTime();
        UpdatePointerDownTimeUIBar();
    }

    /*
     *  VIRTUAL
     */
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        InitializePointerData();

        //?? 규태 : 조이스틱 못 움직이도록 해야함... 게임 오브젝트를 꺼야하나?

        IsPointerDown = true;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("GYUT TEST - SHOTSTRENGTH :" + shotStrength);
        InitializePointerData();
    }

    /*
     *  UPDATE 
     */
    private void UpdatePointerDownTime()
    {
        if (isPointerDown == false)
            return;

        pointerDownTime += Time.deltaTime;

        CalculateShotStrength();
    }
    private void UpdatePointerDownTimeUIBar()
    {
        IMGStrengthBar.fillAmount = shotStrength;
    }

    /*
     *  INIT
     */
    private void InitializePointerData()
    {
        isPointerDown = false;
        pointerDownTime = 0.0f;
        shotStrength = 0.0f;
        IMGStrengthBar.fillAmount = 0.0f;
    }

    /*
     *  ETC
     */

    private void CalculateShotStrength()
    {
        if (MaxPointerDownTime == 0 || pointerDownTime >= MaxPointerDownTime)
        {
            shotStrength = 1.0f;
        }
        else
        {
            shotStrength = pointerDownTime / MaxPointerDownTime;
        }
    }
}
