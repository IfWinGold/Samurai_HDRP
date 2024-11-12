using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameUtility
{
    public static Vector3 GetInputDirection()
    {
        float mx = Input.GetAxis("Horizontal");
        float my = Input.GetAxis("Vertical");
        return new Vector3(mx, 0f, my);
    }    
    public static Vector3 GetMouse()
    {
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        return new Vector3(mx, my, 0f);
    }
    public static Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }
}
