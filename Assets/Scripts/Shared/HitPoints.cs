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

    public enum EntityType { Player, Boar, Draugr }
    public EntityType entityType;

    [Header("Unit Specific")]
    public Boar boar;
    public Draugr draugr;

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
    public void Hit(float dmg, Vector3 pos = default, float force = 1)
    {
        //------------   Reduce HitPoints   -----------------------------------
        float oldHp = hitPoints;
        hitPoints -= dmg;

        if (entityType == EntityType.Player)
        {
            Healthbar.Instance.StartCoroutine(Healthbar.Instance.ReduceHealth(oldHp, hitPoints));
            HealthRegen.Instance.JustGotHit();
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
                KnockBack.Instance.StartCoroutine(KnockBack.Instance.Knock(tf, pos, force, entityType));
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
                    Die();
                    break;
            }
        }
    }


    //===================|   Die()   |=================================
    private void Die()
    {
        Destroy(gameObject);
    }

}