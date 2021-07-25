using UnityEngine;

public class PushPullObj : MonoBehaviour
{
    //=======================|   Variables   |========================================
    //Rigidbody2D rb;
    Collider2D col;
    const float radius = 0.6f;

    bool pushed = false;

    //=======================|   Start()   |========================================
    private void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        //rb.isKinematic = true;
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    //=======================|   Update()   |========================================
    private void Update()
    {
        if (pushed)
            RaycastOntoTerrain.RaycastOnto2dTerrain(transform, radius);
    }

    //=======================|   Activate()   |========================================
    public void Activate()
    {
        //rb.isKinematic = false;
        col.isTrigger = false;
        pushed = true;
    }

    //=======================|   Deactivate()   |========================================
    public void Deactivate()
    {
        //rb.isKinematic = true;
        col.isTrigger = true;
        pushed = false;
    }

    /*
     * Cannot walk through crates
     * Can push/drag or jump over
     * movement_lateral raycast ignores crates
     * RaycastOntoTerrain includes crates
     */

}