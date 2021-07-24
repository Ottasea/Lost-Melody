using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Player : MonoBehaviour
{
    //========================|   Variables   |=======================================================
    [SerializeField] Movement movement;
    Movement.PlayerState prevMoveState;

    [SerializeField] AudioSource audioSrc_movement;
    [SerializeField] AudioSource audioSrc_action;
    [SerializeField] AudioSource audioSrc_vibe;

    public enum ActionClip { AttackLight, AttackHeavy_Charge, AttackHeavy, Sonar, Horn }
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
                /*
            case Movement.PlayerState.sprinting:
                audioSrc_movement.clip = AudioClips.Instance.run;
                audioSrc_movement.loop = true;
                break;
                */
            case Movement.PlayerState.jump:
                audioSrc_movement.clip = AudioClips.Instance.jump;
                audioSrc_movement.loop = false;
                break;
            case Movement.PlayerState.jump_land:
                audioSrc_movement.clip = AudioClips.Instance.jump_land;
                audioSrc_movement.loop = false;
                break;
            case Movement.PlayerState.pushing:
                audioSrc_movement.clip = AudioClips.Instance.exertion;
                break;
        }

        if (movement.playerState != Movement.PlayerState.idle)
            audioSrc_movement.Play();
        else
            audioSrc_movement.Stop();
    }

    //========================|   PlayClip_Attack()   |=======================================================
    public void PlayClip_Action(ActionClip clip)
    {
        switch(clip)
        {
            case ActionClip.AttackLight:
                audioSrc_action.clip = AudioClips.Instance.attacksLight[attackLightIndex];
                attackLightIndex++;
                if (attackLightIndex >= AudioClips.Instance.attacksLight.Length)
                    attackLightIndex = 0;
                break;
            case ActionClip.AttackHeavy_Charge:
                audioSrc_action.clip = AudioClips.Instance.attackHeavy_Charge;
                audioSrc_movement.Stop();
                break;
            case ActionClip.AttackHeavy:
                audioSrc_action.clip = AudioClips.Instance.attackHeavy;
                break;
            case ActionClip.Sonar:
                audioSrc_action.clip = AudioClips.Instance.ability_sonar;
                break;
            case ActionClip.Horn:
                audioSrc_action.clip = AudioClips.Instance.ability_horn;
                break;
        }

        audioSrc_action.Play();
    }

    public void PlayClip_VibeChange(VibeSystem.Vibe vibe)
    {
        AudioClip clip = null;
        switch (vibe)
        {
            case VibeSystem.Vibe.Red:
                clip = AudioClips.Instance.vibe_red;
                break;
            case VibeSystem.Vibe.Blue:
                clip = AudioClips.Instance.vibe_blue;
                break;
            case VibeSystem.Vibe.Green:
                clip = AudioClips.Instance.vibe_green;
                break;
            case VibeSystem.Vibe.Yellow:
                clip = AudioClips.Instance.vibe_yellow;
                break;
        }
        audioSrc_vibe.clip = clip;
        audioSrc_vibe.Play();
    }


    //========================|   PlayClip_Damage()   |=======================================================
    public void PlayClip_Damage(DamageClip clip)
    {
        switch (clip)
        {
            case DamageClip.Hit:
                audioSrc_action.clip = AudioClips.Instance.hitVocals[hitIndex];
                hitIndex++;
                if (hitIndex >= AudioClips.Instance.hitVocals.Length)
                    hitIndex = 0;
                break;
            case DamageClip.Die:
                audioSrc_action.clip = AudioClips.Instance.die;
                break;
        }

        audioSrc_action.Play();
    }

}