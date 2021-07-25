using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies_Shared : MonoBehaviour
{
    public VibeSystem.Vibe vibe;

    public const float range = 12.0f;


    private void Start()
    {
        ChangeVibe();
    }

    public void ChangeVibe()
    {
        int vibeIndex = Random.Range(0, VibeSystem.vibeCount);
        vibe = (VibeSystem.Vibe)vibeIndex;
    }
}
