using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemiesSpawnManager;

public static class ModelController
{
    public static bool IsUsingNightVision = false;
    public static bool IsUsingFlashlight = false;
    private const string EnemySavePrefix = "Enemy_";

    public static bool CheckEnemySeen(EnemyType type)
    {
        string enemyType = Enum.GetName(typeof(EnemyType), type);
        return PlayerPrefs.HasKey(EnemySavePrefix + enemyType);
    }

    public static void SetEnemySeen(EnemyType type)
    {
        string enemyType = Enum.GetName(typeof(EnemyType), type);
        PlayerPrefs.SetInt(EnemySavePrefix + enemyType, 1);
    }
}
