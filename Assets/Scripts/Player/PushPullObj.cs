using UnityEngine;

public class PushPullObj : MonoBehaviour
{
    //=======================|   Variables   |========================================
    Rigidbody2D rb;
    Collider2D col;

    //=======================|   Start()   |========================================
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    //=======================|   Activate()   |========================================
    public void Activate()
    {
        rb.isKinematic = false;
        col.isTrigger = false;
    }

    //=======================|   Deactivate()   |========================================
    public void Deactivate()
    {
        rb.isKinematic = true;
        col.isTrigger = true;
    }

}