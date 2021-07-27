using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastOntoTerrain : MonoBehaviour
{
    //========================|   Variables   |===============================================================
    [SerializeField] LayerMask terrainLayer;
    static LayerMask terrainLayer_Static;
    const float distance = 20.0f;
    const float shotElevation = 10.0f;
    //const float onGroundElevationLimit = 0,;
    //const string layer_Walkable = "Walkable";

    public static RaycastOntoTerrain Instance;


    //========================|   Awake()   |===============================================================
    private void Awake()
    {
        Instance = this;
        terrainLayer_Static = terrainLayer;
    }


    //========================|   RaycastOnto2dTerrain()   |===============================================================
    public static bool RaycastOnto2dTerrain(Transform obj, float yOffset = 0.0f)
    {
        RaycastHit2D hit = Physics2D.Raycast(obj.position + Vector3.up * shotElevation, Vector2.down, distance, terrainLayer_Static);

        /*
        if (!YDiffNotTooGreat(hit.point.y, obj.position.y))
            return;
        */

        if (hit.collider != null)
        {
            obj.position = hit.point + Vector2.up * yOffset;
            return true;
        }
        else
        {
            Debug.Log("RaycastHit2D hit.Collider == null");
            return false;
        }
    }

    /*
    //========================|   YDiffNotTooGreat()   |===============================================================
    static bool YDiffNotTooGreat (float hitY, float tfY)
    {
        if (hitY - tfY < onGroundElevationLimit)
            return true;
        else
            return false;
    }
    */

    /*
    //========================|   RaycastOnto2dTerrain()   |===============================================================
    public static bool IsOnTerrain(Transform obj, float range)
    {
        RaycastHit2D hit = Physics2D.Raycast(obj.position, Vector2.down, range, terrainLayer_Static);

        if (hit)
        {
            if (YDiffNotTooGreat(hit.point.y, obj.position.y))
                return true;
            else
                return false;
        }
        else
            return false;
    }
    */
}