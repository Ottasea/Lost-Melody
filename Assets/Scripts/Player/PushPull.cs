using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPull : MonoBehaviour
{
    //==========================|   Variables   |======================================
    Transform pushedObj;

    //==========================|   OnTriggerEnter2D()   |======================================
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.GetComponent<PushPullObj>() == null)
            return;

        PushPullObj ppObj = other.collider.GetComponent<PushPullObj>();
        pushedObj = other.transform;

        SpineAnim_Player.Instance.SetAnimation(SpineAnim_Player.RefAsset.PUSH);
    }

    //==========================|   OnTriggerExit2D()   |======================================
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.GetComponent<PushPullObj>() == null)
            return;

        if (other.collider.transform == pushedObj.transform)
        {
            SpineAnim_Player.Instance.SetAnimation(SpineAnim_Player.RefAsset.IDLE);
            pushedObj = null;
        }
    }

}