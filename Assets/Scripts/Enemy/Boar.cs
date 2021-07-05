using System.Collections;
using UnityEngine;

public class Boar : MonoBehaviour
{
    //========================|   Variables   |=================================================
    [SerializeField] Animator anim;
    [SerializeField] Transform tf;
    [SerializeField] Audio_Boar audioScript;

    const string anim_walkTo    = "Walk To";
    const string anim_walkFrom  = "Walk From";
    const string anim_run       = "Run";
    const string anim_attack    = "Attack";
    const string anim_hit       = "Hit";
    const string anim_die       = "Die";

    [System.NonSerialized] public Transform enemy;

    const float speed_walk  = 3.0f;
    const float mult_run    = 2.0f;
    const float mult_attack = 0.5f;

    const float distance_charge = 10.0f;
    const float distance_attack = 1.0f;
    const float distance_margin = 0.9f;

    const float duration_hit = 1.0f;
    const float duration_attack = 0.4f;
    const float duration_cancelRetreat_min = 1.0f;
    const float duration_cancelRetreat_max = 3.0f;
    const float duration_die = 1.0f;

    bool cr_hit = false;
    bool cr_attack = false;

    float duration_cancelRetreat;

    float timer;

    const float damage = 10.0f;

    public enum State { Walk_Toward, Walk_Away, Run, Attack, Hit, Die, Victory, KnockBack };
    [System.NonSerialized] public State state;


    //========================|   Start()   |=================================================
    void Start()
    {
        enemy = GameObject.FindGameObjectWithTag("Player").transform;

        SwitchState(State.Walk_Toward);
    }


    //========================|   Update()   |=================================================
    void Update()
    {
        timer += Time.deltaTime;

        //-------------   If walking or running   ----------------------
        if (state == State.Walk_Toward || state == State.Walk_Away || state == State.Run)
        {
            //-------------   Move()   ----------------------
            Move();

            //-------------   If within range, change state   ----------------------
            if (state == State.Walk_Toward || state == State.Run)
            {
                float distance = Vector3.Distance(enemy.position, tf.position);
                if (state == State.Walk_Toward && distance <= distance_charge)              // A - If walking towards, maybe charge?
                    SwitchState(State.Run);
                else if (state == State.Run && distance <= distance_attack)                 // B - If charging, maybe attack?
                    StartCoroutine(Attack());
            }
            else if (state == State.Walk_Away)    // C - If retreating, maybe walk toward?
            {
                if (timer >= duration_cancelRetreat)
                    SwitchState(State.Walk_Toward);
            }
        }
    }


    //========================|   Move()   |=================================================
    private void Move()
    {
        //-------------   Heading   -------------------------------------
        Vector3 heading = (enemy.position - tf.position).normalized;

        //-------------   Direction - Look   -------------------------------------
        tf.localScale = new Vector3(heading.x, 1, 1);

        //-------------   Direction or Margin   -------------------------------------
        if (state == State.Walk_Away)
            heading = -heading;
        else if (heading.magnitude <= distance_margin)
            return;

        //-------------   Speed   -------------------------------------
        float speed = speed_walk;

        if (state == State.Run)
            speed *= mult_run;
        else if (state == State.Attack)
            speed *= mult_attack;

        //-------------   Translate   -------------------------------------
        float x = heading.x * speed * Time.deltaTime;
        tf.Translate(x, 0, 0);

        //-------------   Raycast Onto Terrain   -------------------------------------
        RaycastOntoTerrain.RaycastOnto2dTerrain(tf);
    }


    //========================|   IEnumerator - Attack()   |=================================================
    IEnumerator Attack()
    {
        if (cr_attack)
            StopCoroutine(Attack());

        cr_attack = true;

        SwitchState(State.Attack);

        //-------------   While Loop - 1st half   -------------------------------------
        float t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime / duration_attack;

            yield return null;

            Move();
        }

        if (state != State.Attack)
            yield break;

        //-------------   Apply Damage if within range   -------------------------------------
        float distance = Vector3.Distance(enemy.position, tf.position);

        if (distance < distance_attack)
            enemy.GetComponentInChildren<HitPoints>().Hit(damage, tf.position);

        //-------------   While Loop - 2nd half  -------------------------------------
        while (t < 1.0f)
        {
            t += Time.deltaTime / duration_attack;

            yield return null;

            Move();
        }

        if (state != State.Attack)
            yield break;

        //-------------   Finish   -------------------------------------
        SwitchState(State.Walk_Away);
        cr_attack = false;
    }

    //========================|   SwitchState()   |=================================================
    public void SwitchState(State _state)
    {
        state = _state;

        switch (state)
        {
            case State.Walk_Toward:
                anim.SetTrigger(anim_walkTo);
                audioScript.PlayFootsteps(Audio_Boar.ClipSteps.walk);
                break;
            case State.Walk_Away:
                anim.SetTrigger(anim_walkFrom);
                audioScript.PlayFootsteps(Audio_Boar.ClipSteps.walk);
                duration_cancelRetreat = Random.Range(duration_cancelRetreat_min, duration_cancelRetreat_max);
                timer = 0;
                break;
            case State.Run:
                anim.SetTrigger(anim_run);
                audioScript.PlayFootsteps(Audio_Boar.ClipSteps.run);
                break;
            case State.Attack:
                anim.SetTrigger(anim_attack);
                audioScript.PlayFootsteps(Audio_Boar.ClipSteps.none);
                audioScript.PlayVocal(Audio_Boar.ClipVocal.growl);
                break;
            case State.Hit:
                anim.SetTrigger(anim_hit);
                audioScript.PlayFootsteps(Audio_Boar.ClipSteps.none);
                audioScript.PlayVocal(Audio_Boar.ClipVocal.pain);
                break;
            case State.Die:
                anim.SetTrigger(anim_die);
                audioScript.PlayFootsteps(Audio_Boar.ClipSteps.none);
                audioScript.PlayVocal(Audio_Boar.ClipVocal.die);
                break;
            case State.Victory:
                Debug.Log("Boar has no victory state");
                break;
            case State.KnockBack:
                StopAllCoroutines();
                anim.SetTrigger(anim_hit);
                audioScript.PlayFootsteps(Audio_Boar.ClipSteps.none);
                break;
        }
    }

    //========================|   IEnumerator - Hit()   |=================================================
    public IEnumerator Hit()
    {
        if (cr_hit)
            StopCoroutine(Hit());

        cr_hit = true;

        SwitchState(State.Hit);
        StopCoroutine(Attack());

        yield return new WaitForSeconds(duration_hit);

        if (state != State.Hit)
        {
            Debug.Log("ERROR: state != State.Hit");
            yield break;
        }

        SwitchState(State.Walk_Away);
        cr_hit = false;
    }

    //========================|   IEnumerator - Die()   |=================================================
    public IEnumerator Die()
    {
        StopCoroutine(Hit());
        StopCoroutine(Attack());
        cr_hit = false;
        cr_attack = false; 
        
        Destroy(GetComponent<HitPoints>());

        SwitchState(State.Die);

        yield return new WaitForSeconds(duration_die);

        if (state != State.Die)
        {
            Debug.Log("ERROR: state != State.Die");
            yield break;
        }

        Destroy(GetComponent<Audio_Boar>());
        Destroy(this);
    }

    //========================|   Victory()   |=================================================
    public void Victory()
    {
        StopAllCoroutines();
        SwitchState(State.Victory);
    }

}