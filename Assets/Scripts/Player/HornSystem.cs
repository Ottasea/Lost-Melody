﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornSystem : MonoBehaviour
{
    //=======================|   Variables   |=================================
    [SerializeField] Animator anim;
    [SerializeField] Transform tf_dir;

    const string anim_pulse = "Pulse";
    const float duration_pulse = 1.0f;
    const float range = 7.0f;

    const float hornForce = 2.0f;


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
        Movement.Instance.EnableDisable(false);
        ApplyKnockback();

        yield return new WaitForSeconds(duration_pulse);

        Movement.Instance.EnableDisable(true);
    }

    //=======================|   ApplyKnockback()   |=================================
    private void ApplyKnockback()
    {
        Vector2 boxPos = new Vector2(transform.position.x + tf_dir.localScale.z * range / 2, transform.position.y + 1.0f);
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(boxPos, range / 2);

        foreach (Collider2D c in collider2Ds)
            if (c.GetComponent<HitPoints>())
            {
                HitPoints hp = c.GetComponent<HitPoints>();
                float div = Vector3.Distance(hp.tf.position, transform.position) / range;
                if (div > 0)
                {
                    float force = hornForce / div;
                    KnockBack.Instance.StartCoroutine(KnockBack.Instance.Knock(hp.tf, transform.position, force, hp.entityType));
                }
            }
    }

}