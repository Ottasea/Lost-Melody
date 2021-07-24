using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    //=====================|   Variables   |=====================================
    const float duration_knock = 0.5f;
    const float speed_knock = 1.0f;

    public static KnockBack Instance;


    //=====================|   Awake()   |=====================================
    private void Awake()
    {
        Instance = this;
    }


    //=====================|   IEnumerator - Knock()   |=====================================
    public IEnumerator Knock(Transform tf, Vector3 from, float force, HitPoints.EntityType entityType, bool raycast)
    {
        //-----------------------   Start - SwitchState, Calculations   -------------------------------------
        ApplyKnockbackState(entityType, tf);

        Vector3 velocity = (tf.position - from).normalized;
        velocity *= force * speed_knock;

        //Debug.Log("Force: " + force);
        float decel = Mathf.Clamp(duration_knock / force, 0.5f, 2.0f);
        //Debug.Log("Decel: " + decel);

        //-----------------------   Middle - Loop   -------------------------------------
        float t = 1;

        while (t >= 0)
        {
            if (tf == null)
                yield break;

            t -= Time.deltaTime / decel;
            tf.Translate(velocity * t * Time.deltaTime);

            if (raycast)    RaycastOntoTerrain.RaycastOnto2dTerrain(tf);

            yield return null;
        }

        //-----------------------   End - State switch   -------------------------------------
        switch (entityType)
        {
            case HitPoints.EntityType.Boar:
                Boar boar = tf.GetComponent<Boar>();
                if (boar.state == Boar.State.KnockBack)
                    boar.SwitchState(Boar.State.Walk_Away);
                break;
            case HitPoints.EntityType.Draugr:
                Draugr draugr = tf.GetComponent<Draugr>();
                if (draugr.state == Draugr.State.KnockBack)
                    draugr.SwitchState(Draugr.State.Walk_Away);
                break;
        }
    }


    //=====================|   ApplyKnockbackState()   |=====================================
    private void ApplyKnockbackState(HitPoints.EntityType entityType, Transform tf)
    {
        switch (entityType)
        {
            case HitPoints.EntityType.Boar:
                Boar b = tf.GetComponent<Boar>();
                if (b.state != Boar.State.Hit && b.state != Boar.State.Die)
                    b.SwitchState(Boar.State.KnockBack);
                break;
            case HitPoints.EntityType.Draugr:
                Draugr d = tf.GetComponent<Draugr>();
                if (d.state != Draugr.State.Hit && d.state != Draugr.State.Die)
                    d.SwitchState(Draugr.State.KnockBack);
                break;
        }
    }

}