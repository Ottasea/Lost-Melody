using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Melee : MonoBehaviour
{
    //===============================|   Variables   |=====================================================
    [SerializeField] Animator anim;

    [SerializeField] LayerMask layer_hitPoints;

    [System.NonSerialized] public Transform tf_dir;

    const string anim_attack1 = "Axe - Attack - 1";
    const string anim_attack2 = "Axe - Attack - 2";
    const string anim_attackHeavy_Charging = "Axe - Heavy - Charge";
    const string anim_attackHeavy = "Axe - Heavy";

    const float duration_attackLight_recover = 0.4f;
    const float duration_attackLight = 0.3f;
    const float duration_attackHeavy = 1.7f;

    const float transition_in_attackHeavyCharge = 0.5f;
    const float transition_middle_attachHeavy = 1.3f;
    const float transition_out_attackHeavy = 0.25f;

    public const int layer_upperBody = 1;
    public const int layer_fullBody = 2;

    const float dmg_light = 20.0f;
    const float dmg_heavy = 100.0f;

    const float force_light = 1.0f;
    const float force_heavy = 2.0f;

    const float range_light = 1.5f;
    const float range_heavy = 2.0f;

    float timeSinceLastLightAttack;

    int lastLightAttack = 0;

    [System.NonSerialized] public bool attacking;

    public static Attack_Melee Instance;


    //===============================|   Awake()   |=====================================================
    private void Awake()
    {
        Instance = this;
    }

    //===============================|   Start()   |=====================================================
    private void Start()
    {
        anim.SetLayerWeight(layer_upperBody, 0.0f);
        anim.SetLayerWeight(layer_fullBody, 0.0f);
    }

    //===============================|   Update()   |=====================================================
    void Update()
    {
        if (Movement.canMove)
        {
            if (ClickManager.clickType == ClickManager.ClickType.leftDown)
            {
                if (Movement.Instance.playerState == Movement.PlayerState.walking || Movement.Instance.playerState == Movement.PlayerState.sprinting)
                    Movement.Instance.playerState = Movement.PlayerState.idle;
            }
            else if (ClickManager.clickType == ClickManager.ClickType.leftHeld)
            {
                StopCoroutine(Attack_Light());
                StartCoroutine(Attack_Heavy_Charge());
            }
            else if (ClickManager.clickType == ClickManager.ClickType.leftUp_notHeld)
            {
                if ((Time.time - timeSinceLastLightAttack) > duration_attackLight)
                {
                    StopCoroutine(Attack_Light());
                    StartCoroutine(Attack_Light());
                }
            }

        }

    }

    //===============================|   IEnumerator - Attack()   |=====================================================
    IEnumerator Attack_Light()
    {
        //-------------------   Beginning   --------------------------------
        timeSinceLastLightAttack = Time.time;
        float localTimeSinceLast = timeSinceLastLightAttack;
        attacking = true;

        anim.SetLayerWeight(layer_upperBody, 1.0f);

        lastLightAttack = lastLightAttack == 2 ? 1 : 2;
        anim.SetTrigger(lastLightAttack == 1 ? anim_attack1 : anim_attack2);
        anim.SetBool(Movement.anim_idle, false);

        Audio_Player.Instance.PlayClip_Attack(Audio_Player.AttackClip.AttackLight);

        //-------------------   Middle   --------------------------------
        yield return new WaitForSeconds(duration_attackLight);

        ApplyDamage(dmg_light, force_light, range_light);

        yield return new WaitForSeconds(duration_attackLight_recover);

        //-------------------   After   --------------------------------
        if (timeSinceLastLightAttack == localTimeSinceLast)
        {
            anim.SetBool(Movement.anim_idle, true);
            anim.SetLayerWeight(layer_upperBody, 0.0f);
            attacking = false;
        }
    }

    //===============================|   IEnumerator - Attack_Heavy_Charge()   |=====================================================
    IEnumerator Attack_Heavy_Charge()
    {
        //-----------------------   Beginning   -------------------------------------
        anim.SetTrigger(anim_attackHeavy_Charging);
        anim.SetBool(Movement.anim_idle, false);

        attacking = false;

        Movement.Instance.EnableDisable(false);

        Audio_Player.Instance.PlayClip_Attack(Audio_Player.AttackClip.AttackHeavy_Charge);

        //-----------------------   Transition in animation   -------------------------------------
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / transition_in_attackHeavyCharge;
            anim.SetLayerWeight(layer_fullBody, t);
            yield return null;

            if (Input.GetMouseButtonUp(0))
            {
                StartCoroutine(Attack_Heavy(t * 0.25f));
                yield break;
            }
        }

        //-----------------------   After / wait for GetMouseButtonUp()   -------------------------------------
        anim.SetLayerWeight(layer_fullBody, 1.0f);

        while (true)
        {
            if (Input.GetMouseButtonUp(0))
            {
                StartCoroutine(Attack_Heavy(1.0f));
                yield break;
            }

            yield return null;
        }
    }

    //===============================|   IEnumerator - Attack_Heavy()   |=====================================================
    IEnumerator Attack_Heavy(float chargedAmount)
    {
        //-------------------   Begin   ---------------------------------
        anim.SetBool(Movement.anim_idle, true);
        anim.SetFloat(Movement.anim_speed, 0.0f);
        anim.SetTrigger(anim_attackHeavy);

        Audio_Player.Instance.PlayClip_Attack(Audio_Player.AttackClip.AttackHeavy);

        //-------------------   Wait for Middle   ---------------------------------
        yield return new WaitForSeconds(duration_attackHeavy - transition_out_attackHeavy - transition_middle_attachHeavy);

        ApplyDamage(dmg_heavy, chargedAmount * force_heavy, range_heavy);

        yield return new WaitForSeconds(transition_middle_attachHeavy);

        //-------------------   Transition out   ---------------------------------
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / transition_out_attackHeavy;
            anim.SetLayerWeight(layer_fullBody, 1 - t);
            yield return null;
        }

        //-------------------   End   ---------------------------------
        Attack_Heavy_Stop();
    }


    //===============================|   Attack_Heavy_Stop()   |=====================================================
    private void Attack_Heavy_Stop()
    {
        anim.SetBool(Movement.anim_idle, true);
        anim.SetLayerWeight(layer_fullBody, 0.0f);
        Movement.Instance.EnableDisable(true);
    }


    //===============================|   ApplyDamage()   |=====================================================
    private void ApplyDamage(float dmg, float force, float range)
    {
        Vector2 boxPos = new Vector2(transform.position.x + tf_dir.localScale.z* range, transform.position.y + 1.0f);
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(boxPos, range);

        foreach (Collider2D c in collider2Ds)
            if (c.GetComponent<HitPoints>())
                if (c.tag != "Player")
                    c.GetComponent<HitPoints>().Hit(dmg, transform.position, force);
    }

}