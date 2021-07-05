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
    const string layer_Walkable = "Walkable";

    public static RaycastOntoTerrain Instance;


    //========================|   Awake()   |===============================================================
    private void Awake()
    {
        Instance = this;
        terrainLayer_Static = terrainLayer;
    }


    //========================|   RaycastOnto2dTerrain()   |===============================================================
    public static void RaycastOnto2dTerrain(Transform obj)
    {
        RaycastHit2D hit = Physics2D.Raycast(obj.position + Vector3.up * shotElevation, Vector2.down, distance, terrainLayer_Static);

        if (hit.collider != null)
            obj.position = hit.point;
        else
            Debug.Log("RaycastHit2D hit.Collider == null");
    }

    //========================|   RaycastOnto2dTerrain()   |===============================================================
    public static bool IsOnTerrain(Transform obj, float range)
    {
        RaycastHit2D hit = Physics2D.Raycast(obj.position, Vector2.down, range, terrainLayer_Static);

        if (hit)
            return true;
        else
            return false;
    }

}