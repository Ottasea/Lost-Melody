using System.Collections;
using UnityEngine;

public class Boar : MonoBehaviour
{
    //========================|   Variables   |=================================================
    [SerializeField] Transform tf;
    [SerializeField] Audio_Boar audioScript;
    [SerializeField] Enemies_Spine2D spine2d;

    /*
    const string anim_walkTo    = "Walk To";
    const string anim_walkFrom  = "Walk From";
    const string anim_run       = "Run";
    const string anim_attack    = "Attack";
    const string anim_hit       = "Hit";
    const string anim_die       = "Die";
    */

    [System.NonSerialized] public Transform enemy;

    const float speed_walk = 3.0f;
    const float mult_run = 2.0f;
    const float mult_attack = 0.5f;

    const float distance_charge = 10.0f;
    const float distance_attack = 1.0f;
    const float distance_margin = 0.9f;

    const float duration_hit = 1.0f;
    const float duration_attack = 0.4f;
    const float duration_cancelRetreat_min = 1.0f;
    const float duration_cancelRetreat_max = 3.0f;
    const float duration_die = 1.0f;

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
        
        if (Vector3.Distance(enemy.position, transform.position) > Enemies_Shared.range)
            return;

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
        tf.localScale = new Vector3(heading.x > 0 ? 1 : -1, 1, 1);

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
        Vector3 pos = tf.position;
        tf.Translate(x, 0, 0);

        //-------------   Raycast Onto Terrain   -------------------------------------
        if (!RaycastOntoTerrain.RaycastOnto2dTerrain(tf))
        {
            tf.position = pos;
            SwitchState(State.Walk_Toward);
        }
    }


    //========================|   IEnumerator - Attack()   |=================================================
    IEnumerator Attack()
    {
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
    }


    //========================|   SwitchState()   |=================================================
    public void SwitchState(State _state)
    {
        state = _state;

        switch (state)
        {
            case State.Walk_Toward:
                spine2d.SetAnimation(Enemies_Spine2D.RefAsset.WALK_TO);
                audioScript.PlayFootsteps(Audio_Boar.ClipSteps.walk);
                break;
            case State.Walk_Away:
                spine2d.SetAnimation(Enemies_Spine2D.RefAsset.WALK_FROM);
                audioScript.PlayFootsteps(Audio_Boar.ClipSteps.walk);
                duration_cancelRetreat = Random.Range(duration_cancelRetreat_min, duration_cancelRetreat_max);
                timer = 0;
                break;
            case State.Run:
                spine2d.SetAnimation(Enemies_Spine2D.RefAsset.RUN);
                audioScript.PlayFootsteps(Audio_Boar.ClipSteps.run);
                break;
            case State.Attack:
                spine2d.SetAnimation(Enemies_Spine2D.RefAsset.ATTACK);
                audioScript.PlayFootsteps(Audio_Boar.ClipSteps.none);
                audioScript.PlayVocal(Audio_Boar.ClipVocal.growl);
                break;
            case State.Hit:
                spine2d.SetAnimation(Enemies_Spine2D.RefAsset.HIT);
                audioScript.PlayFootsteps(Audio_Boar.ClipSteps.none);
                audioScript.PlayVocal(Audio_Boar.ClipVocal.pain);
                break;
            case State.Die:
                spine2d.SetAnimation(Enemies_Spine2D.RefAsset.DEATH);
                audioScript.PlayFootsteps(Audio_Boar.ClipSteps.none);
                audioScript.PlayVocal(Audio_Boar.ClipVocal.die);
                break;
            case State.Victory:
                Debug.Log("Boar has no victory state");
                break;
            case State.KnockBack:
                StopAllCoroutines();
                spine2d.SetAnimation(Enemies_Spine2D.RefAsset.HIT);
                audioScript.PlayFootsteps(Audio_Boar.ClipSteps.none);
                break;
        }
    }


    //========================|   Hit()   |=================================================
    public void Hit()
    {
        StopAllCoroutines();
        StartCoroutine(Hit_Coroutine());
    }


    //========================|   IEnumerator - Hit()   |=================================================
    private IEnumerator Hit_Coroutine()
    {
        SwitchState(State.Hit);

        yield return new WaitForSeconds(duration_hit);

        if (state != State.Hit)
        {
            Debug.Log("ERROR: state != State.Hit, state == " + state);
            yield break;
        }

        SwitchState(State.Walk_Away);
    }


    //========================|   Die()   |=================================================
    public void Die()
    {
        StopAllCoroutines();
        StartCoroutine(Die_Coroutine());
    }


    //========================|   IEnumerator - Die_Coroutine()   |=================================================
    private IEnumerator Die_Coroutine()
    {
        Destroy(GetComponent<HitPoints>());

        SwitchState(State.Die);

        yield return new WaitForSeconds(duration_die);

        if (state != State.Die)
        {
            Debug.Log("ERROR: state != State.Die");
            yield break;
        }

        Destroy(audioScript);
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<Enemies_Shared>().enabled = false;
        Destroy(this);
    }


    //========================|   Victory()   |=================================================
    public void Victory()
    {
        StopAllCoroutines();
        SwitchState(State.Victory);
    }

}