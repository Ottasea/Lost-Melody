using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Player : MonoBehaviour
{
    //========================|   Variables   |=======================================================
    [SerializeField] Movement movement;
    Movement.PlayerState prevMoveState;

    [SerializeField] AudioSource audioSrc_movement;
    [SerializeField] AudioSource audioSrc_Attack;

    public enum AttackClip { AttackLight, AttackHeavy_Charge, AttackHeavy }
    int attackLightIndex = 0;
    public enum DamageClip { Hit, Die }
    int hitIndex = 0;

    public static Audio_Player Instance;


    //========================|   Awake()   |=======================================================
    private void Awake()
    {
        Instance = this;
    }

    //========================|   Update()   |=======================================================
    private void Update()
    {
        if (movement.playerState != prevMoveState)
            ChangeState_Movement();

        prevMoveState = movement.playerState;
    }


    //========================|   ChangeState_Movement()   |=======================================================
    private void ChangeState_Movement()
    {
        switch (movement.playerState)
        {
            case Movement.PlayerState.idle:
                audioSrc_movement.clip = AudioClips.Instance.idle;
                audioSrc_movement.loop = true;
                break;
            case Movement.PlayerState.walking:
                audioSrc_movement.clip = AudioClips.Instance.walk;
                audioSrc_movement.loop = true;
                break;
            case Movement.PlayerState.sprinting:
                audioSrc_movement.clip = AudioClips.Instance.run;
                audioSrc_movement.loop = true;
                break;
            case Movement.PlayerState.jump:
                audioSrc_movement.clip = AudioClips.Instance.jump;
                audioSrc_movement.loop = false;
                break;
            case Movement.PlayerState.jump_land:
                audioSrc_movement.clip = AudioClips.Instance.jump_land;
                audioSrc_movement.loop = false;
                break;
        }

        if (movement.playerState != Movement.PlayerState.idle)
            audioSrc_movement.Play();
        else
            audioSrc_movement.Stop();
    }

    //========================|   PlayClip_Attack()   |=======================================================
    public void PlayClip_Attack(AttackClip clip)
    {
        switch(clip)
        {
            case AttackClip.AttackLight:
                audioSrc_Attack.clip = AudioClips.Instance.attacksLight[attackLightIndex];
                attackLightIndex++;
                if (attackLightIndex >= AudioClips.Instance.attacksLight.Length)
                    attackLightIndex = 0;
                break;
            case AttackClip.AttackHeavy_Charge:
                audioSrc_Attack.clip = AudioClips.Instance.attackHeavy_Charge;
                audioSrc_movement.Stop();
                break;
            case AttackClip.AttackHeavy:
                audioSrc_Attack.clip = AudioClips.Instance.attackHeavy;
                break;
        }

        audioSrc_Attack.Play();
    }

    //========================|   PlayClip_Damage()   |=======================================================
    public void PlayClip_Damage(DamageClip clip)
    {
        switch (clip)
        {
            case DamageClip.Hit:
                audioSrc_Attack.clip = AudioClips.Instance.hitVocals[hitIndex];
                hitIndex++;
                if (hitIndex >= AudioClips.Instance.hitVocals.Length)
                    hitIndex = 0;
                break;
            case DamageClip.Die:
                audioSrc_Attack.clip = AudioClips.Instance.die;
                break;
        }

        audioSrc_Attack.Play();
    }

}