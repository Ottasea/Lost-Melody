using System.Collections;
using UnityEngine;

public class PolarBear : MonoBehaviour
{
    //========================|   Variables   |=================================================
    [SerializeField] Transform tf;
    [SerializeField] Audio_PolarBear audioScript;
    [SerializeField] Enemies_Spine2D spine2d;

    [System.NonSerialized] public Transform enemy;

    const float speed_walk = 4.0f;
    const float mult_run = 1.5f;
    const float mult_attack = 0.5f;
    const float mult_roll = 2.5f;

    const float distance_charge = 10.0f;
    const float distance_attack = 1.0f;
    const float distance_margin = 0.9f;

    const float duration_hit = 1.0f;
    const float duration_attack = 0.4f;
    const float duration_cancelRetreat_min = 2.0f;
    const float duration_cancelRetreat_max = 5.0f;
    const float duration_die = 1.0f;
    const float duration_roll = 2.0f;

    float duration_cancelRetreat;

    float timer;

    const float damage_slash = 30.0f;
    const float damage_pound = 60.0f;
    const float damage_roll = 20.0f;

    public enum State { Walk_Toward, Walk_Away, Run, Slash, GroundPound, Roll_Transition, Roll, Hit, Die, Victory, KnockBack };
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
        if (state == State.Walk_Toward || state == State.Walk_Away || state == State.Run || state == State.Roll)
        {
            //-------------   Move()   ----------------------
            Move();

            //-------------   If within range, change state   ----------------------
            if (state == State.Walk_Toward || state == State.Run)
            {
                float distance = Vector3.Distance(enemy.position, tf.position);
                if (state == State.Walk_Toward && distance <= distance_charge)  // A - If walking towards, maybe charge?
                {
                    int binary = Random.Range(0, 2);
                    if (binary == 0)
                        SwitchState(State.Run);
                    else
                        StartCoroutine(Attack_Roll());
                }
                else if (state == State.Run && distance <= distance_attack)     // B - If charging, maybe attack?
                {
                    int binary = Random.Range(0, 2);
                    if (binary == 0)
                        StartCoroutine(Attack_Slash());
                    else
                        StartCoroutine(Attack_GroundPound());
                }
            }
            else if (state == State.Walk_Away)      // C - If retreating, maybe walk toward?
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
        Vector3 heading;
        if (state != State.Roll)
            heading = (enemy.position - tf.position).normalized;
        else
            heading = Vector3.right * tf.localScale.x;

        //-------------   Direction - Look   -------------------------------------
        if (state == State.Walk_Toward || state == State.Walk_Away)
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
        else if (state == State.Slash)
            speed *= mult_attack;
        else if (state == State.Roll)
            speed *= mult_roll;

        //-------------   Translate   -------------------------------------
        float x = heading.x * speed * Time.deltaTime;
        tf.Translate(x, 0, 0);

        //-------------   Raycast Onto Terrain   -------------------------------------
        RaycastOntoTerrain.RaycastOnto2dTerrain(tf);
    }

    //========================|   IEnumerator - Attack()   |=================================================
    IEnumerator Attack_Slash()
    {
        SwitchState(State.Slash);

        //-------------   While Loop - 1st half   -------------------------------------
        float t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime / duration_attack;

            yield return null;

            Move();
        }

        if (state != State.Slash)
            yield break;

        //-------------   Apply Damage if within range   -------------------------------------
        float distance = Vector3.Distance(enemy.position, tf.position);

        if (distance < distance_attack)
            enemy.GetComponentInChildren<HitPoints>().Hit(damage_slash, tf.position);

        //-------------   While Loop - 2nd half  -------------------------------------
        while (t < 1.0f)
        {
            t += Time.deltaTime / duration_attack;

            yield return null;

            Move();
        }

        if (state != State.Slash)
            yield break;

        //-------------   Finish   -------------------------------------
        SwitchState(State.Walk_Away);
    }

    //========================|   IEnumerator - Attack_Roll()   |=================================================
    IEnumerator Attack_Roll()
    {
        SwitchState(State.Roll_Transition);

        yield return new WaitForSeconds(1.0f);

        SwitchState(State.Roll);

        bool hasAppliedDamage = false;

        while (true)
        {
            if (!hasAppliedDamage)
                if (Vector3.Distance(tf.position, enemy.position) < 0.2f)
                {
                    enemy.GetComponentInChildren<HitPoints>().Hit(damage_roll, tf.position);
                    hasAppliedDamage = true;
                }

            if (state != State.Roll)
                yield break;
            else if (tf.localScale.x / Mathf.Abs(tf.localScale.x) != (enemy.position.x > tf.position.x ? 1 : -1) && Mathf.Abs(enemy.position.x - tf.position.x) > 1.0f)
                SwitchState(State.Walk_Away);

            yield return null;
        }
    }

    //========================|   IEnumerator - Attack_GroundPound()   |=================================================
    IEnumerator Attack_GroundPound()
    {
        SwitchState(State.GroundPound);

        //-------------   While Loop - 1st half   -------------------------------------
        float t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime / duration_attack;

            yield return null;

            Move();
        }

        if (state != State.GroundPound)
            yield break;

        //-------------   Apply Damage if within range   -------------------------------------
        float distance = Vector3.Distance(enemy.position, tf.position);

        if (distance < distance_attack)
            enemy.GetComponentInChildren<HitPoints>().Hit(damage_pound, tf.position);

        //-------------   While Loop - 2nd half  -------------------------------------
        while (t < 1.0f)
        {
            t += Time.deltaTime / duration_attack;

            yield return null;

            Move();
        }

        if (state != State.GroundPound)
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
                audioScript.PlayFootsteps(Audio_PolarBear.ClipSteps.walk);
                break;
            case State.Walk_Away:
                spine2d.SetAnimation(Enemies_Spine2D.RefAsset.WALK_FROM);
                audioScript.PlayFootsteps(Audio_PolarBear.ClipSteps.walk);
                duration_cancelRetreat = Random.Range(duration_cancelRetreat_min, duration_cancelRetreat_max);
                timer = 0;
                break;
            case State.Run:
                spine2d.SetAnimation(Enemies_Spine2D.RefAsset.RUN);
                audioScript.PlayFootsteps(Audio_PolarBear.ClipSteps.run);
                break;
            case State.Slash:
                spine2d.SetAnimation(Enemies_Spine2D.RefAsset.ATTACK);
                audioScript.PlayFootsteps(Audio_PolarBear.ClipSteps.none);
                audioScript.PlayVocal(Audio_PolarBear.ClipVocal.growl);
                break;
            case State.Roll_Transition:
                spine2d.SetAnimation(Enemies_Spine2D.RefAsset.ROLL);
                audioScript.PlayFootsteps(Audio_PolarBear.ClipSteps.none);
                audioScript.PlayVocal(Audio_PolarBear.ClipVocal.growl);
                break;
            case State.Roll:
                audioScript.PlayFootsteps(Audio_PolarBear.ClipSteps.roll);
                break;
            case State.Hit:
                spine2d.SetAnimation(Enemies_Spine2D.RefAsset.HIT);
                audioScript.PlayFootsteps(Audio_PolarBear.ClipSteps.none);
                audioScript.PlayVocal(Audio_PolarBear.ClipVocal.pain);
                break;
            case State.Die:
                spine2d.SetAnimation(Enemies_Spine2D.RefAsset.DEATH);
                audioScript.PlayFootsteps(Audio_PolarBear.ClipSteps.none);
                audioScript.PlayVocal(Audio_PolarBear.ClipVocal.die);
                break;
            case State.Victory:
                Debug.Log("Boar has no victory state");
                break;
            case State.KnockBack:
                StopAllCoroutines();
                spine2d.SetAnimation(Enemies_Spine2D.RefAsset.HIT);
                audioScript.PlayFootsteps(Audio_PolarBear.ClipSteps.none);
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
        Destroy(this);
    }


    //========================|   Victory()   |=================================================
    public void Victory()
    {
        StopAllCoroutines();
        SwitchState(State.Victory);
    }

}