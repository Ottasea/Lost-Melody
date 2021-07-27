using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //=========================|   Variables   |=======================================
    const float speed = 2000;
    const float jumpForce = 11;
    //const float sprintMultiplier = 2.0f;

    Vector3 spawnPos;

    [SerializeField] Transform tf;

    public static bool canMove = false; //If we are grounded
    bool onGround = false;

    Vector2 velocity;

    public const string anim_idle = "IdleOrMoving";
    public const string anim_speed = "Speed";
    const string anim_jump_launch = "Jump - Start";
    const string anim_jump_land = "Jump - Land";
    const float duration_land = 0.35f;
    public enum PlayerState { idle, walking, pushing, jump, jump_land }
    [System.NonSerialized] public PlayerState playerState;

    public static Movement Instance;

    float prevX = 0;


    //=========================|   Awake()   |=======================================
    private void Awake()
    {
        Instance = this;
    }

    //=========================|   Start()   |=======================================
    private void Start()
    {
        spawnPos = tf.position;
    }

    //=========================|   Update()   |=======================================
    private void Update()
    {
        //-----------------------   1 - Horizontal Movement   --------------------------------
        Move();

        //-----------------------   1A - Check if we are on the ground   --------------------------------
        if (onGround)
        {
            //-----------------------   3 - Jumping   --------------------------------
            if (Input.GetKeyDown(KeyCode.Space))
                Jump_Launch();

            /*
            if (!RaycastOntoTerrain.IsOnTerrain(tf, 1.0f))
            {
                onGround = false;
            }
            */
        }
        //-----------------------   1B - Check if we are on the ground   --------------------------------
        else if (!onGround && velocity.y < 0/* && RaycastOntoTerrain.IsOnTerrain(tf, velocity.y * Time.deltaTime * 4.0f)*/)
            StartCoroutine(Jump_Land());
        //-----------------------   1C - Apply Gravity   --------------------------------
        else if (!onGround)
            velocity += Vector2.up * Physics.gravity.y * Time.deltaTime;

        Vector3 translation = new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime;
        tf.Translate(translation);

        //-----------------------   3 - PushPullMove, RaycastOnto2dTerrain   --------------------------------
        if (canMove)
        {
            PushPull.Instance.Move();
           // RaycastOntoTerrain.RaycastOnto2dTerrain(tf);
        }
    }

    //=========================|   Move()   |=======================================
    void Move()
    {
        //---------------------  Get normalised x-value  --------------------------
        float x = Input.GetAxis("Horizontal");
        if (x != 0)
            x /= Mathf.Abs(x);  // Have to do this before GetRunPushOrPull(), but can't do it if x == 0, or we divide by 0

        if (playerState != PlayerState.jump && playerState != PlayerState.jump_land)
        {
            //---------------------  Get animation  --------------------------
            SpineAnim_Player.RefAsset idleRunPushPull;

            if (x == 0 && !PushPull.isPushing)
                idleRunPushPull = SpineAnim_Player.RefAsset.IDLE;
            else
                idleRunPushPull = PushPull.Instance.GetRunPushOrPull(x);

            //---------------------  Set animation  --------------------------
            if (SpineAnim_Player.prevAsset != idleRunPushPull && !SpineAnim_Player.IsPerformingAction())
                SpineAnim_Player.Instance.SetAnimation(idleRunPushPull);

            //---------------------  Set prevX  --------------------------
            prevX = x;

            //---------------------  Direction (art localScale), playerState & speed multiplication  ---------------------
            if (idleRunPushPull == SpineAnim_Player.RefAsset.IDLE)
            {
                playerState = PlayerState.idle;
                SpineAnim_Player.Instance.SetDirection(SpineAnim_Player.CursorDirectionRight());
            }
            else
            {
                if (idleRunPushPull == SpineAnim_Player.RefAsset.PUSH || idleRunPushPull == SpineAnim_Player.RefAsset.PULL)
                {
                    SpineAnim_Player.Instance.SetDirection(PushPull.Instance.DirectionToPushedObj());
                    playerState = PlayerState.pushing;
                }
                else
                {
                    SpineAnim_Player.Instance.SetDirection(x);
                    playerState = PlayerState.walking;
                }
            }
        }

        x *= speed * Time.deltaTime;

        //---------------------  Assign velocity  ---------------------
        velocity = new Vector2(x, velocity.y);
    }

    //=========================|   Jump_Launch()   |=======================================
    void Jump_Launch()
    {
        velocity = new Vector2(velocity.x * 0.75f, jumpForce);
        SpineAnim_Player.Instance.SetAnimation(SpineAnim_Player.RefAsset.JUMP);
        playerState = PlayerState.jump;
        canMove = onGround = false;
    }

    //=========================|   Jump_Land()   |=======================================
    IEnumerator Jump_Land()
    {
        onGround = true;
        SpineAnim_Player.Instance.SetTimeScale(2.0f);
        SpineAnim_Player.Instance.SetAnimation(SpineAnim_Player.RefAsset.JUMP_LAND);
        playerState = PlayerState.jump_land;

        velocity = Vector2.zero;

        yield return new WaitForSeconds(duration_land);

        SpineAnim_Player.Instance.SetAnimation(SpineAnim_Player.RefAsset.IDLE);
        SpineAnim_Player.Instance.SetTimeScale(1.0f);

        playerState = PlayerState.idle;
        canMove = true;
    }

    //=========================|   EnableDisable()   |=======================================
    public void EnableDisable(bool enableDisable)
    {
        canMove = enableDisable;

        if (!enableDisable)
            velocity = Vector2.zero;

        /*
        playerState = PlayerState.idle;

        if (!enableDisable)
        {
            velocity = Vector2.zero;
            SpineAnim_Player.Instance.SetAnimation(SpineAnim_Player.RefAsset.IDLE);
        }
        */
    }

    //=========================|   Respawn()   |=======================================
    public void Respawn()
    {
        tf.position = spawnPos;
    }

}