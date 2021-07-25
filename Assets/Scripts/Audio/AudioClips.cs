using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClips : MonoBehaviour
{
    //==========================|   Variables   |===============================================
    [Header("Movement")]
    public AudioClip idle;
    public AudioClip walk;
    public AudioClip run;
    public AudioClip jump;
    public AudioClip jump_land;
    public AudioClip exertion;

    [Header("Attack")]
    public AudioClip[] attacksLight;
    public AudioClip attackHeavy_Charge;
    public AudioClip attackHeavy;

    [Header("Damage")]
    public AudioClip[] hitVocals;
    public AudioClip die;

    [Header("Boar")]
    public AudioClip boar_walk;
    public AudioClip boar_run;
    public AudioClip boar_hit;
    public AudioClip boar_die;
    public AudioClip boar_growl;

    [Header("Polar Bear")]
    public AudioClip polarBear_roll;

    [Header("Abilities")]
    public AudioClip ability_sonar;
    public AudioClip ability_horn;

    [Header("Vibe Changing")]
    public AudioClip vibe_red;
    public AudioClip vibe_green;
    public AudioClip vibe_blue;
    public AudioClip vibe_yellow;

    [Header("Hits")]
    public AudioClip hit_block;
    public AudioClip hit_crit;

    public static AudioClips Instance;


    //==========================|   Awake()   |===============================================
    private void Awake()
    {
        Instance = this;
    }

}