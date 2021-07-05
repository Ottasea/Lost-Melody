using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibeSystem : MonoBehaviour
{
    //=======================|   Variables   |=================================
    [SerializeField] Animator anim;

    const string anim_pulse = "Pulse";


    //=======================|   Update()   |=================================
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            VibePulse();
    }


    //=======================|   VibePulse()   |=================================
    private void VibePulse()
    {
        anim.SetTrigger(anim_pulse);
    }

}