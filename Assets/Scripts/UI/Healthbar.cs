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

            ShowPercentage(percentage);
        }
    }


    //==============================|   ShowPercentage()   |=============================================
    public void ShowPercentage(float percent)
    {
        Image img = FindImage(percent);
        Sprite spr = FindSprite(percent);

        FillLowerImages(img);

        img.sprite = spr;
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

    //==============================|   LowerImagesAreFull()   |=============================================
    private void FillLowerImages(Image img)
    {
        if (img == one)
            return;
        else if (img == two)
            one.sprite = oneHundredpercent;
        else if (img == three)
        {
            one.sprite = oneHundredpercent;
            two.sprite = oneHundredpercent;
        }    
        else if (img == four)
        {
            one.sprite = oneHundredpercent;
            two.sprite = oneHundredpercent;
            three.sprite = oneHundredpercent;
        }
    }


    //==============================|   FindSprite()   |=============================================
    Sprite FindSprite(float percentage)
    {
        //-----------   If we are at MAX health
        if (percentage >= 1.0f)
            return oneHundredpercent;

        //-----------   Else, proceed as normal
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