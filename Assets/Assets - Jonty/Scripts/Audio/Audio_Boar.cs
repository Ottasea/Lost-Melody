using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Boar : MonoBehaviour
{
    //==================|   Variables   |=========================================
    public enum ClipSteps { walk, run, none }
    public enum ClipVocal { growl, pain, die }

    [SerializeField] AudioSource audioSrc_footsteps;
    [SerializeField] AudioSource audioSrc_vocal;
    

    //==================|   PlayFootsteps()   |=========================================
    public void PlayFootsteps(ClipSteps steps)
    {
        switch (steps)
        {
            case ClipSteps.walk:
                audioSrc_footsteps.clip = AudioClips.Instance.boar_walk;
                break;
            case ClipSteps.run:
                audioSrc_footsteps.clip = AudioClips.Instance.boar_run;
                break;
        }

        if (steps != ClipSteps.none)
            audioSrc_footsteps.Play();
        else
            audioSrc_footsteps.Stop();
    }


    //==================|   PlayVocal()   |=========================================
    public void PlayVocal(ClipVocal vocal)
    {
        switch (vocal)
        {
            case ClipVocal.growl:
                audioSrc_vocal.clip = AudioClips.Instance.boar_growl;
                break;
            case ClipVocal.pain:
                audioSrc_vocal.clip = AudioClips.Instance.boar_hit;
                break;
            case ClipVocal.die:
                audioSrc_vocal.clip = AudioClips.Instance.boar_die;
                break;
        }

        audioSrc_vocal.Play();
    }

}