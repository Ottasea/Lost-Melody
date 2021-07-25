using UnityEngine;
using Spine.Unity;

public class Enemies_Spine2D : MonoBehaviour
{
    //============================|   Variables   |=================================================
    [SerializeField] SkeletonAnimation skeletonAnimation;
    Transform tf;

    [Header("Movement")]
    [SerializeField] AnimationReferenceAsset idle;
    [SerializeField] AnimationReferenceAsset move;
    [SerializeField] AnimationReferenceAsset moveFrom;

    [Header("Attack")]
    [SerializeField] AnimationReferenceAsset attack;

    [Header("Damage")]
    [SerializeField] AnimationReferenceAsset death;
    [SerializeField] AnimationReferenceAsset hit;

    [Header("Polar Bear")]
    [SerializeField] AnimationReferenceAsset roll;
    [SerializeField] AnimationReferenceAsset groundPound;

    public enum RefAsset { IDLE, WALK_TO, WALK_FROM, RUN, HIT, ATTACK, DEATH, ROLL, GROUNDPOUND };
    public static RefAsset prevAsset;

    public const float scale = 0.1f;
    const float scaleX = 1.2f;

    public static Enemies_Spine2D Instance;
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
        float timeScale = 1.0f;

        switch (refAsset)
        {
            case RefAsset.IDLE:
                refAss = idle;
                loop = true;
                break;
            case RefAsset.WALK_FROM:
                refAss = moveFrom;
                loop = true;
                break;
            case RefAsset.WALK_TO:
                refAss = move;
                loop = true;
                break;
            case RefAsset.RUN:
                refAss = move;
                timeScale = 2.0f;
                loop = true;
                break;
            case RefAsset.ATTACK:
                refAss = attack;
                loop = false;
                break;
            case RefAsset.DEATH:
                refAss = death;
                loop = false;
                break;
            case RefAsset.HIT:
                refAss = hit;
                loop = false;
                break;
            case RefAsset.ROLL:
                refAss = roll;
                loop = true;
                break;
            case RefAsset.GROUNDPOUND:
                refAss = groundPound;
                loop = false;
                break;
        }

        skeletonAnimation.timeScale = timeScale;

        if (refAss != null)
            skeletonAnimation.state.SetAnimation(0, refAss, loop);
        else
            Debug.Log("RefAss == null, for: " + refAsset);
    }

    //=========================|   SetDirection()   |=======================================
    public void SetDirection(float x)
    {
        if (x == 0 || Mathf.Abs(x) != 1.0f)
            Debug.Log("WARNING: SetDirection() called with value of: " + x);

        tf.localScale = new Vector3(x * scaleX, 1, 1) * scale;
        dir = x;
    }

}