using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //=========================|   Variables   |=======================================
    const float speed = 1100;
    const float jumpForce = 7;
    const float sprintMultiplier = 2.0f;

    [SerializeField] Transform tf;
    [SerializeField] Animator anim;
    [SerializeField] Transform art;

    public static bool canMove = false; //If we are grounded
    bool onGround = false;

    Vector2 velocity;

    public const string anim_idle = "IdleOrMoving";
    public const string anim_speed = "Speed";
    const string anim_jump_launch = "Jump - Start";
    const string anim_jump_land = "Jump - Land";
    const float duration_land = 0.25f;

    public enum PlayerState { idle, walking, sprinting, jump, jump_land }
    [System.NonSerialized] public PlayerState playerState;

    public static Movement Instance;


    //=========================|   Awake()   |=======================================
    private void Awake()
    {
        Instance = this;
    }

    //=========================|   Start()   |=======================================
    private void Start()
    {
        Attack_Melee.Instance.tf_dir = art;
    }

    //=========================|   Update()   |=======================================
    private void Update()
    {
        //-----------------------   1A - Check if we are on the ground   --------------------------------
        if (canMove)
        {
            //-----------------------   1 - Scale direction   --------------------------------
            art.localScale = new Vector3(1, 1, CursorDirectionRight());

            //-----------------------   2 - Horizontal Movement   --------------------------------
            Move();

            //-----------------------   3 - Jumping   --------------------------------
            if (Input.GetKeyDown(KeyCode.Space))
                Jump_Launch();
        }
        //-----------------------   1B - Check if we are on the ground   --------------------------------
        else if (!onGround && velocity.y < 0 && RaycastOntoTerrain.IsOnTerrain(tf, velocity.y * Time.deltaTime * 1.5f))
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

        if (x != 0)
        {
            x /= Mathf.Abs(x);

            bool sprint = Input.GetKey(KeyCode.LeftShift) && x == art.localScale.z && !Attack_Melee.Instance.attacking;

            if (sprint)
                x *= sprintMultiplier;

            playerState = sprint ? PlayerState.sprinting : PlayerState.walking;

            anim.SetFloat(anim_speed, x * art.localScale.z);

            x *= speed * Time.deltaTime;
        }
        else
        {
            anim.SetFloat(anim_speed, 0);
            playerState = PlayerState.idle;
        }

        velocity = new Vector2(x, velocity.y);
    }

    //=========================|   Jump_Launch()   |=======================================
    void Jump_Launch()
    {
        velocity = new Vector2(velocity.x * 0.75f, jumpForce);
        anim.SetBool(anim_idle, false);
        anim.SetTrigger(anim_jump_launch);
        playerState = PlayerState.jump;
        canMove = onGround = false;
    }

    //=========================|   Jump_Land()   |=======================================
    public IEnumerator Jump_Land()
    {
        onGround = true;
        anim.SetTrigger(anim_jump_land);
        playerState = PlayerState.jump_land;

        velocity = Vector2.zero;
        RaycastOntoTerrain.RaycastOnto2dTerrain(tf);

        yield return new WaitForSeconds(duration_land);

        anim.SetBool(anim_idle, true);
        playerState = PlayerState.idle;
        canMove = true;
    }

    //=========================|   EnableDisable()   |=======================================
    public void EnableDisable(bool enableDisable)
    {
        canMove = enableDisable;
        playerState = PlayerState.idle;

        if (!enableDisable)
            velocity = Vector2.zero;
    }

    //=========================|   CursorDirection()   |=======================================
    float CursorDirectionRight()
    {
        return Input.mousePosition.x > Screen.width / 2 ? 1.0f : -1.0f;
    }

}