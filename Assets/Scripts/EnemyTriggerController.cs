using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTriggerController : MonoBehaviour
{
    public EnemiesSpawnManager.EnemyType EnemyType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            switch (EnemyType)
            {
                case EnemiesSpawnManager.EnemyType.Domovoy:
                    EnemiesSpawnManager.Instance.SpawnDomovoy();
                    break;
            }
        }
    }
}
