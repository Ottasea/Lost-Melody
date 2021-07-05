using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    //====================|   Variables   |==========================================
    public static float leftDownTime_Prev = 0;
    public static float leftDownTime_Current = 0;
    public static float heldTime;
    public const float timeTillHeld = 0.25f;

    public enum ClickType { none, leftDown, leftHeld, leftUp_notHeld, leftUp_Held };
    public static ClickType clickType;


    //====================|   Update()   |==========================================
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickType = ClickType.leftDown;
            leftDownTime_Prev = leftDownTime_Current;
            leftDownTime_Current = Time.time;
        }
        else if (Input.GetMouseButton(0))
        {
            if (Time.time - leftDownTime_Current > timeTillHeld)
            {
                clickType = ClickType.leftHeld;
                heldTime = Time.time - leftDownTime_Current;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (clickType == ClickType.leftHeld)
                clickType = ClickType.leftUp_Held;
            else
                clickType = ClickType.leftUp_notHeld;
        }
        else
            clickType = ClickType.none;

    }

}