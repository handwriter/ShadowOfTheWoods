using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesSpawnManager : MonoBehaviour
{
    [Serializable]
    public class EnemySpawnData
    {
        public GameObject EnemyPrefab;
        public Transform SpawnPoint;
        public float SpawnTimeFromStart;
    }

    public EnemySpawnData[] SpawnData;
    private float _timeFromStart;
    private List<EnemySpawnData> _dataParsed = new List<EnemySpawnData>();
    private List<EnemySpawnData> dataToDelete = new List<EnemySpawnData>();

    private void Awake()
    {
        foreach (EnemySpawnData data in SpawnData) _dataParsed.Add(data);
    }

    void Update()
    {
        _timeFromStart += Time.deltaTime;
        
        foreach (EnemySpawnData data in _dataParsed)
        {
            if (_timeFromStart >= data.SpawnTimeFromStart)
            {
                GameObject newEnemy = Instantiate(data.EnemyPrefab);
                newEnemy.transform.position = data.SpawnPoint.position;
                dataToDelete.Add(data);
            }
        }

        foreach (EnemySpawnData data in dataToDelete) _dataParsed.Remove(data);
        dataToDelete.Clear();
    }
}
