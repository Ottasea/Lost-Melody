using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    //==========================|   Variables   |===========================================
    [SerializeField] Transform tf;

    const float duration_hit = 1.0f;

    const float duration_transitionIn = 0.25f;
    const float duration_transitionOut = 0.25f;

    public static PlayerDamage Instance;

    public static bool dead;


    //==========================|   Awake()   |==============================================
    private void Awake()
    {
        Instance = this;
    }


    //==========================|   IEnumerator - GetHit()   |=========================================
    public IEnumerator GetHit(Vector3 origin)
    {
        Debug.Log("GetHit() has been disabled due to a lack of a flinch animation");
        yield break;
        /*
        ///---------------   Before   ------------------------------
        //anim.SetTrigger(anim_hit);
        //anim.SetBool(Movement.anim_idle, false);
        SpineAnim_Player.Instance.SetAnimation(SpineAnim_Player.RefAsset.)
        Movement.Instance.EnableDisable(false);
        Audio_Player.Instance.PlayClip_Damage(Audio_Player.DamageClip.Hit);

        float x = origin.x - tf.position.x > 0 ? 1 : -1;
        anim.transform.localScale = new Vector3(x, 1, 1);

        ///---------------   Transition in   ------------------------------
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration_transitionIn;
            anim.SetLayerWeight(Attack_Melee.layer_fullBody, t);
            yield return null;
        }

        ///---------------   Wait   ------------------------------
        yield return new WaitForSeconds(duration_hit - duration_transitionIn);

        ///---------------   After   ------------------------------
        Movement.Instance.EnableDisable(true);
        anim.SetBool(Movement.anim_idle, true);
        anim.SetLayerWeight(Attack_Melee.layer_fullBody, 0);

        ///---------------   Transition in   ------------------------------
        t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime / duration_transitionOut;
            anim.SetLayerWeight(Attack_Melee.layer_fullBody, t);
            yield return null;
        }
        */
    }

    //=======================|   IEnumerator - Die()   |========================================
    public void Die(Vector3 origin)
    {
        //---------------   Before   ------------------------------
        SpineAnim_Player.Instance.SetAnimation(SpineAnim_Player.RefAsset.DEATH);
        Movement.Instance.EnableDisable(false);

        float x = origin.x - tf.position.x > 0 ? 1 : -1;
        SpineAnim_Player.Instance.SetDirection(x);

        Boar[] boars = FindObjectsOfType<Boar>();
        foreach (Boar b in boars)
            b.Victory();
    }

}