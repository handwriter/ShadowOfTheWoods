using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookHeadController : MonoBehaviour
{
    public GameObject EnemyAttackSphereCenter;
    public float AttackSphereRadius;
    public int CounterUndamageAttacks;
    public SkinnedMeshRenderer[] BodyMeshRenderer;

    public int AttackCount;

    private void Update()
    {
        foreach (SkinnedMeshRenderer meshRnd in BodyMeshRenderer) meshRnd.enabled = ModelController.IsUsingNightVision;
    }
}
