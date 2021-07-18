using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //=========================|   Variables   |=======================================
    const float speed = 2000;
    const float jumpForce = 4;
    const float sprintMultiplier = 2.0f;

    [SerializeField] Transform tf;

    public static bool canMove = false; //If we are grounded
    bool onGround = false;

    Vector2 velocity;

    public const string anim_idle = "IdleOrMoving";
    public const string anim_speed = "Speed";
    const string anim_jump_launch = "Jump - Start";
    const string anim_jump_land = "Jump - Land";
    const float duration_land = 0.35f;
    public enum PlayerState { idle, walking, sprinting, jump, jump_land }
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

    }

    //=========================|   Update()   |=======================================
    private void Update()
    {
        //-----------------------   1A - Check if we are on the ground   --------------------------------
        if (canMove)
        {
            //-----------------------   2 - Horizontal Movement   --------------------------------
            Move();

            //-----------------------   3 - Jumping   --------------------------------
            if (Input.GetKeyDown(KeyCode.Space))
                Jump_Launch();
        }
        //-----------------------   1B - Check if we are on the ground   --------------------------------
        else if (!onGround && velocity.y < 0 && RaycastOntoTerrain.IsOnTerrain(tf, velocity.y * Time.deltaTime * 4.0f))
            StartCoroutine(Jump_Land());
        //-----------------------   1C - Apply Gravity   --------------------------------
        else if (!onGround)
            velocity += Vector2.up * Physics.gravity.y * Time.deltaTime;

        //-----------------------   2 - Apply velocity   --------------------------------
        tf.Translate(new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime);

        if (canMove)
            RaycastOntoTerrain.RaycastOnto2dTerrain(tf);
    }

    //=========================|   Move()   |=======================================
    void Move()
    {
        float x = Input.GetAxis("Horizontal");

        if (x != prevX || SpineAnim_Player.prevAsset != SpineAnim_Player.RefAsset.RUN)
        {
            SpineAnim_Player.RefAsset idleOrRun = x == 0 ? SpineAnim_Player.RefAsset.IDLE : SpineAnim_Player.RefAsset.RUN;
            SpineAnim_Player.Instance.SetAnimation(idleOrRun);
            prevX = x;
        }

        if (x != 0)
        {
            x /= Mathf.Abs(x);
            SpineAnim_Player.Instance.SetDirection(x);

            /*
            bool sprint = Input.GetKey(KeyCode.LeftShift) && x == SpineAnim_Player.dir && !Attack_Melee.Instance.attacking;

            if (sprint)
                x *= sprintMultiplier;

            playerState = sprint ? PlayerState.sprinting : PlayerState.walking;
            */
            playerState = PlayerState.walking;

            x *= speed * Time.deltaTime;
        }
        else
        {
            playerState = PlayerState.idle;
            SpineAnim_Player.Instance.SetDirection(CursorDirectionRight());
        }

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
        RaycastOntoTerrain.RaycastOnto2dTerrain(tf);

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
        playerState = PlayerState.idle;

        if (!enableDisable)
        {
            velocity = Vector2.zero;
            SpineAnim_Player.Instance.SetAnimation(SpineAnim_Player.RefAsset.IDLE);
        }
    }

    //=========================|   CursorDirection()   |=======================================
    float CursorDirectionRight()
    {
        return Input.mousePosition.x > Screen.width / 2 ? 1.0f : -1.0f;
    }

}