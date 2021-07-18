using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRegen : MonoBehaviour
{
    //==========================|   Variables   |===============================================
    const float duration_untilStart = 2.0f;
    const float duration_fullRegen = 20.0f;
    const float duration_interval = 0.2f;

    public static HealthRegen Instance;


    //==========================|   Awake()   |===============================================
    private void Awake()
    {
        Instance = this;
    }


    //==========================|   JustGotHit()   |===============================================
    public void JustGotHit()
    {
        StopAllCoroutines();
        StartCoroutine(Regen());
    }


    //==========================|   IEnumerator - Regen()   |===============================================
    private IEnumerator Regen()
    {
        yield return new WaitForSeconds(duration_untilStart);

        float perSecond = HitPoints.hp_player / duration_fullRegen;

        float timeLast = Time.time;
        while (HitPoints.Instance_Player.hitPoints < HitPoints.hp_player)
        {
            HitPoints.Instance_Player.hitPoints += perSecond * (Time.time - timeLast);
            timeLast = Time.time;
            Healthbar.Instance.ShowPercentage(HitPoints.Instance_Player.hitPoints / HitPoints.hp_player);
            yield return new WaitForSeconds(duration_interval);
        }

        HitPoints.Instance_Player.hitPoints = HitPoints.hp_player;
        Healthbar.Instance.ShowPercentage(1.0f);
    }

}