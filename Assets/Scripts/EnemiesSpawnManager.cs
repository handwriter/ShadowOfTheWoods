using System;
using System.Collections;
using System.Collections.Generic;
using UHFPS.Runtime;
using UnityEngine;

public class EnemiesSpawnManager : Singleton<EnemiesSpawnManager>
{
    [Serializable]
    public class EnemySpawnData
    {
        public enum SpawnTypeEnum { AtStart, RegularInstance, TriggeredInstance }
        public GameObject EnemyPrefab;
        public Transform SpawnPoint;
        public float SpawnTimeFromStart;
        public SpawnTypeEnum SpawnType;
        [HideInInspector]
        public float TimeFromSpawn;
        public GameObject Instance;
    }

    public enum EnemyType { Domovoy }

    public EnemySpawnData[] SpawnData;
    private float _timeFromStart;
    private List<EnemySpawnData> _dataParsed = new List<EnemySpawnData>();
    private List<EnemySpawnData> dataToDelete = new List<EnemySpawnData>();
    public int DomovoyDataIndex;

    private void Awake()
    {
        foreach (EnemySpawnData data in SpawnData) _dataParsed.Add(data);
    }

    public void SpawnDomovoy()
    {
        if (!SpawnData[DomovoyDataIndex].Instance)
        {
            GameObject newDomovoy = SpawnEnemyObj(SpawnData[DomovoyDataIndex]);
            SpawnData[DomovoyDataIndex].Instance = newDomovoy;
            newDomovoy.SetActive(true);
        }
    }

    public GameObject SpawnEnemyObj(EnemySpawnData data)
    {
        GameObject newEnemy = Instantiate(data.EnemyPrefab);
        newEnemy.transform.position = PlayerManager.Instance.CalculateRayStartPoint() + new Vector3(0, 5, 0);
        return newEnemy;
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
