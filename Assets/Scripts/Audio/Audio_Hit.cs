using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Hit : MonoBehaviour
{
    [SerializeField] AudioSource audioSrc_hit;

    public void PlayHit(bool crit)
    {
        AudioClip clip;
        if (crit)
            clip = AudioClips.Instance.hit_crit;
        else
            clip = AudioClips.Instance.hit_block;

        if (clip == null)
            Debug.Log("ERROR: audioClip == null");

        audioSrc_hit.clip = clip;
        audioSrc_hit.Play();
    }
}
