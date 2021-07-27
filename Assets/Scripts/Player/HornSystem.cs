using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornSystem : MonoBehaviour
{
    //=======================|   Variables   |=================================
    [SerializeField] Animator anim;

    const string anim_pulse = "Pulse";
    const float duration_pulse = 1.1f;
    const float range = 7.0f;

    const float hornForce = 4.0f;

    public const string tag_player = "Player";


    //=======================|   Update()   |=================================
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(HornPulse());
    }


    //=======================|   IEnumerator - VibePulse()   |=================================
    private IEnumerator HornPulse()
    {
        anim.SetTrigger(anim_pulse);
        Audio_Player.Instance.PlayClip_Action(Audio_Player.ActionClip.Horn);
        Movement.Instance.EnableDisable(false);
        ApplyKnockback();

        yield return new WaitForSeconds(duration_pulse);

        Movement.Instance.EnableDisable(true);
    }

    //=======================|   ApplyKnockback()   |=================================
    private void ApplyKnockback()
    {
        Vector2 boxPos = new Vector2(transform.position.x + SpineAnim_Player.dir * range / 2, transform.position.y + 1.0f);
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(boxPos, range / 2);

        foreach (Collider2D c in collider2Ds)
        {
            Transform tf = null;
            HitPoints.EntityType entityType = HitPoints.EntityType.None;
            bool raycast = false;

            if (c.GetComponent<HitPoints>() != null && c.gameObject.tag != tag_player)
            {
                HitPoints hp = c.GetComponent<HitPoints>();
                tf = hp.tf;
                entityType = hp.entityType;
                raycast = true;
            }
            else if (c.GetComponent<PushPullObj>())
                tf = c.transform;

            if (tf != null)
            {
                float div = Vector3.Distance(tf.position, transform.position) / range;
                if (div > 0)
                {
                    float force = hornForce / div;
                    KnockBack.Instance.StartCoroutine(KnockBack.Instance.Knock(tf, transform.position, force, entityType, raycast));
                }
            }
        }
            
        
    }

}