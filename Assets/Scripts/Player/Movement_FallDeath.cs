using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_FallDeath : MonoBehaviour
{
    const string tag_deathZone = "DeathZone";
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == tag_deathZone)
            Movement.Instance.Respawn();
    }
}
