using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPull : MonoBehaviour
{
    //==========================|   Variables   |======================================
    [SerializeField] GameObject uiPressToStart;
    [SerializeField] GameObject uiPressToStop;
    PushPullObj maybePushObj;
    Transform pushedObj;
    float pushObjOffsetX;

    public static bool isPushing = false;

    public static PushPull Instance;


    //==========================|   Awake()   |======================================
    private void Awake()
    {
        Instance = this;
    }

    //==========================|   Start()   |======================================
    private void Start()
    {
        uiPressToStart.SetActive(false);
        uiPressToStop.SetActive(false);
    }

    //==========================|   Update()   |======================================
    private void Update()
    {
        if (maybePushObj != null && Input.GetKeyDown(KeyCode.W))
            Push_Start();
        else if (pushedObj != null && Input.GetKeyDown(KeyCode.W))
            Push_Stop();
    }

    //==========================|   GetRunPushOrPull()   |======================================
    public SpineAnim_Player.RefAsset GetRunPushOrPull(float x)
    {
        if (pushedObj == null)
            return SpineAnim_Player.RefAsset.RUN;
        else
        {
            float direction = DirectionToPushedObj();

            if (x == direction)
                return SpineAnim_Player.RefAsset.PUSH;
            else
                return SpineAnim_Player.RefAsset.PULL;
        }
    }

    //==========================|   DirectionToPushedObj()   |======================================
    public float DirectionToPushedObj(Transform _pushedObj = null)
    {
        if (pushedObj == null && _pushedObj == null)
            Debug.Log("ERROR: no pushedObj");

        Transform tF = _pushedObj == null ? pushedObj : _pushedObj;
        return tF.position.x >= transform.position.x ? 1 : -1;
    }

    //==========================|   Move()   |======================================
    public void Move()
    {
        if (pushedObj == null)
            return;

        pushedObj.position = new Vector3(transform.position.x + pushObjOffsetX, pushedObj.position.y, pushedObj.position.z);
        //RaycastOntoTerrain.RaycastOnto2dTerrain(pushedObj);
    }

    //==========================|   Collision - OnCollisionEnter2D()   |======================================
    void OnTriggerEnter2D(Collider2D col)
    {
        if (pushedObj != null)
        {
            //Debug.Log("return: pushedObj != null");
            return;
        }
        else if (col.GetComponent<PushPullObj>() == null)
        {
            //Debug.Log("return: Component PushPullObj == null");
            return;
        }
        else if (SpineAnim_Player.IsJumping() || SpineAnim_Player.IsPerformingAction()) 
        {
            //Debug.Log("return: IsJumping() or IsPerformingAction()");
            return;
        }

        uiPressToStart.SetActive(true);
        maybePushObj = col.GetComponent<PushPullObj>();

        //SpineAnim_Player.Instance.SetAnimation(SpineAnim_Player.RefAsset.PUSH);
    }


    //==========================|   Collision - OnCollisionExit2D()   |======================================
    private void OnTriggerExit2D(Collider2D col)
    {
        if (maybePushObj == null)
            return;
        else if (col.gameObject != maybePushObj.gameObject)
            return;

        uiPressToStart.SetActive(false);
        maybePushObj = null;
    }

    //==========================|   Push_Start()   |======================================
    private void Push_Start()
    {
        pushedObj = maybePushObj.transform;
        maybePushObj.Activate();
        uiPressToStart.SetActive(false);
        uiPressToStop.SetActive(true);
        isPushing = true;
        pushObjOffsetX = pushedObj.position.x - transform.position.x;
        maybePushObj = null;
    }

    //==========================|   Push_Stop()   |======================================
    private void Push_Stop()
    {
        pushedObj.GetComponent<PushPullObj>().Deactivate();
        pushedObj = null;
        uiPressToStop.SetActive(false);
        isPushing = false;
    }

}