using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Reflection;

public class GuiShotController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private const float kMaxPointerDownTime = 3.0f;

    public bool IsPointerDown
    {
        get { return m_isPointerDown; }
        set { m_isPointerDown = value; }
    }

    public Image IMGStrengthBar;

    [SerializeField] private bool m_isPointerDown;

    [SerializeField] private float m_pointerDownTime;
    [SerializeField] private float m_shotStrength;

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
        ResetPointerData();

        //?? 규태 : 조이스틱 못 움직이도록 해야함... 게임 오브젝트를 꺼야하나?

        this.m_isPointerDown = true;
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("GYUT TEST - SHOTSTRENGTH :" + m_shotStrength);
        ResetPointerData();
    }

    /*
     *  UPDATE 
     */
    private void UpdatePointerDownTime()
    {
        if (this.m_isPointerDown == false)
            return;

        this.m_pointerDownTime += Time.deltaTime;

        CalculateShotStrength();
    }
    private void UpdatePointerDownTimeUIBar()
    {
        this.IMGStrengthBar.fillAmount = this.m_shotStrength;
    }

    /*
     *  INIT
     */
    private void ResetPointerData()
    {
        this.m_isPointerDown = false;
        this.m_pointerDownTime = 0.0f;
        this.m_shotStrength = 0.0f;
        this.IMGStrengthBar.fillAmount = 0.0f;
    }

    /*
     *  ETC
     */

    private void CalculateShotStrength()
    {
        if (kMaxPointerDownTime == 0 || this.m_pointerDownTime >= kMaxPointerDownTime)
        {
            this.m_shotStrength = 1.0f;
        }
        else
        {
            this.m_shotStrength = this.m_pointerDownTime / kMaxPointerDownTime;
        }
    }
}
