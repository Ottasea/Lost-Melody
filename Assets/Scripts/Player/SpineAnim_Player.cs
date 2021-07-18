using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;


public class SpineAnim_Player : MonoBehaviour
{
    //============================|   Variables   |=================================================
    [SerializeField] SkeletonAnimation skeletonAnimation;
    Transform tf;

    [Header("Movement")]
    [SerializeField] AnimationReferenceAsset idle;
    [SerializeField] AnimationReferenceAsset run;
    [SerializeField] AnimationReferenceAsset jump;
    [SerializeField] AnimationReferenceAsset jump_land;

    [Header("Attack")]
    [SerializeField] AnimationReferenceAsset attack_light;
    [SerializeField] AnimationReferenceAsset attack_heavy;

    [Header("Damage")]
    [SerializeField] AnimationReferenceAsset death;

    public enum RefAsset { IDLE, RUN, JUMP, JUMP_LAND, ATTACK_LIGHT, ATTACK_HEAVY, DEATH, PUSH, PULL };
    public static RefAsset prevAsset;

    public const float scale = 0.1f;
    const float scaleX = 1.2f;

    public static SpineAnim_Player Instance;
    public static float dir;


    //============================|   Awake()   |=================================================
    private void Awake()
    {
        Instance = this;
        tf = skeletonAnimation.transform;
    }


    //=========================|   SetAnimation()   |=======================================
    public void SetAnimation(RefAsset refAsset)
    {
        prevAsset = refAsset;
        AnimationReferenceAsset refAss = null;
        bool loop = false;

        switch (refAsset)
        {
            case RefAsset.IDLE:
                refAss = idle;
                loop = true;
                break;
            case RefAsset.RUN:
                refAss = run;
                loop = true;
                break;
            case RefAsset.JUMP:
                refAss = jump;
                loop = false;
                break;
            case RefAsset.JUMP_LAND:
                refAss = jump_land;
                loop = false;
                break;
            case RefAsset.ATTACK_LIGHT:
                refAss = attack_light;
                loop = false;
                break;
            case RefAsset.ATTACK_HEAVY:
                refAss = attack_heavy;
                loop = false;
                break;
            case RefAsset.DEATH:
                refAss = death;
                loop = false;
                break;
        }

        if (refAss != null)
            skeletonAnimation.state.SetAnimation(0, refAss, loop);
        else
            Debug.Log("RefAss == null, for: " + refAsset);
    }

    //=========================|   SetDirection()   |=======================================
    public void SetDirection(float x)
    {
        if (x == 0 || Mathf.Abs(x) > 1.0f)
            Debug.Log("WARNING: SetDirection() called with value of: " + x);

        tf.localScale = new Vector3(x * scaleX, 1, 1) * scale;
        dir = x;
    }

    //=========================|   SetTimeScale()   |=======================================
    public void SetTimeScale(float ts)
    {
        skeletonAnimation.timeScale = ts;
    }
}