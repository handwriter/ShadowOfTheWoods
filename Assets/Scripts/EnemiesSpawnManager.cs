using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AutoLOD;
using UnityEngine;

public class EnemiesSpawnManager : MonoBehaviour
{
    


    [Serializable]
    public class EnemySpawnData
    {
        public enum SpawnTypeEnum { AtStart, RegularInstance }
        public GameObject EnemyPrefab;
        public Transform SpawnPoint;
        public float SpawnTimeFromStart;
        public SpawnTypeEnum SpawnType;
        [HideInInspector]
        public float TimeFromSpawn;
        public GameObject Instance;
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
            switch (data.SpawnType)
            {
                case EnemySpawnData.SpawnTypeEnum.AtStart:
                    if (_timeFromStart >= data.SpawnTimeFromStart)
                    {
                        GameObject newEnemy = Instantiate(data.EnemyPrefab);
                        newEnemy.transform.position = data.SpawnPoint.position;
                        dataToDelete.Add(data);
                    }
                    break;
                case EnemySpawnData.SpawnTypeEnum.RegularInstance:
                    if (!data.Instance)
                    {
                        data.Instance = Instantiate(data.EnemyPrefab);
                        data.Instance.transform.position = data.SpawnPoint.position;
                        data.Instance.gameObject.SetActive(false);
                        data.TimeFromSpawn = 0;
                    }
                    else
                    {
                        if (!data.Instance.activeSelf && data.TimeFromSpawn >= data.SpawnTimeFromStart)
                        {
                            data.Instance.SetActive(true);
                        }
                    }
                    data.TimeFromSpawn += Time.deltaTime;
                    break;
            }
            
        }

        foreach (EnemySpawnData data in dataToDelete) _dataParsed.Remove(data);
        dataToDelete.Clear();
    }
}
