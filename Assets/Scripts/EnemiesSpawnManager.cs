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
        public enum SpawnTypeEnum { AtStart, RegularInstance, TriggeredInstance, AtStartByDistance }
        public GameObject EnemyPrefab;
        public Transform[] SpawnPoints;
        public float CloseDistance;
        public float SpawnTimeFromStart;
        public SpawnTypeEnum SpawnType;
        [HideInInspector]
        public float TimeFromSpawn;
        public GameObject Instance;
        public Transform DefaultPoint;
    }

    public enum EnemyType { Domovoy, WaterMonster }

    public EnemySpawnData[] SpawnData;
    private float _timeFromStart;
    private List<EnemySpawnData> _dataParsed = new List<EnemySpawnData>();
    private List<EnemySpawnData> dataToDelete = new List<EnemySpawnData>();
    public int DomovoyDataIndex;
    public int WaterMonsterDataIndex;
    public int LeshyDataIndex;

    private void Awake()
    {
        foreach (EnemySpawnData data in SpawnData) _dataParsed.Add(data);
    }

    public Vector3 GetDefaultEnemyPosition(int enemyIndex) => SpawnData[enemyIndex].DefaultPoint.position;

    public void SpawnDomovoy()
    {
        SpawnAndSetupEnemy(DomovoyDataIndex);
        SpawnData[DomovoyDataIndex].Instance.SetActive(true);
    }
    public void SpawnWaterMonster(Vector3 spawnPos)
    {
        SpawnAndSetupEnemy(WaterMonsterDataIndex);
        SpawnData[WaterMonsterDataIndex].Instance.transform.position = spawnPos;
        SpawnData[WaterMonsterDataIndex].Instance.GetComponent<WaterMonsterController>().StartPosition = spawnPos;
        SpawnData[WaterMonsterDataIndex].Instance.SetActive(true);
    }

    public void SpawnAndSetupEnemy(int enemyIndex)
    {
        if (!SpawnData[enemyIndex].Instance)
        {
            GameObject newDomovoy = CreateEnemyObj(SpawnData[enemyIndex]);
            SpawnData[enemyIndex].Instance = newDomovoy;
        }
    }

    public GameObject CreateEnemyObj(EnemySpawnData data)
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
                        newEnemy.transform.position = data.SpawnPoints[0].position;
                        dataToDelete.Add(data);
                    }
                    break;
                case EnemySpawnData.SpawnTypeEnum.RegularInstance:
                    if (!data.Instance)
                    {
                        data.Instance = Instantiate(data.EnemyPrefab);
                        data.Instance.transform.position = data.SpawnPoints[0].position;
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
                case EnemySpawnData.SpawnTypeEnum.AtStartByDistance:
                    if (_timeFromStart >= (data.SpawnTimeFromStart * 60) && !data.Instance)
                    {
                        Transform maxDistancePoint = data.SpawnPoints[0];
                        if (Vector3.Distance(data.SpawnPoints[0].position, PlayerManager.Instance.transform.position) < data.CloseDistance)
                        {
                            maxDistancePoint = data.SpawnPoints[1];
                        }

                        GameObject newEnemy = Instantiate(data.EnemyPrefab);
                        newEnemy.transform.position = maxDistancePoint.position;
                        data.Instance = newEnemy;
                        newEnemy.SetActive(true);
                        dataToDelete.Add(data);
                    }
                    break;
            }
            
        }

        foreach (EnemySpawnData data in dataToDelete) _dataParsed.Remove(data);
        dataToDelete.Clear();
    }
}
