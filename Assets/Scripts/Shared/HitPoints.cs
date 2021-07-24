using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoints : MonoBehaviour
{
    //===================|   Variables   |=================================
    public const float hp_player = 50;
    public const float hp_boar = 50;
    public const float hp_draugr = 40;

    [Header("Basic")]
    public Transform tf;
    [System.NonSerialized] public float hitPoints = 100;

    public enum EntityType { Player, Boar, Draugr, None }
    public EntityType entityType;

    [Header("Unit Specific")]
    public Boar boar;
    public Draugr draugr;

    [SerializeField] Audio_Hit audioHit;

    public static HitPoints Instance_Player;


    //===================|   Start()   |=================================
    private void Start()
    {
        if (tf == null)
            tf = transform;

        switch (entityType)
        {
            case EntityType.Player:
                hitPoints = hp_player;
                Instance_Player = this;
                break;
            case EntityType.Boar:
                hitPoints = hp_boar;
                break;
            case EntityType.Draugr:
                hitPoints = hp_draugr;
                break;
        }
    }


    //===================|   ApplyDamage()   |=================================
    public void Hit(float dmg, Vector3 pos = default, float force = 1, VibeSystem.Vibe vibe = VibeSystem.Vibe.NONE)
    {
        //------------   Reduce HitPoints   -----------------------------------
        float oldHp = hitPoints;
        hitPoints -= dmg;
        bool crit = false;

        if (entityType == EntityType.Player)
        {
            Healthbar.Instance.StartCoroutine(Healthbar.Instance.ReduceHealth(oldHp, hitPoints));
            HealthRegen.Instance.JustGotHit();
        }
        else
        {
            if (vibe == VibeSystem.Vibe.NONE)
                Debug.Log("MISTAKE: vibe argument not passed");

            if (vibe == GetComponent<Enemies_Shared>().vibe)
            {
                crit = true;
                dmg *= VibeSystem.vibeDmgMult;
            }

            Attack_NumbersPop.Instance.StartCoroutine(Attack_NumbersPop.Instance.PopNumbers(dmg, crit));
        }

        //------------   Hit anims, knockback   -----------------------------------
        if (hitPoints > 0)
        {
            switch (entityType)
            {
                case EntityType.Player:
                    PlayerDamage.Instance.StartCoroutine(PlayerDamage.Instance.GetHit(pos));
                    break;
                case EntityType.Boar:
                    boar.Hit();
                    break;
                case EntityType.Draugr:
                    draugr.StartCoroutine(draugr.Hit());
                    break;
            }

            //------------   KnockBack   -----------------------------------
            if (pos != default)
                KnockBack.Instance.StartCoroutine(KnockBack.Instance.Knock(tf, pos, force, entityType, true));
            else
                Debug.Log("not Knocking, pos: " + pos + ", tf: " + tf.name);
        }
        //------------   Die?   -----------------------------------
        else
        {
            switch (entityType)
            {
                case EntityType.Player:
                    PlayerDamage.Instance.Die(pos);
                    break;
                case EntityType.Boar:
                    boar.Die();
                    break;
                case EntityType.Draugr:
                    draugr.StartCoroutine(draugr.Die());
                    break;
                default:
                    Debug.Log("ERROR: Default EntityType");
                    Die();
                    break;
            }

            crit = true;
        }

        //------------   Hit SFX   -----------------------------------
        audioHit.PlayHit(crit);
    }


    //===================|   Die()   |=================================
    private void Die()
    {
        Destroy(gameObject);
    }

}