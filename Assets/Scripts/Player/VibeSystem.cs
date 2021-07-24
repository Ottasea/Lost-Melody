using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibeSystem : MonoBehaviour
{
    //=======================|   Variables   |=================================
    [SerializeField] Animator anim;

    const string anim_pulse = "Pulse";
    const float range = 8.0f;
    const float relativeScale = 0.35f;
    const float speed = 1.0f;
    const float yOffset = 0.67f;

    public const float vibeDmgMult = 3.0f;

    bool currentlyPulsing = false;

    public const int vibeCount = 4;
    public enum Vibe { Red, Green, Blue, Yellow, NONE }
    public static Vibe ourVibe;
    [SerializeField] UnityEngine.UI.Image vibeNotesImg;


    //=======================|   Start()   |=================================
    private void Start()
    {
        ourVibe = Vibe.Green;
        vibeNotesImg.color = Color.green;
    }

    //=======================|   Update()   |=================================
    void Update()
    {
        //-----------------   Send out vibe pulse  -------------------------------
        if (Input.GetKeyDown(KeyCode.Q) && !currentlyPulsing)
            StartCoroutine(VibePulse());

        //-----------------   Change vibe  -------------------------------
        if (Input.GetKeyDown(KeyCode.UpArrow))
            SwitchVibe(Vibe.Red);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            SwitchVibe(Vibe.Green);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            SwitchVibe(Vibe.Blue);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            SwitchVibe(Vibe.Yellow);
    }

    //=======================|   SwitchVibe()   |======================================
    private void SwitchVibe(Vibe vibe)
    {
        ourVibe = vibe;

        switch (vibe)
        {
            case Vibe.Red:
                vibeNotesImg.color = Color.red;
                break;
            case Vibe.Green:
                vibeNotesImg.color = Color.green;
                break;
            case Vibe.Blue:
                vibeNotesImg.color = Color.blue;
                break;
            case Vibe.Yellow:
                vibeNotesImg.color = Color.yellow;
                break;
        }

        Audio_Player.Instance.PlayClip_VibeChange(vibe);
    }

    //=======================|   IEnumerator - VibePulse()   |======================================
    const float duration_pulse = 1.0f;
    private IEnumerator VibePulse()
    {
        //-------------------------  Begin  ---------------------------------
        anim.SetTrigger(anim_pulse);
        Audio_Player.Instance.PlayClip_Action(Audio_Player.ActionClip.Sonar);
        currentlyPulsing = true;

        Collider2D[] colArr = Physics2D.OverlapCircleAll(transform.position, range);

        foreach (Collider2D col in colArr)
            if (col.GetComponent<HitPoints>() && col.tag != HornSystem.tag_player)
                StartCoroutine(ShowVibe(col.transform));

        //-------------------------  Middle  ---------------------------------
        yield return new WaitForSeconds(duration_pulse);

        //-------------------------  End  ---------------------------------
        currentlyPulsing = false;
    }

    //=======================|   IEnumerator - ShowVibe()   |=================================
    const float duration_display = 2.0f;
    const float duration_wait_max = 0.3f;
    private IEnumerator ShowVibe(Transform tf)
    {
        //-------------  0 - BEFORE begin  ---------------
        float distance = Vector2.Distance(tf.position, transform.position);
        float duration_wait = (distance / range) * duration_wait_max;
        yield return new WaitForSeconds(duration_wait);

        //-------------  1 - Begin  ---------------
        //---------  Create Obj, play anim  --------------------------------
        Vector3 pos = tf.position + Vector3.up * yOffset;
        GameObject vibeObj = Instantiate(anim.gameObject, pos, Quaternion.identity, tf);
        vibeObj.transform.localScale = Vector3.one * relativeScale;
        vibeObj.GetComponent<Animator>().SetTrigger(anim_pulse);
        vibeObj.GetComponent<Animator>().speed = speed;

        //---------  Color  --------------------------------
        Color color =   Color.white;
        switch (tf.GetComponent<Enemies_Shared>().vibe)
        {
            case Vibe.Red:
                color = Color.red;
                break;
            case Vibe.Blue:
                color = Color.blue;
                break;
            case Vibe.Green:
                color = Color.green;
                break;
            case Vibe.Yellow:
                color = Color.yellow;
                break;
        }
        vibeObj.GetComponent<SpriteRenderer>().color = color;

        //-------------  2 - Middle: Wait to end  ---------------
        yield return new WaitForSeconds(duration_display);

        //-------------  3 - End  ---------------
        Destroy(vibeObj); 
    }

}