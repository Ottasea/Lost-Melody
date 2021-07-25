using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_LateralCollision : MonoBehaviour
{
    //==================|   Variables   |=========================================
    [SerializeField] LayerMask layerMask;
    public const float yOffset = 0.75f;
    const float range = 0.75f;

    public static Movement_LateralCollision Instance;

    //==================|   Awake()   |=========================================
    private void Awake()
    {
        Instance = this;
    }

    //==================|   AllowMovement()   |=========================================
    public bool AllowMovement(float x)
    {
        Vector2 dir = Vector2.right * x;

        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up * yOffset, dir, range, layerMask);

        if (hit.collider != null)
        {
            //Debug.Log("Hit: " + hit.collider.name);
            return false;
        }

        return true;
    }

}