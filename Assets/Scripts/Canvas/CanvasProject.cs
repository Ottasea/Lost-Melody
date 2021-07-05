using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasProject : MonoBehaviour
{
    //================================|   Variables   |==========================================================
    [SerializeField] Camera cam;
    [SerializeField] RectTransform canvas;
    float ratio;
    Vector3 centerOffset;
    public static CanvasProject Instance;

    //================================|   Start()   |==========================================================
    public void Start()
    {
        ratio = canvas.sizeDelta.x / cam.pixelWidth;
        centerOffset = new Vector3(canvas.sizeDelta.x, canvas.sizeDelta.y, 0) / 2;

        Instance = this;
    }

    //================================|   Project()   |==========================================================
    public void Project(bool isWithinAngle, RectTransform projection, Transform target, float centerMassRaise = 1.0f)
    {
        if (isWithinAngle)
        {
            projection.gameObject.SetActive(true);
            Vector3 canvasPos = cam.WorldToScreenPoint(target.position + target.up * centerMassRaise);
            canvasPos *= ratio;
            canvasPos -= centerOffset;
            projection.localPosition = canvasPos;
        }
        else
            projection.gameObject.SetActive(false);
    }

}