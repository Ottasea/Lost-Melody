using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    //==============================|   Variables   |=============================================
    [Header("Sprites")]
    [SerializeField] Sprite zeroPercent;
    [SerializeField] Sprite twentyFivePercent;
    [SerializeField] Sprite fiftyPercent;
    [SerializeField] Sprite seventyFivePercent;
    [SerializeField] Sprite oneHundredpercent;

    [Header("Sprite Renderers")]
    [SerializeField] Image one;
    [SerializeField] Image two;
    [SerializeField] Image three;
    [SerializeField] Image four;

    const float duration_reduction = 0.5f;

    public static Healthbar Instance;


    //==============================|   Awake()   |=============================================
    private void Awake()
    {
        Instance = this;
    }


    //==============================|   IEnumerator - ReduceHealth()   |=============================================
    public IEnumerator ReduceHealth(float oldHealth, float newHealth)
    {
        float t = 0;

        while (t < 1)
        {
            yield return null;

            t += Time.deltaTime / duration_reduction;

            float health = Mathf.Lerp(oldHealth, newHealth, t);
            float percentage = health / HitPoints.hp_player;

            Image img = FindImage(percentage);
            Sprite spr = FindSprite(percentage);

            img.sprite = spr;
        }
    }


    //==============================|   FindSpriteRenderer()   |=============================================
    Image FindImage(float percentage)
    {
        if (percentage >= 0.75f)
            return four;
        else if (percentage >= 0.5f)
            return three;
        else if (percentage >= 0.25f)
            return two;
        else
            return one;
    }


    //==============================|   FindSprite()   |=============================================
    Sprite FindSprite(float percentage)
    {
        percentage *= 4.0f;

        float remainder = percentage % 1;

        if (remainder >= 0.9f)
            return oneHundredpercent;
        else if (remainder >= 0.67f)
            return seventyFivePercent;
        else if (remainder > 0.4f)
            return fiftyPercent;
        else if (remainder > 0.1f)
            return twentyFivePercent;
        else 
            return zeroPercent;
    }

}