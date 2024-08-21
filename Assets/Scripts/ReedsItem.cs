using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UHFPS.Runtime;

public class ReedsItem : MonoBehaviour, IDamagable
{
    private int hitCounter = 0;

    public void ApplyDamageMax(Transform sender = null)
    {
        OnDamage();
    }

    public void OnApplyDamage(int damage, Transform sender = null)
    {
        OnDamage();
    }

    public void OnDamage()
    {
        hitCounter += 1;
        if (hitCounter >= 3) Destroy(gameObject);
    }
}
