using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies_Shared : MonoBehaviour
{
    public VibeSystem.Vibe vibe;


    private void Start()
    {
        int vibeIndex = Random.Range(0, VibeSystem.vibeCount);
        vibe = (VibeSystem.Vibe)vibeIndex;
    }
}
